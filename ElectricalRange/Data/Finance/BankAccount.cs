using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[BankAccounts]")]
    public class BankAccount : Base
    {
        private string _Name;
        private string _Number;
        private string _IBAN;

        [Key]
        public int ID { get; set; }
        public string Name
        {
            get => _Name;
            set => SetValue(ref _Name, value);
        }
        public string Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }
        public string IBAN
        {
            get => _IBAN;
            set => SetValue(ref _IBAN, value);
        }
    }
}
