using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[ProformaInvoicesPanels]")]
    public class ProformaInvoicePanel : Base
    {
        private string _Description;
        private double _NetPrice;
        private double _UnitNetPrice;
        private double _VAT;
        private double _VATValue;
        private double _UnitVATValue;
        private double _GrossPrice;
        private double _UnitGrossPrice;
        private double _UnitOriginalPrice;

        [Key]
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int? PanelId { get; set; }
        public int SN { get; set; }
        public string Code { get; set; }
        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }
        public string ArabicDescription { get; set; }
        public string Brand { get; set; }
        public string Unit { get; set; } = "No";
        public double Qty { get; set; }
        public double NetPrice
        {
            get => _NetPrice;
            set => SetValue(ref _NetPrice, value);
        }
        public double UnitNetPrice
        {
            get => _UnitNetPrice;
            set => SetValue(ref _UnitNetPrice, value);
        }
        public double VAT
        {
            get => _VAT;
            set => SetValue(ref _VAT, value);
        }
        public double VATValue
        {
            get => _VATValue;
            set => SetValue(ref _VATValue, value);
        }
        public double UnitVATValue
        {
            get => _UnitVATValue;
            set => SetValue(ref _UnitVATValue, value);
        }
        public double GrossPrice
        {
            get => _GrossPrice;
            set => SetValue(ref _GrossPrice, value);
        }
        public double UnitGrossPrice
        {
            get => _UnitGrossPrice;
            set => SetValue(ref _UnitGrossPrice, value);
        }
        public double UnitOriginalPrice
        {
            get => _UnitOriginalPrice;
            set => SetValue(ref _UnitOriginalPrice, value);
        }
    }
}
