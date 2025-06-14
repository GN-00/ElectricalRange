namespace ProjectsNow.Data
{
    public class CItem : Base  //Closing Item
    {
        public int JobOrderID { get; set; }
        public int PanelID { get; set; }
        public int PanelTransactionID { get; set; }

        private string _Category;
        public string Category
        {
            get => _Category;
            set => SetValue(ref _Category, value);
        }

        private string _Code;
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }

        private string _Description;
        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }

        public string Unit { get; set; }
        public decimal PanelQty { get; set; }
        public decimal ItemQty { get; set; }
        public decimal TotalQty { get; set; }

        private decimal _StockQty;
        public decimal StockQty
        {
            get => _StockQty;
            set => SetValue(ref _StockQty, value);
        }

        private decimal _UsedQty;
        public decimal UsedQty
        {
            get => _UsedQty;
            set
            {
                if (SetValue(ref _UsedQty, value))
                {
                    OnPropertyChanged(nameof(RemainingQty));
                }
            }
        }

        public decimal RemainingQty => TotalQty - UsedQty;

        private decimal _PanelToPostQty;
        public decimal PanelToPostQty
        {
            get => _PanelToPostQty;
            set
            {
                if (SetValue(ref _PanelToPostQty, value))
                {
                    OnPropertyChanged(nameof(ItemToPostQty));
                    OnPropertyChanged(nameof(IsReady));
                }
            }
        }

        public decimal? ItemToPostQty
        {
            get
            {
                if (ItemQty * PanelToPostQty != 0)
                {
                    return ItemQty * PanelToPostQty;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool? IsReady
        {
            get
            {
                if (ItemToPostQty <= StockQty)
                {
                    return true;
                }
                else if (ItemToPostQty > StockQty)
                {
                    return false;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
