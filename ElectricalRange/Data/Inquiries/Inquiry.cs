using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Inquiries
{
    [Table("[Inquiry].[Inquiries]")]
    public class Inquiry : Base, IAccess
    {
        private int? _QuotationID;
        private string _QuotationStatus;
        private int? _JobOrderID;
        private string _RegisterCode;
        private string _ProjectName;
        private DateTime _RegisterDate = DateTime.Now;
        private DateTime _DuoDate = DateTime.Now.AddDays(7);
        private string _Priority = "Normal";
        private string _CustomerName;
        private string _EstimationName;
        private string _EstimationCode;
        private string _SalesmanName;
        private string _SalesmanCode;
        private string _ProjectStatus = "On Hand";
        private string _Classification = "Medium Term";
        private decimal? _Value;
        private string _Location;
        private string _Scope;

        [Key]
        public int InquiryID { get; set; }

        [Write(false)]
        public int Id => InquiryID;

        public int CustomerID { get; set; }
        public int ConsultantID { get; set; }
        public int EstimationID { get; set; }
        public int SalesmanID { get; set; }

        [Write(false)]
        public int? QuotationID
        {
            get => _QuotationID;
            set => SetValue(ref _QuotationID, value)
                  .UpdateProperties(this, nameof(Status));
        }

        [Write(false)]
        public string QuotationStatus
        {
            get => _QuotationStatus;
            set => SetValue(ref _QuotationStatus, value);
        }

        [Write(false)]
        public int? JobOrderID
        {
            get => _JobOrderID;
            set => SetValue(ref _JobOrderID, value)
                  .UpdateProperties(this, nameof(Status));
        }

        public string RegisterCode
        {
            get => _RegisterCode;
            set => SetValue(ref _RegisterCode, value);
        }
        public string ProjectName
        {
            get => _ProjectName;
            set => SetValue(ref _ProjectName, value);
        }
        public DateTime RegisterDate
        {
            get => _RegisterDate;
            set => SetValue(ref _RegisterDate, value)
                  .UpdateProperties(this, nameof(RegisterDateInfo));

        }
        [Write(false)]
        public string RegisterDateInfo
        {
            get => _RegisterDate.ToString("dd-MM-yyyy");
        }

        public DateTime DuoDate
        {
            get => _DuoDate;
            set => SetValue(ref _DuoDate, value)
                  .UpdateProperties(this, nameof(DuoDateInfo));
        }

        [Write(false)]
        public string DuoDateInfo
        {
            get => _DuoDate.ToString("dd-MM-yyyy");
        }
        public string Priority
        {
            get => _Priority;
            set => SetValue(ref _Priority, value);
        }
        public int RegisterNumber { get; set; }
        public int RegisterMonth { get; set; }
        public int RegisterYear { get; set; }
        public string DeliveryCondition { get; set; } = "Ex-Factory";
        [Write(false)]
        public string CustomerName
        {
            get => _CustomerName;
            set => SetValue(ref _CustomerName, value);
        }
        [Write(false)]
        public string EstimationName
        {
            get => _EstimationName;
            set => SetValue(ref _EstimationName, value);
        }
        [Write(false)]
        public string EstimationCode
        {
            get => _EstimationCode;
            set => SetValue(ref _EstimationCode, value);
        }
        [Write(false)]
        public string SalesmanName
        {
            get => _SalesmanName;
            set => SetValue(ref _SalesmanName, value);
        }
        [Write(false)]
        public string SalesmanCode
        {
            get => _SalesmanCode;
            set => SetValue(ref _SalesmanCode, value);
        }

        public string ProjectStatus
        {
            get => _ProjectStatus;
            set => SetValue(ref _ProjectStatus, value);
        }

        public string Classification
        {
            get => _Classification;
            set => SetValue(ref _Classification, value);
        }

        public decimal? Value
        {
            get => _Value;
            set => SetValue(ref _Value, value);
        }

        public string Location
        {
            get => _Location;
            set => SetValue(ref _Location, value);
        }
        public string Scope
        {
            get => _Scope;
            set => SetValue(ref _Scope, value);
        }

        [Write(false)]
        public string Status
        {
            get
            {
                if (JobOrderID != null)
                {
                    return "Order";
                }
                else if (QuotationStatus == "Submitted")
                {
                    return "Submitted";
                }
                else if (QuotationID != null)
                {
                    return "Quoting";
                }
                else
                {
                    return "New";
                }
            }
        }
    }
}