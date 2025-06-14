using System;

namespace ProjectsNow.Data.Finance
{
    public class TransactionInfo
    {
        public int ID { get; set; }
        #region Ids
        public int AccountID { get; set; }
        public int? EmployeeID { get; set; }
        public int? JobOrderID { get; set; }
        public int? CustomerID { get; set; }
        public int? SupplierID { get; set; }
        public string ItemsPO { get; set; }
        public int? SupplierInvoiceID { get; set; }
        #endregion
        public DateTime Date { get; set; } //
        public string DateInfo 
        {
            get => Date.ToString("dd-MM-yyyy");
        } //
        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
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
        public string Code { get; set; }
        public string CodeInfo
        {
            get
            {
                if (JobOrderID == 0)
                {
                    return Database.Store.Code;
                }
                else
                {
                    return Code;
                }
            }
        }
        public string Description { get; set; } //
        public decimal Amount { get; set; } //
        public string Type { get; set; }
        public string TypeInfo
        {
            get
            {
                if (Type == Enums.MoneyTransactionTypes.Project.ToString())
                {
                    return "In";
                }
                else
                {
                    return "Out";
                }
            }
        }


        //Post
    }
}
