using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class SupplierInvoiceView : UserControl, IView
    {
        public SupplierInvoiceView(SupplierInvoice invoice)
        {
            InitializeComponent();
            DataContext = new SupplierInvoiceViewModel(invoice, (IView)this);
        }
    }
}
