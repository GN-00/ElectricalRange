using Dapper.Contrib.Extensions;


namespace ProjectsNow.Data.Production
{
    [Table("[Production].[Orders]")]
    public class Order : Base, IAccess
    {
        private string _Code;
        private string _Project;
        private string _Customer;
        private string _Quotation;
        private bool _IsSiteWork;
        private bool _IsClosed;

        [Key]
        public int Id { get; set; }
        public int JobOrderId { get; set; }

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

        public string Quotation
        {
            get => _Quotation;
            set => SetValue(ref _Quotation, value);
        }


        public bool IsClosed
        {
            get => _IsClosed;
            set => SetValue(ref _IsClosed, value)
                  .UpdateProperties(this, "Type");
        }

        public bool IsSiteWork
        {
            get => _IsSiteWork;
            set => SetValue(ref _IsSiteWork, value)
                  .UpdateProperties(this, "Type");
        }

        [Write(false)]
        public string Type
        {
            get
            {
                if (IsClosed)
                    return "Closed";

                if (IsSiteWork)
                    return "Site Work";
                else
                    return "Factory";
            }
        }


        [Write(false)]
        public int Panels { get; set; }

        public DateTime? CloseDate { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}
