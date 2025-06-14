using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.SalesInvoicesView
{
    public partial class ItemView : UserControl, IPopup
    {
        public ItemView(InvoiceItem item, ObservableCollection<InvoiceItem> items)
        {
            InitializeComponent();
            DataContext = new ItemViewModel(item, items);
        }

        private void Qty_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }

        private void Cost_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
