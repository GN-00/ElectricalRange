using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[ProformaInvoices]")]
    public class ProformaInvoice : Base
    {
        private string _Code;
        private DateTime _Date = DateTime.Now;
        private string _VATNumber = Database.CompanyVAT.ToString();
        private string _CustomerVATNumber;
        private string _CustomerName;
        private string _CustomerNameArabic;
        private string _Contact;
        private string _Project;
        private string _Address;
        private string _Type = "Invoice";
        private double _Amount;
        private double? _Percentage;
        private double _NetPrice;
        private double _VAT;
        private double _VATValue;
        private double _GrossPrice;
        private string _Description;
        private string _DescriptionArabic;
        private int _Panels;

        [Key]
        public int Id { get; set; }

        public int JobOrderId { get; set; }
        public int CustomerId { get; set; }
        public string JobOrderCode { get; set; }
        public string PurchaseOrder { get; set; }

        public int Number { get; set; }
        public int Year { get; set; }
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

        [Write(false)]
        public string DateInfo => Date.ToString("dd-MM-yyyy");

        public string VATNumber
        {
            get => _VATNumber;
            set => SetValue(ref _VATNumber, value);
        }
        public string CustomerVATNumber
        {
            get => _CustomerVATNumber;
            set => SetValue(ref _CustomerVATNumber, value);
        }

        public string CustomerName
        {
            get => _CustomerName;
            set => SetValue(ref _CustomerName, value)
                  .UpdateProperties(this, nameof(CustomerNameArabic));
        }
        public string CustomerNameArabic
        {
            get => _CustomerNameArabic;
            set => SetValue(ref _CustomerNameArabic, value)
                  .UpdateProperties(this, nameof(CustomerNameArabic));
        }

        [Write(false)]
        public string CustomerNameInfo
        {
            get
            {
                if (CustomerNameArabic != null)
                    return $"{CustomerName.TrimEnd()} ({CustomerNameArabic.TrimEnd()})";
                else
                    return CustomerName;
            }
        }

        public string Contact
        {
            get => _Contact;
            set => SetValue(ref _Contact, value);
        }
        public string Project
        {
            get => _Project;
            set => SetValue(ref _Project, value);
        }
        public string Address
        {
            get => _Address;
            set => SetValue(ref _Address, value);
        }

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

        public double Amount
        {
            get => _Amount;
            set => SetValue(ref _Amount, value);
        }
        public double? Percentage
        {
            get => _Percentage;
            set => SetValue(ref _Percentage, value);
        }

        [Write(false)]
        public string PercentageArabic
        {
            get => DataInput.Input.ToArabicNumbers(Percentage.ToString());
        }

        public string Type
        {
            get => _Type;
            set => SetValue(ref _Type, value);
        }

        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }

        public string DescriptionArabic
        {
            get => _DescriptionArabic;
            set => SetValue(ref _DescriptionArabic, value);
        }

        public int Panels
        {
            get => _Panels;
            set => SetValue(ref _Panels, value);
        }
    }
}
