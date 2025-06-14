using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data
{
    [Table("[Purchase].[Orders]")]
    public class CompanyPO : Base
    {
        private string _SupplierName;
        private int _Number;
        private string _Code;
        private string _QuotationCode = "-";
        private string _SupplierCode;
        private string _SupplierAttentionID;
        private string _SupplierAttention;
        private DateTime _Date;
        private string _DeliverToPlace;
        private string _DeliverToPerson;
        private string _DeliveryAddress;
        private string _Payment;
        private decimal _VAT;
        private string _CompanyName;
        private string _CompanyFullName;
        private long _CompanyVATNumber;
        private long _SupplierVATNumber;
        private decimal _NetPrice;
        private decimal _VATValue;
        private decimal _GrossPrice;
        private decimal _Paid;
        private decimal _InvoicedPrice;
        private string _Note;

        [Key]
        public int ID { get; set; }
        public int JobOrderID { get; set; }

        [Write(false)]
        public string JobOrderCode { get; set; }

        [Write(false)]
        public int? AttachmentId { get; set; }

        public int CompanyID { get; set; } = 1;
        public int? SupplierID { get; set; }
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
        public string QuotationCode
        {
            get => _QuotationCode;
            set => SetValue(ref _QuotationCode, value);
        }
        [Write(false)]
        public string SupplierName
        {
            get => _SupplierName;
            set => SetValue(ref _SupplierName, value);
        }
        [Write(false)]
        public string SupplierCode
        {
            get => _SupplierCode;
            set => SetValue(ref _SupplierCode, value);
        }
        public string SupplierAttentionID
        {
            get => _SupplierAttentionID;
            set => SetValue(ref _SupplierAttentionID, value);
        }
        [Write(false)]
        public string SupplierAttention
        {
            get => _SupplierAttention;
            set => SetValue(ref _SupplierAttention, value);
        }
        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }
        public string DeliverToPlace
        {
            get => _DeliverToPlace;
            set => SetValue(ref _DeliverToPlace, value);
        }
        public string DeliverToPerson
        {
            get => _DeliverToPerson;
            set => SetValue(ref _DeliverToPerson, value);
        }
        public string DeliveryAddress
        {
            get => _DeliveryAddress;
            set => SetValue(ref _DeliveryAddress, value);
        }
        public string Payment
        {
            get => _Payment;
            set => SetValue(ref _Payment, value);
        }
        public decimal VAT
        {
            get => _VAT;
            set => SetValue(ref _VAT, value);
        }

        [Write(false)]
        public long SupplierVATNumber
        {
            get => _SupplierVATNumber;
            set => SetValue(ref _SupplierVATNumber, value);
        }

        [Write(false)]
        public string CompanyName
        {
            get => _CompanyName;
            set => SetValue(ref _CompanyName, value);
        }
        [Write(false)]
        public string CompanyFullName
        {
            get => _CompanyFullName;
            set => SetValue(ref _CompanyFullName, value);
        }

        [Write(false)]
        public long CompanyVATNumber
        {
            get => _CompanyVATNumber;
            set => SetValue(ref _CompanyVATNumber, value)
                  .UpdateProperties(this, nameof(CompanyVATNumberInfo));
        }
        [Write(false)]
        public string CompanyVATNumberInfo
        {
            get => $"VAT: {CompanyVATNumber}";
        }
        [Write(false)]
        public string SupplierVATNumberInfo
        {
            get => $"VAT: {SupplierVATNumber}";
        }

        [Write(false)]
        public decimal NetPrice
        {
            get => _NetPrice;
            set => SetValue(ref _NetPrice, value);
        }

        [Write(false)]
        public decimal GrossPrice
        {
            get => _GrossPrice;
            set => SetValue(ref _GrossPrice, value);
        }

        [Write(false)]
        public decimal VATValue
        {
            get => _VATValue;
            set => SetValue(ref _VATValue, value);
        }

        [Write(false)]
        public decimal Paid
        {
            get => _Paid;
            set => SetValue(ref _Paid, value)
                  .UpdateProperties(this, nameof(Balance), nameof(Paid));
        }

        [Write(false)]
        public decimal Balance
        {
            get => GrossPrice - Paid;
        }

        [Write(false)]
        public decimal PaidPercentage
        {
            get
            {
                if (GrossPrice > 0)
                {
                    return (Paid / GrossPrice) * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Write(false)]
        public string PaidInfo
        {
            get
            {
                if (PaidPercentage == 100)
                {
                    return "Complete";
                }
                else if (PaidPercentage > 100)
                {
                    return "OverPaid";
                }
                else
                {
                    return "Uncomplete";
                }
            }
        }

        [Write(false)]
        public decimal InvoicedPrice
        {
            get => _InvoicedPrice;
            set => SetValue(ref _InvoicedPrice, value)
                  .UpdateProperties(this, nameof(InvoicedPriceInfo));
        }

        [Write(false)]
        public string InvoicedPriceInfo
        {
            get => $"({Paid - InvoicedPrice:N2}) {InvoicedPrice:N2} Invoiced form {Paid:N2}";
        }

        public int Revise { get; set; }
        public DateTime? ReviseDate { get; set; }
        public int? OriginalOrderID { get; set; }
        public bool Revised { get; set; }

        public string Note
        {
            get => _Note;
            set => SetValue(ref _Note, value);
        }
    }
}
