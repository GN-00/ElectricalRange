namespace ProjectsNow.Data
{
    public class Category : Base
    {
        private string _Name;
        public string Name
        {
            get => _Name;
            set => SetValue(ref _Name, value);
        }

        private decimal _Discount;
        public decimal Discount
        {
            get => _Discount;
            set => SetValue(ref _Discount, value);
        }

        private decimal _OldDiscount;
        public decimal OldDiscount
        {
            get => _OldDiscount;
            set => SetValue(ref _OldDiscount, value);
        }

        private int _TotalItems;
        public int TotalItems
        {
            get => _TotalItems;
            set => SetValue(ref _TotalItems, value);
        }

        private bool _IsEnabled = false;
        public bool IsEnabled
        {
            get => _IsEnabled;
            set => SetValue(ref _IsEnabled, value);
        }
    }
}
