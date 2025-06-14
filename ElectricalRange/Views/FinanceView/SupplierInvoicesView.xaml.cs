using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class SupplierInvoicesView : UserControl, IView
    {
        public SupplierInvoicesView(SupplierAccount account, PurchaseOrder order = null)
        {
            InitializeComponent();
            DataContext = new SupplierInvoicesViewModel(account, order, this);
        }
    }
}
