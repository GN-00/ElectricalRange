namespace ProjectsNow.Data
{
    public class ItemPurchased : Base  //Job Orders Items (PurchaseDetails)
    {
        public int JobOrderID { get; set; }

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

        private string _Unit;
        public string Unit
        {
            get => _Unit;
            set => SetValue(ref _Unit, value);
        }

        private decimal _Qty;
        public decimal Qty
        {
            get => _Qty;
            set => SetValue(ref _Qty, value)
                  .UpdateProperties(this, nameof(RemainingQty));
        }

        private decimal _PurchasedQty;
        public decimal PurchasedQty
        {
            get => _PurchasedQty;
            set => SetValue(ref _PurchasedQty, value)
                  .UpdateProperties(this, nameof(RemainingQty));
        }

        private decimal _DamagedQty;
        public decimal DamagedQty
        {
            get => _DamagedQty;
            set => SetValue(ref _DamagedQty, value)
                  .UpdateProperties(this, nameof(RemainingQty));
        }

        private decimal _InOrderQty;
        public decimal InOrderQty
        {
            get => _InOrderQty;
            set => SetValue(ref _InOrderQty, value)
                  .UpdateProperties(this, nameof(RemainingQty));
        }

        public decimal ListPrice { get; set; }
        public decimal Estimated { get; set; }

        public decimal RemainingQty => Qty - PurchasedQty + DamagedQty - InOrderQty;

        public string Status
        {
            get
            {
                if (PurchasedQty + InOrderQty - DamagedQty > Qty)
                {
                    return "Over Qty";
                }
                if (PurchasedQty + InOrderQty - DamagedQty == Qty)
                {
                    return "Complete";
                }
                else
                {
                    return "Reemaining";
                }
            }
        }
    }
}
