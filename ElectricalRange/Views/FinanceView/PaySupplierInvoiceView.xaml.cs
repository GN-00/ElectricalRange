using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class PaySupplierInvoiceView : UserControl, IPopup
    {
        public PaySupplierInvoiceView(SupplierAccount supplier, SupplierInvoice invoice)
        {
            InitializeComponent();
            DataContext = new PaySupplierInvoiceViewModel(supplier, invoice);
        }
    }
}
