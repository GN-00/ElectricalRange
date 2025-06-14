using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[JobOrders]")]
    public class JobOrder : Base, IAccess
    {
        private string _Code;
        private string _ProjectName;
        private string _CustomerName;

        [Key]
        public int ID { get; set; }

        [Write(false)]
        public int Id => ID;

        [Write(false)]
        public int? AcknowledgmentAttachmentId { get; set; }

        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
        public int CodeNumber { get; set; }
        public int CodeMonth { get; set; }
        public int CodeYear { get; set; }
        public DateTime? Date { get; set; }
        public int QuotationID { get; set; }

        [Write(false)]
        public string QuotationCode { get; set; }

        [Write(false)]
        public int InquiryID { get; set; }

        [Write(false)]
        public string ProjectName
        {
            get => _ProjectName;
            set => SetValue(ref _ProjectName, value);
        }
        [Write(false)]
        public int CustomerID { get; set; }
        [Write(false)]
        public string CustomerName
        {
            get => _CustomerName;
            set => SetValue(ref _CustomerName, value);
        }
        [Write(false)]
        public int EstimationID { get; set; }

        [Write(false)]
        public string EstimationName { get; set; }

        [Write(false)]
        public decimal VAT { get; set; }

        public override string ToString()
        {
            return Code;
        }

        [Write(false)]
        public int Invoices { get; set; }

        [Write(false)]
        public int Deliveries { get; set; }

        [Write(false)]
        public DateTime? CanceledDate { get; set; }

        [Write(false)]
        public bool Canceled => CanceledDate != null;

    }
}
