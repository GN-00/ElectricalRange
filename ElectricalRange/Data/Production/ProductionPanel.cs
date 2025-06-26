namespace ProjectsNow.Data.Production
{
    public class ProductionPanel
    {
        public int PanelId { get; set; } 
        public int OrderId { get; set; }
        public int Reference { get; set; }
        public int SN { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public int ClosedQty { get; set; }
        public DateTime Date { get; set; }
        public bool InProduction { get; set; }
        public int ReadyToCloseQty => Qty - ClosedQty;
    }
}
