using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[MoneySubTransactions]")]
    public class MoneySubTransaction : Base
    {
        private DateTime _Date = DateTime.Today;
        private string _Description;
        private decimal _Amount;

        [Key]
        public int Id { get; set; }
        public int? PurchaseOrderID { get; set; }
        public int? SupplierInvoiceID { get; set; }
        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }
        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }
        public decimal Amount
        {
            get => _Amount;
            set => SetValue(ref _Amount, value);
        }
        public string Type { get; set; }
    }
}
