using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.QuotationsStatus
{
    [Table("[Quotation].[Quotations]")]
    public class Quotation : Base
    {
        private string _CurrentStatus;
        private string _ProjectStatus;

        [Key]
        public int QuotationID { get; set; }

        [Write(false)]
        public string QuotationCode { get; set; }

        [Write(false)]
        public int? InquiryID { get; set; }

        [Write(false)]
        public int? JobOrderID { get; set; }

        [Write(false)]
        public DateTime DateIssued { get; set; }

        [Write(false)]
        public string ProjectStatus
        {
            get => _ProjectStatus;

            set => SetValue(ref _ProjectStatus, value);
        }

        [Write(false)]
        public string Project { get; set; }

        [Write(false)]
        public string Location { get; set; }

        [Write(false)]
        public string Customer { get; set; }

        [Write(false)]
        public string Contact { get; set; }

        [Write(false)]
        public string Mobile { get; set; }

        [Write(false)]
        public string Scope { get; set; }

        [Write(false)]
        public string CurrentStatus 
        {
            get => _CurrentStatus;

            set => SetValue(ref _CurrentStatus, value)
                  .UpdateProperties(this, nameof(StatusGroup));
        }

        [Write(false)]
        public string StatusGroup 
        { 
            get
            {
                if (CurrentStatus == "Win")
                    return "1";
                else if (CurrentStatus == "Hold")
                    return "2";
                else if (CurrentStatus == "Cancel")
                    return "3";
                else if (CurrentStatus == "Lost")
                    return "4";
                else if (CurrentStatus == "On Going")
                    return "5";
                else
                    return "5";
            }
        }

        [Write(false)]
        public decimal NetPrice { get; set; }

        [Write(false)]
        public int QuotationNumber { get; set; }

        [Write(false)]
        public int QuotationYear { get; set; }

        [Write(false)]
        public int EstimationID { get; set; }

        [Write(false)]
        public string EstimationName { get; set; }

        [Write(false)]
        public int SalesmanID { get; set; }

        [Write(false)]
        public string Salesman { get; set; }

        [Write(false)]
        public string ConsultantName { get; set; }

        [Write(false)]
        public string Note { get; set; }

        [Write(false)]
        public string IsOrder => JobOrderID.HasValue? "Order" : null;

        [Write(false)]
        public double Total { get; set; }

        [Write(false)]
        public double QuotationEstimatedPrice { get; set; }

        [Write(false)]
        public double QuotationFinalPrice { get; set; }
    }

    public class QuotationsYear
    {
        public int Year { get; set; }
    }
}
