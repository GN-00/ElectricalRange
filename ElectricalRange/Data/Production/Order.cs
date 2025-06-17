using Dapper.Contrib.Extensions;


namespace ProjectsNow.Data.Production
{
    [Table("[Production].[Orders]")]
    internal class Order : Base, IAccess
    {
        private string _Code;
        private string _Project;
        private string _Customer;

        [Key]
        public int Id { get; set; }
        public int ProjectId { get; set; }

        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }

        public int CodeNumber { get; set; }

        [Write(false)]
        public int Number => CodeNumber;

        public int CodeMonth { get; set; }

        [Write(false)]
        public int Month => CodeMonth;

        public int CodeYear { get; set; }

        [Write(false)]
        public int Year => CodeYear;

        public DateTime? Date { get; set; }

        public string Project
        {
            get => _Project;
            set => SetValue(ref _Project, value);
        }

        public int CustomerId { get; set; }

        public string Customer
        {
            get => _Customer;
            set => SetValue(ref _Customer, value);
        }

        public bool IsClosed { get; set; }
        public DateTime? ClosedDate { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}
