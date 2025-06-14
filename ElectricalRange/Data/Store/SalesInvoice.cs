using Dapper.Contrib.Extensions;

using ProjectsNow.Data.Application;

using System;

namespace ProjectsNow.Data.Store
{
    [Table("[Store].[SalesInvoices]")]
    public class SalesInvoice : Base
    {
        private int _Number;
        private string _Code;
        private DateTime _Date = DateTime.Now;
        private decimal _VAT = AppData.VAT;
        private decimal _NetPrice;
        private bool _IsPosted;

        [Key]
        public int Id { get; set; }
        public int? CustomerId { get; set; } 
        public int? ContactId { get; set; }

        [Write(false)]
        public string Customer { get; set; }

        [Write(false)]
        public string Contact { get; set; }

        public int Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }
        public string Address { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public bool IsPosted
        {
            get => _IsPosted;
            set => SetValue(ref _IsPosted, value);
        }

        public decimal VAT
        {
            get => _VAT;
            set => SetValue(ref _VAT, value);
        }


        [Write(false)]
        public decimal NetPrice
        {
            get => _NetPrice;
            set => SetValue(ref _NetPrice, value)
                  .UpdateProperties(this, nameof(VATValue), nameof(GrossPrice));
        }

        [Write(false)]
        public decimal VATValue => NetPrice * VAT / 100m;

        [Write(false)]
        public decimal GrossPrice => NetPrice * (1 + VAT / 100m);


        [Write(false)]
        public string CompanyVAT => AppData.CompanyData.VATNumber.ToString();

        [Write(false)]
        public string CustomerVAT { get; set; }
    }
}
