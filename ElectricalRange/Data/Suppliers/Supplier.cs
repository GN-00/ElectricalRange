using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Suppliers
{
    [Table("[Supplier].[Suppliers]")]
    public class Supplier : Base, IAccess, IPartner
    {
        private string _CR;
        private string _Code;
        private string _Name;
        private string _ArabicName;
        private string _City;
        private string _Address;
        private int? _POBox;
        private string _Phone;
        private string _Email;
        private string _Website;
        private DateTime _StartRelation = DateTime.Now;
        private string _VATNumber;
        private string _Note;
        private double _CreditLimit;
        private int _ReturnPeriod;

        [Key]
        public int ID { get; set; }


        //[Write(false)]
        //public int Id => ID;

        public int BankID { get; set; }

        public string CR
        {
            get => _CR;
            set => SetValue(ref _CR, value);
        }
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
        public string Name
        {
            get => _Name;
            set => SetValue(ref _Name, value);
        }

        [Write(false)]
        public string Group
        {
            get => Name.Substring(0, 1).ToUpper();
        }
        public string ArabicName
        {
            get => _ArabicName;
            set => SetValue(ref _ArabicName, value);
        }
        public string City
        {
            get => _City;
            set => SetValue(ref _City, value);
        }
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

        public double CreditLimit
        {
            get => _CreditLimit;
            set => SetValue(ref _CreditLimit, value);
        }

        public int ReturnPeriod
        {
            get => _ReturnPeriod;
            set => SetValue(ref _ReturnPeriod, value);
        }

        [Write(false)]
        public int? CRAttachmentId { get; set; }

        [Write(false)]
        public int? VATAttachmentId { get; set; }

        [Write(false)]
        public int? AddressAttachmentId { get; set; }
    }
}