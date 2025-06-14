using ProjectsNow.Data.Store;

using System.Windows;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows.PurshaseOrderItems
{
    public partial class ItemsWindow : Window
    {
        ItemsViewModel returnToSupplierViewModel;
        public ItemsWindow(SupplierInvoice invoice)
        {
            InitializeComponent();
            returnToSupplierViewModel = new ItemsViewModel(invoice);
            DataContext = returnToSupplierViewModel;
        }
    }
}
