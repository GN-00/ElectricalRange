using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[QuotationsRequestsItems]")]
    public class QuotationRequestItem : Base
    {

        [Key]
        public int Id { get; set; }

        [Write(false)]
        public int SN { get; set; }
        public int JobOrderID { get; set; }
        public int QuotationRequestId { get; set; }

        private string _Code;
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }

        private string _Description;
        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }

        private string _Unit;
        public string Unit
        {
            get => _Unit;
            set => SetValue(ref _Unit, value);
        }

        private decimal _Qty;
        public decimal Qty
        {
            get => _Qty;
            set => SetValue(ref _Qty, value);
        }

        public int? Reference { get; set; }
        [Write(false)]
        public bool Received => Reference != null;
    }
}
