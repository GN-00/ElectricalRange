using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Suppliers
{
    public class SupplierInvoice : Base
    {
        private decimal _Paid;

        [Key]
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public int JobOrderID { get; set; }
        public int SupplierID { get; set; }
        public string Code { get; set; }
        public string InvoiceNumber { get; set; }
        public string SupplierName { get; set; }
        public DateTime Date { get; set; }
        public string QuotationCode { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public decimal VAT { get; set; }
        public decimal InvoiceTotal { get; set; }
        [Write(false)]
        public decimal Paid
        {
            get { return _Paid; }
            set => SetValue(ref _Paid, value)
                  .UpdateProperties(this, nameof(Balance), nameof(BalanceView), nameof(Status));
        }
        public decimal Balance => InvoiceTotal - Paid;
        public string BalanceView => Balance.ToString("N2");
        public string Status
        {
            get
            {
                decimal percentage = Paid / InvoiceTotal * 100;
                if (percentage >= 100)
                {
                    return "Paid in Full";
                }
                else
                {
                    return $"Paid {percentage:N2}%";
                }
            }
        }
    }
}
