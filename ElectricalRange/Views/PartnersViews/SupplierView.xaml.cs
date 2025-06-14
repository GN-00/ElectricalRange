using ProjectsNow.Data.Suppliers;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.PartnersViews
{
    public partial class SupplierView : UserControl, IView
    {
        public SupplierView(Supplier supplier, ObservableCollection<Supplier> suppliers)
        {
            InitializeComponent();
            DataContext = new SupplierViewModel(supplier, suppliers, this);
        }

        private void POBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 4);
        }

        private void VAT_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 15);
        }

        private void CR_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 10);
        }

        private void CreditLimit_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }

        private void ReturnPeriod_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 3);
        }
    }
}
