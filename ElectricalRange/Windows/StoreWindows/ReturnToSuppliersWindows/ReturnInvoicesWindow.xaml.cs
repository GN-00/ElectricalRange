using ProjectsNow.Data.Store;

using System.Windows;

namespace ProjectsNow.Windows.StoreWindows.ReturnToSuppliersWindows
{
    public partial class ReturnInvoicesWindow : Window
    {

        public ReturnInvoicesWindow(SupplierInvoice invoice)
        {
            InitializeComponent();
            DataContext = new ReturnInvoicesViewModel(invoice);
        }
    }
}
