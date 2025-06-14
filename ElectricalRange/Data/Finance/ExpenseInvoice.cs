using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[ExpensesInvoices]")]
    public class ExpenseInvoice : Base
    {
        private string _Number;
        private DateTime _Date;
        private decimal _Amount;
        private decimal _Paid;

        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }
        public string Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }
        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }

        public decimal Amount
        {
            get => _Amount;
            set => SetValue(ref _Amount, value);
        }

        [Write(false)]
        public decimal Paid
        {
            get => _Paid;
            set => SetValue(ref _Paid, value)
                  .UpdateProperties(this, nameof(Balance), nameof(PaidPercentage), nameof(PaidInfo));
        }
        [Write(false)]

        public decimal Balance => Amount - Paid;

        [Write(false)]
        public string PaidPercentage
        {
            get
            {
                if (Amount != 0)
                {
                    return $"{(Paid / Amount) * 100m:N2} %";
                }
                else
                {
                    return null;
                }
            }
        }

        [Write(false)]
        public string PaidInfo
        {
            get
            {
                if (PaidPercentage != null)
                {
                    decimal value = (Paid / Amount) * 100m;
                    if (value == 100)
                    {
                        return "Complete";
                    }
                    else if (value > 100)
                    {
                        return "OverPaid";
                    }
                    else
                    {
                        return "Uncomplete";
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
