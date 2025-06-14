using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Store
{
    [Table("[Store].[Invoices]")]
    public class SupplierInvoice : Base
    {
        private string _Number;
        private DateTime _Date;
        private string _SupplierCode;
        private string _SupplierName;
        private string _PurchaseOrder;
        private int? _InternalTransferNumber;

        [Key]
        public int ID { get; set; }
        public int JobOrderID { get; set; }
        public int PurchaseOrderID { get; set; }
        public int? AttachmentID { get; set; }
        public int SupplierID { get; set; }

        [Write(false)]
        public string PurchaseOrder
        {
            get => _PurchaseOrder;
            set => SetValue(ref _PurchaseOrder, value);
        }

        public int? InternalTransferNumber
        {
            get => _InternalTransferNumber;
            set => SetValue(ref _InternalTransferNumber, value);
        }

        public string Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }
        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }
        [Write(false)]
        public string SupplierCode
        {
            get => _SupplierCode;
            set => SetValue(ref _SupplierCode, value);
        }
        [Write(false)]
        public string SupplierName
        {
            get => _SupplierName;
            set => SetValue(ref _SupplierName, value);
        }

        [Write(false)]
        public int ReturnPeriod { get; set; }

        [Write(false)]
        public bool CanReturn
        {
            get
            {
                if (DateTime.Now > Date.AddDays(ReturnPeriod))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
