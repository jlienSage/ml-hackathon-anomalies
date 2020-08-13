namespace Sage.CRE.ApAnomalyDetector
{
    public class ApInvoicePrediction
    {
        public float Label { get; set; }
        public float Score { get; set; }
        public bool PredictedLabel { get; set; }

        public override string ToString()
        {
            return $"Predicted label: {PredictedLabel} (score: {Score})";
        }
    }
}
