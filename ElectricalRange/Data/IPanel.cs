using ProjectsNow.Data.Finance;

namespace ProjectsNow.Data
{
    public class IPanel 
    {
        public int PanelID { get; set; }
        public string PurchaseOrdersNumber { get; set; }
        public int PanelSN { get; set; }
        public string PanelName { get; set; }
        public int InvoicedQty { get; set; }
        public string PanelType { get; set; }
        public string PanelTypeArabic { get; set; }
        public string PanelTypeArabicInfo
        {
            get
            {
                return PanelTypeArabic ?? DataInput.Panel.ArabicType(PanelType);
            }
        }
        public decimal VAT { get; set; }
        public decimal PanelEstimatedPrice { get; set; }
        public decimal PanelsEstimatedPrice { get; set; }
        public decimal PanelVATValue { get; set; }
        public decimal PanelsVATValue { get; set; }
        public decimal PanelFinalPrice { get; set; }
        public decimal PanelsFinalPrice { get; set; }

        public decimal OriginalPrice { get; set; }
        public decimal SpecialPrice { get; set; }
        public string SpecialName { get; set; }
        public string SpecialArabicType { get; set; }
        public string Note { get; set; }

        public string PanelNameInfo
        {
            get
            {
                if (Note != null)
                {
                    return $"{PanelName}\n({Note})";
                }
                else
                {
                    return PanelName;
                }
            }
        }

    }
}
