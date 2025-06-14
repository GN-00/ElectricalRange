using ProjectsNow.Windows.JobOrderWindows.LookingForQuotations;

namespace ProjectsNow.Data.Finance
{
    public class PanelPrices: Base
    {
        private double _Invoiced;
        private double _InvoicingQty;

        public int PanelId { get; set; }
        public int SN { get; set; }
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public int Qty { get; set; }
        public double InvoicingQty
        {
            get => _InvoicingQty;
            set => SetValue(ref _InvoicingQty, value)
                  .UpdateProperties(this, nameof(Balance));
        }

        public double NetPrice { get; set; }
        public double UnitNetPrice { get; set; }
        public double VAT { get; set; }
        public double VATValue { get; set; }
        public double UnitVATValue { get; set; }
        public double GrossPrice { get; set; }
        public double UnitGrossPrice { get; set; }
        public double UnitOriginalPrice { get; set; }

        public double Invoiced 
        {
            get => _Invoiced;
            set => SetValue(ref _Invoiced, value)
                  .UpdateProperties(this, nameof(Balance));
        }
        public double Balance => GrossPrice - Invoiced;

        
    }
}
