using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Suppliers
{
    [Table("[Supplier].[Contacts]")]
    public class Contact : Base //[Suppliers Contacts]
    {
        private string _Name;
        private string _Mobile;
        private string _Address;
        private string _Email;
        private string _Job;
        private string _Note;
        private bool _Attention;
        private string _CustomerName;

        [Key]
        public int ID { get; set; }
        public int SupplierID { get; set; }
        public string Name
        {
            get => _Name;
            set => SetValue(ref _Name, value);
        }
        [Write(false)]
        public string Group
        {
            get => Name.Substring(0, 1).ToUpper();
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
