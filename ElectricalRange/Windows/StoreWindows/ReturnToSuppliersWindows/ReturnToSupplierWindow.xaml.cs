using ProjectsNow.Data.Store;
using ProjectsNow.Printing;

using System.Windows;

namespace ProjectsNow.Windows.StoreWindows.ReturnToSuppliersWindows
{
    public partial class ReturnToSupplierWindow : Window
    {
        ReturnToSupplierViewModel returnToSupplierViewModel;
        public ReturnToSupplierWindow(SupplierInvoice invoice)
        {
            InitializeComponent();
            returnToSupplierViewModel = new ReturnToSupplierViewModel(invoice);
            DataContext = returnToSupplierViewModel;
        }

        private void Save_Clicked(object sender, RoutedEventArgs e)
        {
            returnToSupplierViewModel.SaveCommand.Execute();
            Close();

            PrintingHelper.PrintReturnInvoice(returnToSupplierViewModel.ReturenInvoiceId);
        }

        private void Cancel_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
