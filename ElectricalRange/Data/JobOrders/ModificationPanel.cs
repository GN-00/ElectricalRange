namespace ProjectsNow.Data.JobOrders
{
    public class ModificationPanel
    {
        public int PanelID { get; set; }
        public int JobOrderID { get; set; }
        public int PanelSN { get; set; }
        public string PanelName { get; set; }
        public decimal PanelQty { get; set; }
        public string Status { get; set; }
        public string EnclosureType { get; set; }
    }
}
