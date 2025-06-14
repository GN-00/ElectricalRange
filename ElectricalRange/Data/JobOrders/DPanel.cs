using System.IO;

namespace ProjectsNow.Data.JobOrders
{
    public class DPanel
    {
        public string DeliveryNumber { get; set; }
        public int PanelID { get; set; }
        public int PanelSN { get; set; }
        public string PanelName { get; set; }
        public string PanelNameInfo
        {
            get
            {
                using var reader = new StringReader(PanelName);
                return reader.ReadLine();
            }
        }
        public int PanelQty { get; set; }
        public int PreviousQty { get; set; }
        public int DeliveredQty { get; set; }
        public int Outstanding => PanelQty - DeliveredQty - PreviousQty;
    }
}
