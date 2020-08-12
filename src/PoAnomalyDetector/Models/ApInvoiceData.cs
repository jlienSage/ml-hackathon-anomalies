using System;
using Microsoft.ML.Data;

namespace Sage.CRE.PoAnomalyDetector
{
    public class ApInvoiceData
    {
        [LoadColumn(0)]
        public int Record { get; set; }
        [LoadColumn(1)]
        public float SubcontractNum { get; set; }
        [LoadColumn(2)]
        public float Vendor { get; set; }
        [LoadColumn(3)]
        public float Job { get; set; }
        [LoadColumn(4)]
        public float Phase { get; set; }
        [LoadColumn(5)]
        public float Type { get; set; }
        [LoadColumn(6)]
        public float Status { get; set; }
        [LoadColumn(7)]
        public float Retention { get; set; }
        [LoadColumn(8)]
        public float Paid { get; set; }
        [LoadColumn(9)]
        public float InvoiceTotal { get; set; }
        [LoadColumn(10)]
        public float TotalPaid { get; set; }
        [LoadColumn(11)]
        public float Balance { get; set; }
        [LoadColumn(12)]
        public float NetDue { get; set; }
        [LoadColumn(13)]
        public float Period { get; set; }
        [LoadColumn(14)]
        public float LinkNum { get; set; }
        [LoadColumn(15)]
        public float BatchNum { get; set; }
        [LoadColumn(16)]
        public float PostingYear { get; set; }
        [LoadColumn(17)]
        public float Label { get; set; }

        public override string ToString()
        {
            return $"Record {Record} ({Label})";
        }
    }
}
