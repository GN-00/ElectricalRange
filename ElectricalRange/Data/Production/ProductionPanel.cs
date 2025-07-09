namespace ProjectsNow.Data.Production
{
    public class ProductionPanel : Base
    {
        public int JobOrderId { get; set; }
        public int PanelId { get; set; }
        public int OrderId { get; set; }
        public int Reference { get; set; }
        public int SN { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public int ClosedQty { get; set; }
        public int DeliveredQty { get; set; }
        public DateTime Date { get; set; }
        public bool InProduction { get; set; }
        public int ReadyToCloseQty => Qty - ClosedQty;
        public int ReadyToDeliverQty => ClosedQty - DeliveredQty;

        private string _EnclosureName;
        public string EnclosureName
        {
            get => _EnclosureName;
            set => SetValue(ref _EnclosureName, value);
        }

        private string _EnclosureType;
        public string EnclosureType
        {
            get => _EnclosureType;
            set => SetValue(ref _EnclosureType, value);
        }

        private string _EnclosureInstallation;
        public string EnclosureInstallation
        {
            get => _EnclosureInstallation;
            set => SetValue(ref _EnclosureInstallation, value);
        }

        private decimal? _EnclosureHeight;
        public decimal? EnclosureHeight
        {
            get => _EnclosureHeight;
            set => SetValue(ref _EnclosureHeight, value);
        }

        private decimal? _EnclosureWidth;
        public decimal? EnclosureWidth
        {
            get => _EnclosureWidth;
            set => SetValue(ref _EnclosureWidth, value);
        }

        private decimal? _EnclosureDepth;
        public decimal? EnclosureDepth
        {
            get => _EnclosureDepth;
            set => SetValue(ref _EnclosureDepth, value);
        }

        private string _EnclosureIP;

        public string EnclosureIP
        {
            get => _EnclosureIP;
            set => SetValue(ref _EnclosureIP, value);
        }

        private double _Items;
        public double Items
        {
            get => _Items;
            set => SetValue(ref _Items, value)
                  .UpdateProperties(this, "PercentageItems");

        }

        private double _ReceivedItems;
        public double ReceivedItems
        {
            get => _ReceivedItems;
            set => SetValue(ref _ReceivedItems, value)
                  .UpdateProperties(this, "PercentageItems");
        }

        public double PercentageItems => (ReceivedItems / Items) * 100;

        public double MissingItems { get; set; }
        public List<Item> ItemsList { get; set; } = [];
    }
}
