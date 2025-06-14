using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[NotificationForDawnPayment]")]
    public class DownPayment : Base
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int JobOrderId { get; set; }
        public string QuotationCode { get; set; }
        public string JobOrderCode { get; set; }

        public string CustomerName { get; set; }
        public string ProjectName { get; set; }
        public string Attention { get; set; }

        public string PurchaseOrderNumber { get; set; }
        public DateTime PurchaseOrderDate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal DownPaymentAmount { get; set; }
        public decimal OutstandingAmount { get; set; }

        public string TotalAmountText => $"({DataInput.Input.NumberToSRWords(TotalAmount)})";
        public string DownPaymentAmountText => $"({DataInput.Input.NumberToSRWords(DownPaymentAmount)})";
        public string OutstandingAmountText => $"({DataInput.Input.NumberToSRWords(OutstandingAmount)})";
    }
}
