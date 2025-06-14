using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[ExpensesCompanies]")]
    public class ExpenseCompany : Base
    {
        private string _Name;
        private string _Address;
        private long? _VATNumber;

        [Key]
        public int Id { get; set; }
        public string Name
        {
            get => _Name;
            set => SetValue(ref _Name, value);
        }
        public string Address
        {
            get => _Address;
            set => SetValue(ref _Address, value);
        }
        public long? VATNumber
        {
            get => _VATNumber;
            set => SetValue(ref _VATNumber, value);
        }
    }
}
