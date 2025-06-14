using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Customers
{
    [Table("[Customer].[Contacts]")]
    public class Contact : Base
    {
        private string _ContactName;
        private string _Address;
        private string _Mobile;
        private string _Email;
        private string _Job;
        private string _Note;
        private bool _Attention;
        private string _CustomerName;

        [Key]
        public int ContactID { get; set; }
        public int CustomerID { get; set; }
        public string ContactName
        {
            get => _ContactName;
            set => SetValue(ref _ContactName, value);
        }
        [Write(false)]
        public string Group
        {
            get => ContactName.Substring(0, 1).ToUpper();
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
        [Write(false)]
        public bool Attention
        {
            get => _Attention;
            set => SetValue(ref _Attention, value);
        }
        [Write(false)]
        public string CustomerName
        {
            get => _CustomerName;
            set => SetValue(ref _CustomerName, value);
        }
    }
}
