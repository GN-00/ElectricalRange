using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[MoneyTransactions]")]
    public class Transaction
    {
        [Key]
        public int ID { get; set; }

        #region Ids

        public int AccountID { get; set; }
        public int? EmployeeID { get; set; }
        public int? JobOrderID { get; set; }
        public int? CustomerID { get; set; }
        public int? SupplierID { get; set; }
        public int? PurchaseOrderID { get; set; }
        public int? SupplierInvoiceID { get; set; }

        #endregion
        public DateTime Date { get; set; }
        public string DateInfo
        {
            get => Date.ToString("dd-MM-yyyy");
        }

        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } //Coming From Projects, Purchase Order ... etc
        public string Category { get; set; }


        [Write(false)]
        public string CustomerName { get; set; }

        [Write(false)]
        public string SupplierName { get; set; }

        [Write(false)]
        public string Client
        {
            get
            {
                if (Type == Enums.MoneyTransactionTypes.Project.ToString())
                {
                    return CustomerName;
                }
                else if (Type == Enums.MoneyTransactionTypes.SupplierInvoice.ToString())
                {
                    return SupplierName;
                }
                else if (Type == Enums.MoneyTransactionTypes.Transportation.ToString())
                {
                    return CustomerName;
                }
                else
                {
                    return "-";
                }
            }
        }
    }
}
