using Microsoft.ML.Data;

namespace Sage.CRE.PoAnomalyDetector
{
    public class ApInvoicePrediction
    {
        //vector to hold alert,score,p-value values
        [VectorType(3)]
        public double[] Prediction { get; set; }
    }
}
