using System;
using System.Linq;
using Microsoft.ML;

namespace Sage.CRE.ApAnomalyDetector
{
    public class Predictor
    {
        public static void Predict(string modelFile, string dataSetFile, int numberOfPredictions)
        {      
            var mlContext = new MLContext();

            // Load data as input for predictions
            IDataView inputDataForPredictions = mlContext.Data.LoadFromTextFile<ApInvoiceData>(dataSetFile, separatorChar: ',', hasHeader: true);

            Console.WriteLine($"Predictions from saved model:");

            ITransformer model = mlContext.Model.Load(modelFile, out var inputSchema);
            
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ApInvoiceData, ApInvoicePrediction>(model);
            
            Console.WriteLine($"\n \n Test {numberOfPredictions} records, from the test datasource, that should be predicted to be anomalies (true):");

            mlContext.Data.CreateEnumerable<ApInvoiceData>(inputDataForPredictions, reuseRowObject: false)
                        .Where(x => x.Label > 0)
                        .Take(numberOfPredictions)
                        .Select(testData => testData)
                        .ToList()
                        .ForEach(testData =>
                                    {
                                        Console.WriteLine($"--- Record ---");
                                        Console.WriteLine(testData.ToString());
                                        Console.WriteLine(predictionEngine.Predict(testData).ToString());
                                        Console.WriteLine($"-------------------");
                                    });


            Console.WriteLine($"\n \n Test {numberOfPredictions} records, from the test datasource, that should NOT be predicted to be anomalies (false):");

            mlContext.Data.CreateEnumerable<ApInvoiceData>(inputDataForPredictions, reuseRowObject: false)
                       .Where(x => x.Label < 1)
                       .Take(numberOfPredictions)
                       .ToList()
                       .ForEach(testData =>
                                   {
                                       Console.WriteLine($"--- Record ---");
                                       Console.WriteLine(testData.ToString());
                                       Console.WriteLine(predictionEngine.Predict(testData).ToString());
                                       Console.WriteLine($"-------------------");
                                   });
        }
    }
}
