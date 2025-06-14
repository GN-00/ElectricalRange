using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[PurchaseOrders]")]
    public class PurchaseOrder : Base
    {
        [Key]
        public int ID { get; set; }

        [Write(false)]
        public int? QuotationID { get; set; }
        public int? JobOrderID { get; set; }

        [Write(false)]
        public int? AttachmentId { get; set; }

        [Write(false)]
        public string JobOrderCode { get; set; }

        [Write(false)]
        public string Customer { get; set; }

        private string _Number;
        public string Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }

        private DateTime? _Date = DateTime.Now;
        public DateTime? Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }

        private decimal _NetPrice;

        [Write(false)]
        public decimal NetPrice
        {
            get => _NetPrice;
            set => SetValue(ref _NetPrice, value);
        }

        private decimal _VAT;

        [Write(false)]
        public decimal VAT
        {
            get => _VAT;
            set => SetValue(ref _VAT, value);
        }

        private decimal _VATPercentage;

        [Write(false)]
        public decimal VATPercentage
        {
            get => _VATPercentage;
            set => SetValue(ref _VATPercentage, value);
        }

        private decimal _VATValue;

        [Write(false)]
        public decimal VATValue
        {
            get => _VATValue;
            set => SetValue(ref _VATValue, value);
        }

        private decimal _GrossPrice;

        [Write(false)]
        public decimal GrossPrice
        {
            get => _GrossPrice;
            set => SetValue(ref _GrossPrice, value);
        }
    }
}
