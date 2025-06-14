using ProjectsNow.Data.Suppliers;

using System.Windows.Controls;

namespace ProjectsNow.Views.PartnersViews
{
    public partial class SupplierContactsView : UserControl, IView
    {
        public SupplierContactsView(Supplier supplier)
        {
            InitializeComponent();
            DataContext = new SupplierContactsViewModel(supplier);
        }
    }
}
