namespace ProjectsNow.Data.Production
{
    public class CloseRequest : Base
    {
        public int JobOrderId { get; set; }

        private int _Number;
        public int Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }
        public DateTime? Date { get; set; }
    }
}
