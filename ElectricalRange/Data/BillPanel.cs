using System.IO;

namespace ProjectsNow.Data
{
    public class BillPanel
    {
        public int PanelID { get; set; }
        public int QuotationID { get; set; }
        public int? PanelSN { get; set; }
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
        public decimal PanelDiscount { get; set; }
        public decimal PanelProfit { get; set; }
        public decimal PanelCost { get; set; }
        public string PanelCostInfo
        {
            get
            {
                if (PanelCost == 0)
                    return "-";
                else
                    return PanelCost.ToString("N2");

            }
        }
        public decimal PanelPrice { get; set; }
        public string PanelPriceInfo
        {
            get
            {
                if (PanelPrice == 0)
                    return "-";
                else
                    return PanelPrice.ToString("N2");

            }
        }
        public decimal PanelsPrice { get; set; }
        public string PanelsPriceInfo
        {
            get
            {
                if (PanelsPrice == 0)
                    return "-";
                else
                    return PanelsPrice.ToString("N2");

            }
        }
        //Technical
        public string EnclosureType { get; set; }
        public string EnclosureInstallation { get; set; }
        public string EnclosureLocation { get; set; }
        public decimal? EnclosureHeight { get; set; }
        public decimal? EnclosureWidth { get; set; }
        public decimal? EnclosureDepth { get; set; }
        public string EnclosureIP { get; set; }
        public string Source { get; set; }
        public string Frequency { get; set; }
        public string EarthingSystem { get; set; }
        public bool IsSpecial { get; set; }
    }
}
