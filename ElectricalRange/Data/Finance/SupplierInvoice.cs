using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    public class SupplierInvoice : Base
    {
        private double _Paid;

        [Key]
        public int ID { get; set; }
        public int JobOrderID { get; set; }
        public int CustomerID { get; set; }
        public int SupplierID { get; set; }
        public int PurchaseOrderID { get; set; }
        public string PurchaseOrder { get; set; }
        public string JobOrderCode { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string DateInfo => Date.ToString("dd-MM-yyyy");

        public double Paid
        {
            get { return _Paid; }
            set => SetValue(ref _Paid, value)
                  .UpdateProperties(this, nameof(Balance));
        }
        public double Balance
        {
            get => GrossPrice - Paid; 
        }
        public int BalanceStatus
        {
            get
            {
                if (Balance > 1)
                    return 1;
                else
                    return 0;
            }
        }

        public double NetPrice { get; set; }
        public double GrossPrice { get; set; }
        public double VATValue { get; set; }

        public string SupplierName { get; set; }

        public int Year => Date.Year;
        public int Month => Date.Month;
        public double Items { get; set; }
        public double Return { get; set; }
        public double ReturnValue { get; set; }
    }
}
