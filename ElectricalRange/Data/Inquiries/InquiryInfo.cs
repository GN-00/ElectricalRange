using System;

namespace ProjectsNow.Data.Inquiries
{
    public class InquiryInfo : Base
    {
        public string RegisterCode { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public DateTime RegisterDate { get; set; }
        public string RegisterDateInfo
        {
            get => RegisterDate.ToString("dd-MM-yyyy");
        }
        public DateTime DuoDate { get; set; }
        public string DuoDateInfo
        {
            get => DuoDate.ToString("dd-MM-yyyy");
        }
        public string Salesman { get; set; } = "-";
        public string Estimator { get; set; } = "-";
        public string Scope { get; set; } = "-";
        public string ProjectStatus { get; set; } = "-";
        public string Contact { get; set; } = "-";
        public string ContactNumber { get; set; } = "-";
        public string QuotationCode { get; set; } = "-";
        public string QuotationGrossPrice { get; set; } = "-";
        public string JobOrderCode { get; set; } = "-";
        public string PurchaseOrder { get; set; } = "-";
        public string TotalPanels { get; set; } = "-";
    }
}
