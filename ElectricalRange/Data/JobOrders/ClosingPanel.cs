using System.IO;

namespace ProjectsNow.Data.JobOrders
{
    public class ClosingPanel
    {
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
    }
}
