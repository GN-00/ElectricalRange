using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.QuotationsStatus
{
    [Table("[Inquiry].[Inquiries]")]
    public class Inquiry : Base
    {
        private string _ProjectStatus;

        [Key]
        public int InquiryID { get; set; }

        [Write(false)]
        public int EstimationID { get; set; }

        [Write(false)]
        public int SalesmanID { get; set; }
        public string ProjectStatus
        {
            get => _ProjectStatus;

            set => SetValue(ref _ProjectStatus, value);
        }

        [Write(false)]
        public string CurrentStatus { get; set; }

        [Write(false)]
        public DateTime Date { get; set; }
        [Write(false)]
        public string Project { get; set; }
        [Write(false)]
        public string Customer { get; set; }
        [Write(false)]
        public string Contact { get; set; }
        [Write(false)]
        public string Mobile { get; set; }
        public string Classification { get; set; }

        [Write(false)]
        public decimal Value { get; set; }

        [Write(false)]
        public DateTime Dated { get; set; }
        [Write(false)]
        public string LastVisit { get; set; }

        [Write(false)]
        public string Consultant { get; set; }

        [Write(false)]
        public int RegisterYear { get; set; }

        [Write(false)]
        public int RegisterNumber { get; set; }

        [Write(false)]
        public string IsOrder => null;
    }
    public class InquiryYear
    {
        public int Year { get; set; }
    }
}
