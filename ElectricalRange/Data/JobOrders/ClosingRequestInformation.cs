using System;

namespace ProjectsNow.Data.JobOrders
{
    public class ClosingRequestInformation
    {
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string ProjectName { get; set; }
        public string JobOrderCode { get; set; }
        public string RequestCode { get; set; }
    }
}
