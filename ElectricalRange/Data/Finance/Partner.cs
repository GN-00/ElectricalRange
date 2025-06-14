namespace ProjectsNow.Data.Finance
{
    public class Partner : Base
    {
        private double _Account;
        private double _Amount;

        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public int? OtherId { get; set; }

        public string Name { get; set; }

        public double Account
        {
            get => _Account;
            set => SetValue(ref _Account, value)
                  .UpdateProperties(this, nameof(Balance), nameof(AccountInfo));
        }

        public string AccountInfo
        {
            get
            {
                if (Account > 0)
                {
                    return $"{Account:N2} Receipt";
                }
                else if (Account < 0)
                {
                    return $"{Account:N2} Paid";
                }
                else
                {
                    return "0";
                }
            }
        }

        public double Amount
        {
            get => _Amount;
            set => SetValue(ref _Amount, value)
                  .UpdateProperties(this, nameof(Balance), nameof(AmountInfo));
        }

        public string AmountInfo
        {
            get
            {
                if (Amount > 0)
                {
                    return $"{Amount:N2} Debtor";
                }
                else if (Amount < 0)
                {
                    return $"{Amount:N2} Creditor";
                }
                else
                {
                    return "0";
                }
            }
        }

        public string Balance
        {
            get
            {
                if (Account - Amount < 0)
                {
                    return $"{-1 * (Account - Amount):N2} Debtor";
                }
                else if (Account - Amount > 0)
                {
                    return $"{(Account - Amount):N2} Creditor";
                }
                else
                {
                    return "0";
                }
            }
        }

        public int BalanceInfo
        {
            get
            {
                if (Account - Amount < 0)
                {
                    return -1;
                }
                else if (Account - Amount > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string VATNumber { get; set; }
    }
}
