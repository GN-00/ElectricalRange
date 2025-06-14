using System;
using System.Windows;

namespace ProjectsNow.Data.Finance
{
    public class ProformaInvoiceInformation
    {
        public long CompanyVAT => Database.CompanyVAT;
        public string InvoiceNumber { get; set; }
        public  string InvoiceNumberInfo => $"ER-{InvoiceNumber}P";
        public DateTime Date { get; set; }
        public string JobOrderCode { get; set; }
        public string PurchaseOrdersNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNameArabic { get; set; }
        public string CustomerNameInfo
        {
            get
            {
                if (CustomerNameArabic != null)
                {
                    return $"{CustomerName.TrimEnd()} ({CustomerNameArabic.TrimEnd()})";
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
        public decimal DownPaymentAmount { get; set; }
        public bool InAdvanceToggle { get; set; }
        public Visibility DownPaymentVisibilty => InAdvanceToggle ? Visibility.Collapsed : Visibility.Visible;
        public Visibility AdvancePaymentVisibilty => InAdvanceToggle ? Visibility.Visible : Visibility.Collapsed;
        public decimal DownPaymentPercentage { get; set; }
        public string DownPaymentPercentageArabic
        {
            get => DataInput.Input.ToArabicNumbers(DownPaymentPercentage.ToString());
        }
    }
}
