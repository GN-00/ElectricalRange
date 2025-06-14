using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Store
{

    [Table("[Store].[SalesItems]")]
    public class SalesItem : Base
    {
        private string _Code;
        private string _Description;
        private string _ArabicInfo = "مواد";
        private string _Unit;
        private decimal _Qty = 1;
        private decimal _Cost;
        private decimal _VAT;
        private string _Brand;
        private string _Remarks;

        [Key]
        public int Id { get; set; }
        public int SalesInvoiceId { get; set; }

        [Write(false)]
        public int SN { get; set; }
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

        [Write(false)]
        public string ArabicInfo
        {
            get => _ArabicInfo;
            set => SetValue(ref _ArabicInfo, value);
        }

        [Write(false)]
        public string CodeInfo => $"{Code}\n{Description}";
        public string Unit
        {
            get => _Unit;
            set => SetValue(ref _Unit, value);
        }
        public decimal Qty
        {
            get => _Qty;
            set => SetValue(ref _Qty, value)
                  .UpdateProperties(this, nameof(TotalCost), nameof(NetPrice), nameof(GrossPrice));
        }
        public decimal Cost
        {
            get => _Cost;
            set => SetValue(ref _Cost, value)
                  .UpdateProperties(this, nameof(TotalCost), nameof(NetPrice), nameof(GrossPrice));
        }

        [Write(false)]
        public decimal VAT
        {
            get => _VAT;
            set => SetValue(ref _VAT, value);
        }
        public string Brand
        {
            get => _Brand;
            set => SetValue(ref _Brand, value);
        }
        public string Remarks
        {
            get => _Remarks;
            set => SetValue(ref _Remarks, value);
        }


        [Write(false)]
        public decimal TotalCost => Cost * Qty;

        [Write(false)]
        public decimal UnitNetPrice => Cost;

        [Write(false)]
        public decimal NetPrice => TotalCost;

        [Write(false)]
        public decimal VATValue => NetPrice * (VAT / 100m);

        [Write(false)]
        public decimal GrossPrice => NetPrice * (1 + VAT / 100m);
    }
}
