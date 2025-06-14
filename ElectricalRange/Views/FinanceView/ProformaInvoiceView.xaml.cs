using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class ProformaInvoiceView : UserControl, IView
    {
        public ProformaInvoiceView(ProformaInvoice invoice, ObservableCollection<ProformaInvoice> invoices)
        {
            InitializeComponent();
            DataContext = new ProformaInvoiceViewModel(invoice, invoices, this);
        }
    }
}
