using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace Sage.CRE.PoAnomalyDetector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dataPath = args[0];
            var docSize = int.Parse(args[1]);

            var mlContext = new MLContext();
            IDataView dataView = mlContext.Data.LoadFromTextFile<ApInvoiceData>(path: dataPath, hasHeader: true, separatorChar: ',');

            var predictions = DetectSpike(mlContext, docSize, dataView);

            Console.WriteLine("Alert\tScore\tP-Value");
            foreach (var p in predictions)
            {
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}";

                if (p.Prediction[0] == 1)
                {
                    results += " <-- Spike detected";
                }

                Console.WriteLine(results);
            }
            Console.WriteLine("");
        }

        private static IEnumerable<ApInvoicePrediction> DetectSpike(MLContext mlContext, int docSize, IDataView productSales)
        {
            IidSpikeEstimator iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(outputColumnName: nameof(ApInvoicePrediction.Prediction), inputColumnName: nameof(ApInvoiceData.InvoiceTotal), confidence: 95, pvalueHistoryLength: docSize / 4);
            ITransformer iidSpikeTransform = iidSpikeEstimator.Fit(CreateEmptyDataView(mlContext));
            IDataView transformedData = iidSpikeTransform.Transform(productSales);
            return mlContext.Data.CreateEnumerable<ApInvoicePrediction>(transformedData, reuseRowObject: false);
        }

        private static IDataView CreateEmptyDataView(MLContext mlContext)
        {
            // Create empty DataView. We just need the schema to call Fit() for the time series transforms
            IEnumerable<ApInvoiceData> enumerableData = new List<ApInvoiceData>();
            return mlContext.Data.LoadFromEnumerable(enumerableData);
        }
    }
}
