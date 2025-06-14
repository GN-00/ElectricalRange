using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[MoneyTransactions]")]
    public class MoneyTransaction : Base
    {
        private DateTime _Date = DateTime.Today;
        private string _Description;
        private decimal _Amount;
        private string _AccountName;
        private decimal _AccountBalance;
        private string _Code;

        [Key]
        public int ID { get; set; }
        public int AccountID { get; set; }
        public int? EmployeeID { get; set; }
        public int? JobOrderID { get; set; }
        public int? CustomerID { get; set; }
        public int? SupplierID { get; set; }
        public int? PurchaseOrderID { get; set; }
        public int? SupplierInvoiceID { get; set; }
        public int? ExpenseInvoiceID { get; set; }

        public int? AttachmentID { get; set; }

        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value)
                  .UpdateProperties(this, nameof(Date));
        }

        [Write(false)]
        public string DateInfo => Date.ToString("dd/MM/yyyy");

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

        [Write(false)]
        public string AccountName
        {
            get => _AccountName;
            set => SetValue(ref _AccountName, value);
        }
        [Write(false)]
        public decimal AccountBalance
        {
            get => _AccountBalance;
            set => SetValue(ref _AccountBalance, value);
        }
        [Write(false)]
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
    }
}
