using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using static Microsoft.ML.DataOperationsCatalog;

namespace Sage.CRE.PoAnomalyDetector
{
    public class Trainer
    {
        public static void TrainModel(string fullDataSetFilePath, string outPutPath)
        {
            string trainDataSetFilePath = Path.Combine(Path.GetTempPath(), "trainAp.csv");
            string testDataSetFilePath = Path.Combine(Path.GetTempPath(), "testAp.csv");
            string modelPath = Path.Combine(outPutPath, "model.zip");

            var mlContext = new MLContext();

            PrepDatasets(mlContext, fullDataSetFilePath, trainDataSetFilePath, testDataSetFilePath);

            // Load Datasets
            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<ApInvoiceData>(trainDataSetFilePath, separatorChar: ',', hasHeader: true);
            IDataView testDataView = mlContext.Data.LoadFromTextFile<ApInvoiceData>(testDataSetFilePath, separatorChar: ',', hasHeader: true);

            // Train Model
            ITransformer model = Train(mlContext, trainingDataView);

            // Evaluate quality of Model
            EvaluateModel(mlContext, model, testDataView);

            // Save model
            mlContext.Model.Save(model, trainingDataView.Schema, modelPath);
            Console.WriteLine("Saved model to " + modelPath);
        }

        private static void PrepDatasets(MLContext mlContext, string fullDataSetFilePath, string trainDataSetFilePath, string testDataSetFilePath)
        {
            // Only prep-datasets if train and test datasets don't exist yet
            if (!File.Exists(trainDataSetFilePath) &&
                !File.Exists(testDataSetFilePath))
            {
                Console.WriteLine("===== Preparing train/test datasets =====");

                // Load the original single dataset
                IDataView originalFullData = mlContext.Data.LoadFromTextFile<ApInvoiceData>(fullDataSetFilePath, separatorChar: ',', hasHeader: true);

                // Split the data 80:20 into train and test sets, train and evaluate.
                TrainTestData trainTestData = mlContext.Data.TrainTestSplit(originalFullData, testFraction: 0.2, seed: 1);

                // 80% of original dataset
                IDataView trainData = trainTestData.TrainSet;

                // 20% of original dataset
                IDataView testData = trainTestData.TestSet;

                // Inspect TestDataView to make sure there are true and false observations in test dataset, after spliting 
                InspectData(mlContext, testData, 4);

                // Save train split
                using (var fileStream = File.Create(trainDataSetFilePath))
                {
                    mlContext.Data.SaveAsText(trainData, fileStream, separatorChar: ',', headerRow: true, schema: true);
                }

                // Save test split 
                using (var fileStream = File.Create(testDataSetFilePath))
                {
                    mlContext.Data.SaveAsText(testData, fileStream, separatorChar: ',', headerRow: true, schema: true);
                }
            }
        }

        private static ITransformer Train(MLContext mlContext, IDataView trainDataView)
        {
            // Get all the feature column names (All except the Label and the IdPreservationColumn)
            string[] featureColumnNames = trainDataView.Schema.AsQueryable()
                .Select(column => column.Name)                               // Get all the column names
                .Where(name => name != nameof(ApInvoiceData.Label)) // Do not include the Label column
                .Where(name => name != nameof(ApInvoiceData.Record))
                .Where(name => name != "IdPreservationColumn")               // Do not include the IdPreservationColumn/StratificationColumn
                .ToArray();

            // Create the data process pipeline
            IEstimator<ITransformer> dataProcessPipeline = mlContext.Transforms.Concatenate("Features", featureColumnNames)
                                                                               .Append(mlContext.Transforms.DropColumns(new string[] { nameof(ApInvoiceData.Record) }))
                                                                               .Append(mlContext.Transforms.NormalizeLpNorm(outputColumnName: "NormalizedFeatures", inputColumnName: "Features"));

            // In Anomaly Detection, the learner assumes all training examples have label 0, as it only learns from normal examples.
            // If any of the training examples has label 1, it is recommended to use a Filter transform to filter them out before training:
            IDataView normalTrainDataView = mlContext.Data.FilterRowsByColumn(trainDataView, columnName: nameof(ApInvoiceData.Label), lowerBound: 0, upperBound: 1);

            // (OPTIONAL) Peek data (such as 2 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            ConsoleHelper.PeekDataViewInConsole(mlContext, normalTrainDataView, dataProcessPipeline, 2);
            ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "NormalizedFeatures", normalTrainDataView, dataProcessPipeline, 2);

            var options = new RandomizedPcaTrainer.Options
            {
                FeatureColumnName = "NormalizedFeatures",   // The name of the feature column. The column data must be a known-sized vector of Single.
                ExampleWeightColumnName = null,				// The name of the example weight column (optional). To use the weight column, the column data must be of type Single.
                Rank = 16,									// The number of components in the PCA.
                Oversampling = 20,							// Oversampling parameter for randomized PCA training.
                EnsureZeroMean = true,						// If enabled, data is centered to be zero mean.
                Seed = 1									// The seed for random number generation.
            };

			// Create an anomaly detector. Its underlying algorithm is randomized PCA.
			IEstimator<ITransformer> trainer = mlContext.AnomalyDetection.Trainers.RandomizedPca(options: options);

			EstimatorChain<ITransformer> trainingPipeline = dataProcessPipeline.Append(trainer);

            ConsoleHelper.ConsoleWriteHeader("=============== Training model ===============");

			TransformerChain<ITransformer> model = trainingPipeline.Fit(normalTrainDataView);

            ConsoleHelper.ConsoleWriteHeader("=============== End of training process ===============");

            return model;
        }

        private static void EvaluateModel(MLContext mlContext, ITransformer model, IDataView testDataView)
        {
            // Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");

            var predictions = model.Transform(testDataView);

            AnomalyDetectionMetrics metrics = mlContext.AnomalyDetection.Evaluate(predictions);

            ConsoleHelper.PrintAnomalyDetectionMetrics("RandomizedPca", metrics);
        }

        private static void InspectData(MLContext mlContext, IDataView data, int records)
        {
            // We want to make sure we have both True and False observations
            Console.WriteLine("Show 4 anomalies (true)");
            ShowObservationsFilteredByLabel(mlContext, data, label: true, count: records);

            Console.WriteLine("Show 4 normal entries (false)");
            ShowObservationsFilteredByLabel(mlContext, data, label: false, count: records);
        }

        private static void ShowObservationsFilteredByLabel(MLContext mlContext, IDataView dataView, bool label = true, int count = 2)
        {
            // Convert to an enumerable of user-defined type. 
            var data = mlContext.Data.CreateEnumerable<ApInvoiceData>(dataView, reuseRowObject: false)
                                            .Where(x => Math.Abs(x.Label - (label ? 1 : 0)) < float.Epsilon)
                                            // Take a couple values as an array.
                                            .Take(count)
                                            .ToList();

            // Print to console
            data.ForEach(row => { Console.WriteLine(row.ToString()); });
        }
    }
}
