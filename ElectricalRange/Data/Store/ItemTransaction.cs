using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Store
{
    [Table("[Store].[Transactions]")]
    public class ItemTransaction : Base
    {
        private string _Code;
        private string _Description;
        private string _Unit;
        private decimal _Qty;
        private decimal _UsedQty;
        private decimal _DamagedQty;
        private decimal _TransferredQty;
        private decimal _ReturnedQty;
        private decimal _Cost;
        private decimal _VAT;

        [Key]
        public int ID { get; set; }
        public int? SalesItemId { get; set; }
        public int JobOrderID { get; set; }
        public int? PanelID { get; set; }
        public int? PanelTransactionID { get; set; }
        public int InvoiceID { get; set; }
        public int? TransferInvoiceID { get; set; }
        public int? OriginalInvoiceID { get; set; }
        public int? ReturnInvoiceID { get; set; }
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }
        public int? Reference { get; set; }
        public string Unit
        {
            get => _Unit;
            set => SetValue(ref _Unit, value);
        }
        public decimal Qty
        {
            get => _Qty;
            set => SetValue(ref _Qty, value)
                  .UpdateProperties(this, nameof(FinalQty), nameof(TotalCost));
        }
        [Write(false)]
        public decimal UsedQty
        {
            get => _UsedQty;
            set => SetValue(ref _UsedQty, value)
                  .UpdateProperties(this, nameof(FinalQty), nameof(TotalCost));
        }
        [Write(false)]
        public decimal DamagedQty
        {
            get => _DamagedQty;
            set => SetValue(ref _DamagedQty, value)
                  .UpdateProperties(this, nameof(FinalQty), nameof(TotalCost));
        }
        [Write(false)]
        public decimal TransferredQty
        {
            get => _TransferredQty;
            set => SetValue(ref _TransferredQty, value)
                  .UpdateProperties(this, nameof(FinalQty), nameof(TotalCost));
        }
        [Write(false)]
        public decimal ReturnedQty
        {
            get => _ReturnedQty;
            set => SetValue(ref _ReturnedQty, value)
                  .UpdateProperties(this, nameof(FinalQty), nameof(TotalCost));
        }
        [Write(false)]
        public decimal FinalQty => Qty - UsedQty - DamagedQty - TransferredQty - ReturnedQty;
        public decimal Cost
        {
            get => _Cost;
            set => SetValue(ref _Cost, value)
                  .UpdateProperties(this, nameof(TotalCost));
        }
        public decimal VAT
        {
            get => _VAT;
            set => SetValue(ref _VAT, value);
        }
        [Write(false)]
        public decimal TotalCost => Cost * Qty;
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }

        [Write(false)]
        public int SN { get; set; }
    }
}
