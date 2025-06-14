using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class SupplierPurchaseOrdersView : UserControl, IView
    {
        public SupplierPurchaseOrdersView(SupplierAccount supplier = null)
        {
            InitializeComponent();
            DataContext = new SupplierPurchaseOrdersViewModel(supplier, this);
        }
    }
}
