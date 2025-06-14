using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.SalesInvoicesView
{
    public partial class InvoiceView : UserControl, IView
    {
        public InvoiceView(Invoice invoice, ObservableCollection<Invoice> invoices, ObservableCollection<InvoiceItem> salesItems)
        {
            InitializeComponent();
            DataContext = new InvoiceViewModel(invoice, invoices, salesItems, this);
        }
    }
}
