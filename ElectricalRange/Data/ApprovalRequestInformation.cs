using System;

namespace ProjectsNow.Data
{
    public class ApprovalRequestInformation
    {
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public string Attention { get; set; }
        public string POs { get; set; }
        public string JobOrderCode { get; set; }
        public int DrawingsNo { get; set; }
        public string ProjectName { get; set; }
    }
}
