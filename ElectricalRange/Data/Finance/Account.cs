using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[CompanyAccounts]")]
    public class Account : Base ,IAccess
    {
        private string _Name;
        private string _Type;
        private DateTime? _CreateDate;
        private string _Bank;
        private string _AccountNumber;
        private string _IBAN;

        [Key]
        public int ID { get; set; }
        public int BankID { get; set; }
        public string Name
        {
            get => _Name;
            set => SetValue(ref _Name, value);
        }
        public string Type
        {
            get => _Type;
            set => SetValue(ref _Type, value);
        }
        public DateTime? CreateDate
        {
            get => _CreateDate;
            set => SetValue(ref _CreateDate, value);
        }

        [Write(false)]
        public decimal Balance { get; set; }

        [Write(false)]
        public string BalanceStatus
        { 
            get
            {
                if (Balance == 0)
                {
                    return "0";
                }
                else if (Balance > 0)
                {
                    return "1";
                }
                else
                {
                    return "-1";
                }
            }
            
        }

        [Write(false)]
        public string Bank
        {
            get => _Bank;
            set => SetValue(ref _Bank, value);
        }


        [Write(false)]
        public string AccountNumber
        {
            get => _AccountNumber;
            set => SetValue(ref _AccountNumber, value);
        }


        [Write(false)]
        public string IBAN
        {
            get => _IBAN;
            set => SetValue(ref _IBAN, value);
        }
    }
}
