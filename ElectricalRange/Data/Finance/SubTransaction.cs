using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{

    [Table("[Finance].[SubTransactions]")]
    public class SubTransaction : TransactionBase //Level 2
    {
        private DateTime _Date = DateTime.Now;
        private string _Description;
        private double _Amount;
        private bool _Post;

        [Key]
        public int Id { get; set; }
        public int OldId { get; set; }

        #region Ids
        public int? JobOrderId { get; set; }
        public int? JobOrderInvoiceId { get; set; }
        public int? PurchaseOrderId { get; set; }
        public int? PurchaseOrderInvoiceId { get; set; }
        public int? ReturnInvoiceId { get; set; }
        #endregion

        [Write(false)]
        public string JobOrderCode { get; set; }

        [Write(false)]
        public string PurchaseOrderCode { get; set; }

        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value).UpdateProperties(this, nameof(DateInfo));
        }

        [Write(false)]
        public string DateInfo
        {
            get => Date.ToString("dd-MM-yyyy");
        }

        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }
        public double Amount
        {
            get => _Amount;
            set => SetValue(ref _Amount, value);
        }

        public string Type { get; set; }

        public bool Post
        {
            get => _Post;
            set => SetValue(ref _Post, value);
        }
    }
}
