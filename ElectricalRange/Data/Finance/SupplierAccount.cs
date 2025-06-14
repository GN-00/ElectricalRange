using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Finance
{
    [Table("[Supplier].[Suppliers]")]
    public class SupplierAccount : Base, IAccess
    {
        private double _Account;
        private double _Debt;

        [Key]
        public int ID { get; set; }

        [Write(false)]
        public int Id => ID;

        [Write(false)]
        public string Name { get; set; }

        [Write(false)]
        public int PurchaseOrders { get; set; }

        [Write(false)]
        public int Invoices { get; set; }

        [Write(false)]
        public double Account
        {
            get => _Account;
            set => SetValue(ref _Account, value)
                  .UpdateProperties(this, nameof(Balance), nameof(BalanceStatus));
        }

        [Write(false)]
        public double Debt
        {
            get => _Debt;
            set => SetValue(ref _Debt, value)
                  .UpdateProperties(this, nameof(Balance), nameof(BalanceStatus));
        }

        [Write(false)]
        public double Balance => Account - Debt;

        [Write(false)]
        public int BalanceStatus
        {
            get
            {
                if (Balance > -1 && Balance < 1)
                    return 0;
                else if (Balance >= 1)
                    return 1;
                else
                    return -1;
            }
        }

        public int BankID { get; set; }

        [Write(false)]
        public string Bank { get; set; }

        [Write(false)]
        public string AccountNumber { get; set; }

        [Write(false)]
        public string IBAN { get; set; }

        [Write(false)]
        public string VATNumber { get; set; }
    }
}
