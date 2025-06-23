namespace ProjectsNow.Data.Production
{
    public class OrderRequest : Base
    {
        private string _Number;
        public string Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }
        public DateTime? Date { get; set; }
    }
}
