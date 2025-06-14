using System;
using System.IO;

namespace ProjectsNow.Data
{
    public class APanel
    {
        public string DeliveryNumber { get; set; }
        public int PanelID { get; set; }
        public int PanelSN { get; set; }
        public string PurchaseOrdersNumber { get; set; }
        public string PanelName { get; set; }
        public string PanelNameInfo
        {
            get
            {
                using var reader = new StringReader(PanelName);
                return reader.ReadLine();
            }
        }

        public DateTime? DateOfCreation { get; set; }
        public int PanelQty { get; set; }
        public string PanelType { get; set; }
        public string DrawingManualCode { get; set; }
        public int? DrawingNo { get; set; }
        public int? Revision { get; set; }
        public string DrawingCode
        {
            get
            {
                if (DrawingManualCode != null)
                {
                    return DrawingManualCode;
                }
                else if (DrawingNo != null && DateOfCreation != null)
                {
                    if (PanelType == "Power Panel" || PanelType == "Ready Made Panel" || PanelType == "ECB" || PanelType == "Fuse Panel" || PanelType == "Junction Box")
                    {
                        return $"DP{((DateTime)DateOfCreation).Month:00}-{DrawingNo:000}/{((DateTime)DateOfCreation).Year.ToString().Substring(2, 2)}";
                    }
                    else if (PanelType == "Control Panel" || PanelType == "MCC" || PanelType == "PFC")
                    {
                        return $"CP{((DateTime)DateOfCreation).Month:00}-{DrawingNo:000}/{((DateTime)DateOfCreation).ToString().Substring(2, 2)}";
                    }
                    else if (PanelType == "ATS")
                    {
                        return $"ATS{((DateTime)DateOfCreation).Month:00}-{DrawingNo:000}/{((DateTime)DateOfCreation).ToString().Substring(2, 2)}";
                    }
                    else if (PanelType == "MTS")
                    {
                        return $"MTS{((DateTime)DateOfCreation).Month:00}-{DrawingNo:000}/{((DateTime)DateOfCreation).ToString().Substring(2, 2)}";
                    }
                    else if (PanelType == "Synchronizing Panel")
                    {
                        return $"SYN{((DateTime)DateOfCreation).Month:00}-{DrawingNo:000}/{((DateTime)DateOfCreation).ToString().Substring(2, 2)}";
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
