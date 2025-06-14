using System;

namespace ProjectsNow.Data.Finance
{
    public class Statement : Base
    {
        private double? _Debt;
        private double? _Credit;
        private double _Balance;
        public int SN { get; set; }
        public DateTime Date { get; set; }
        public string DateInfo => Date.ToString("dd-MM-yyyy");
        public string Description { get; set; }
        public double? Debt
        {
            get => _Debt;
            set
            {
                if (SetValue(ref _Debt, value))
                {
                    OnPropertyChanged(nameof(DebtView));
                    OnPropertyChanged(nameof(Balance));
                }
            }
        }
        public double? Credit
        {
            get => _Credit;
            set
            {
                if (SetValue(ref _Credit, value))
                {
                    OnPropertyChanged(nameof(CreditView));
                    OnPropertyChanged(nameof(Balance));
                }
            }
        }
        public double Balance
        {
            get => _Balance;
            set
            {
                if (SetValue(ref _Balance, value))
                {
                    OnPropertyChanged(nameof(BalanceView));
                }
            }
        }
        public string Sign
        {
            get
            {
                if (Balance > 0)
                {
                    return "Cr";
                }
                else if (Balance < 0)
                {
                    return "Dr";
                }
                else
                {
                    return null;
                }
            }
        }

        public string DebtView => Debt.GetValueOrDefault() == 0 ? null : Debt.ToString();
        public string CreditView => Credit.GetValueOrDefault() == 0 ? null : Credit.ToString();
        public string BalanceView => $"{Math.Abs(Balance):N2} {Sign}";

        public int BalanceStatus
        {
            get
            {
                if (Balance > 1)
                    return 0;
                else
                    return 1;
            }
        } 
    }
}
