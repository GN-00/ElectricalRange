using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[CustomersInvoices]")]
    public class Invoice : Base
    {
        private string _Code;
        private DateTime _Date = DateTime.Now;
        private double _NetPrice;
        private double _VAT;
        private double _VATPercentage;
        private double _VATValue;
        private double _GrossPrice;
        private double _Items;
        private double _Paid;

        [Key]
        public int Id { get; set; }

        [Write(false)]
        public int? AttachmentId { get; set; }

        public int? CustomerId { get; set; }
        public int? JobOrderId { get; set; }
        public int? ContactId { get; set; }
        public string Type { get; set; }
        public int Number { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Code 
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value)
                  .UpdateProperties(this, nameof(DateInfo));
        }

        [Write(false)]
        public string DateInfo => Date.ToString("dd-MM-yyyy");
        public double NetPrice
        {
            get => _NetPrice;
            set => SetValue(ref _NetPrice, value);
        }
        public double VAT
        {
            get => _VAT;
            set => SetValue(ref _VAT, value);
        }
        public double VATPercentage
        {
            get => _VATPercentage;
            set => SetValue(ref _VATPercentage, value);
        }
        public double VATValue
        {
            get => _VATValue;
            set => SetValue(ref _VATValue, value);
        }
        public double GrossPrice
        {
            get => _GrossPrice;
            set => SetValue(ref _GrossPrice, value);
        }
        public bool IsPercentageInvoice { get; set; } = false;

        [Write(false)]
        public long CompanyVAT => Database.CompanyVAT;

        [Write(false)]
        public string InvoiceNumber => Code;

        [Write(false)]
        public string JobOrderCode { get; set; }
        
        public string PurchaseOrderNumber { get; set; }

        [Write(false)]
        public string CustomerName { get; set; }

        [Write(false)]
        public string CustomerNameArabic { get; set; }

        [Write(false)]
        public string CustomerNameInfo
        {
            get
            {
                if (CustomerNameArabic != null)
                {
                    return $"{CustomerName.TrimEnd()} ({CustomerNameArabic.TrimEnd()})";
                }
                else
                {
                    return CustomerName;
                }
            }
        }

        [Write(false)]
        public string Attention { get; set; }

        [Write(false)]
        public string ProjectName { get; set; }

        public string Address { get; set; }

        [Write(false)]
        public string CustomerVAT { get; set; }

        [Write(false)]
        public double Items
        {
            get => _Items;
            set => SetValue(ref _Items, value);
        }

        [Write(false)]
        public double Paid
        {
            get => _Paid;
            set => SetValue(ref _Paid, value)
                  .UpdateProperties(this, nameof(Balance), nameof(BalanceStatus));
        }

        [Write(false)]
        public double Balance => GrossPrice - Paid;

        [Write(false)]
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

        [Write(false)]
        public string Contact { get; set; }

        public bool IsReturn { get; set; }
    }
}
