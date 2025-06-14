namespace ProjectsNow.Data
{
    public class ItemStock : Base
    {
        private string _Code;
        private string _Description;
        private string _Unit;
        private decimal _Qty;
        private decimal _AvgCost;
        private string _Brand;

        public int JobOrderID { get; set; }
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }
        public string Unit
        {
            get => _Unit;
            set => SetValue(ref _Unit, value);
        }
        public decimal Qty
        {
            get => _Qty;
            set => SetValue(ref _Qty, value);
        }
        public decimal AvgCost
        {
            get => _AvgCost;
            set => SetValue(ref _AvgCost, value);
        }
        public decimal TotalAvgCost => Qty * AvgCost;
        public string Brand
        {
            get => _Brand;
            set => SetValue(ref _Brand, value);
        }
    }
}
