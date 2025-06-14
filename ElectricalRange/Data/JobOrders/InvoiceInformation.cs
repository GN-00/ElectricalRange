using System;
using System.Collections.Generic;

namespace ProjectsNow.Data.JobOrders
{
    public class InvoiceInformation
    {
        public long CompanyVAT => Database.CompanyVAT;
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string JobOrderCode { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNameArabic { get; set; }
        public string CustomerNameInfo
        {
            get
            {
                if (CustomerNameArabic != null)
                {
                    return $"{CustomerName} ({CustomerNameArabic})";
                }
                else
                {
                    return CustomerName;
                }
            }
        }
        public string Attention { get; set; }
        public string ProjectName { get; set; }
        public string Address { get; set; }
        public string CustomerVAT { get; set; }
    }
}
