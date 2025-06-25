using System;
using System.IO;

namespace ProjectsNow.Data.JobOrders
{
    public class ProductionPanel
    {
        public int PanelID { get; set; }
        public int PanelSN { get; set; }
        public int PanelOrder { get; set; } //like SN
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
        public int ClosedQty { get; set; }
        public int ReadyToCloseQty => PanelQty - ClosedQty;
        public DateTime DeliveryDate { get; set; }
        public int Revise { get; set; } = 1;
    }
}
