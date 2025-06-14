using Dapper.Contrib.Extensions;


namespace ProjectsNow.Data.Customers
{
    [Table("[Customer].[Consultants]")]
    public class Consultant : Base
    {
        private string _ConsultantName;
        private string _Address;
        private string _Mobile;
        private string _Email;
        private string _Company;
        private string _Website;
        private string _Job;
        private string _Note;

        [Key]
        public int ConsultantID { get; set; }

        [Write(false)]
        public int Id => ConsultantID;
        public string ConsultantName
        {
            get => _ConsultantName;
            set => SetValue(ref _ConsultantName, value);
        }
        public string Address
        {
            get => _Address;
            set => SetValue(ref _Address, value);
        }
        public string Mobile
        {
            get => _Mobile;
            set => SetValue(ref _Mobile, value);
        }
        public string Email
        {
            get => _Email;
            set => SetValue(ref _Email, value);
        }
        public string Company
        {
            get => _Company;
            set => SetValue(ref _Company, value);
        }
        public string Website
        {
            get => _Website;
            set => SetValue(ref _Website, value);
        }
        public string Job
        {
            get => _Job;
            set => SetValue(ref _Job, value);
        }
        public string Note
        {
            get => _Note;
            set => SetValue(ref _Note, value);
        }
    }
}
