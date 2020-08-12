using System;
using Microsoft.ML.Data;

namespace Sage.CRE.PoAnomalyDetector
{
    public class ApInvoiceData
    {
        [LoadColumn(0)]
        public int Record { get; set; }
        [LoadColumn(1)]
        public string Invoice { get; set; }
        [LoadColumn(2)]
        public int SubcontractNum { get; set; }
        [LoadColumn(3)]
        public int Vendor { get; set; }
        [LoadColumn(4)]
        public int Job { get; set; }
        [LoadColumn(5)]
        public int Phase { get; set; }
        [LoadColumn(6)]
        public string Description { get; set; }
        [LoadColumn(7)]
        public DateTime InvoiceDate { get; set; }
        [LoadColumn(8)]
        public DateTime DueDate { get; set; }
        [LoadColumn(9)]
        public DateTime Discovered { get; set; }
        [LoadColumn(10)]
        public int Type { get; set; }
        [LoadColumn(11)]
        public int Status { get; set; }
        [LoadColumn(12)]
        public float Retention { get; set; }
        [LoadColumn(13)]
        public float Paid { get; set; }
        [LoadColumn(14)]
        public float InvoiceTotal { get; set; }
        [LoadColumn(15)]
        public float TotalPaid { get; set; }
        [LoadColumn(16)]
        public float Balance { get; set; }
        [LoadColumn(17)]
        public float NetDue { get; set; }
        [LoadColumn(18)]
        public int Period { get; set; }
        [LoadColumn(19)]
        public DateTime Entered { get; set; }
        [LoadColumn(20)]
        public int LinkNum { get; set; }
        [LoadColumn(21)]
        public string UserName { get; set; }
        [LoadColumn(22)]
        public int BatchNum { get; set; }
        [LoadColumn(23)]
        public int PostingYear { get; set; }
    }
}
