using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Customers
{
    [Table("[Customer].[Customers]")]
    public class Customer : Base, IAccess, IPartner
    {
        private string _Code;
        private string _CR;
        private string _CustomerName;
        private string _CustomerNameArabic;
        private string _City;
        private string _Address;
        private int? _POBox;
        private string _Phone;
        private string _Email;
        private string _Website;
        private DateTime _StartRelation = DateTime.Now;
        private string _VATNumber;
        private string _Note;
        private string _SalesmanName;
        private double _CreditLimit;

        [Key]
        public int CustomerID { get; set; }

        [Write(false)]
        public int Id => CustomerID;
        public int SalesmanID { get; set; }
        public int BankID { get; set; }

        public string CR
        {
            get => _CR;
            set => SetValue(ref _CR, value);
        }

        [Write(false)]
        public int? CRAttachmentId { get; set; }

        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
        public string CustomerName
        {
            get => _CustomerName;
            set => SetValue(ref _CustomerName, value);
        }

        [Write(false)]
        public string Group
        {
            get => CustomerName.Substring(0, 1).ToUpper();
        }
        public string CustomerNameArabic
        {
            get => _CustomerNameArabic;
            set => SetValue(ref _CustomerNameArabic, value);
        }
        public string City
        {
            get => _City;
            set => SetValue(ref _City, value);
        }

        [Write(false)]
        public int? AddressAttachmentId { get; set; }
        public string Address
        {
            get => _Address;
            set => SetValue(ref _Address, value);

        }
        public int? POBox
        {
            get => _POBox;
            set => SetValue(ref _POBox, value);
        }
        public string Phone
        {
            get => _Phone;
            set => SetValue(ref _Phone, value);
        }
        public string Email
        {
            get => _Email;
            set => SetValue(ref _Email, value);
        }
        public string Website
        {
            get => _Website;
            set => SetValue(ref _Website, value);
        }
        public DateTime StartRelation
        {
            get => _StartRelation;
            set => SetValue(ref _StartRelation, value);
        }

        [Write(false)]
        public int? VATAttachmentId { get; set; }
        public string VATNumber
        {
            get => _VATNumber;
            set => SetValue(ref _VATNumber, value);
        }
        public string Note
        {
            get => _Note;
            set => SetValue(ref _Note, value);
        }

        [Write(false)]
        public string SalesmanName
        {
            get => _SalesmanName;
            set => SetValue(ref _SalesmanName, value);
        }

        public double CreditLimit
        {
            get => _CreditLimit;
            set => SetValue(ref _CreditLimit, value);
        }
    }
}
