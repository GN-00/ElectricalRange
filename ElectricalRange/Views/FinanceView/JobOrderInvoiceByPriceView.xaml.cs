using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class JobOrderInvoiceByPriceView : UserControl, IView
    {
        public JobOrderInvoiceByPriceView(Invoice invoice, ObservableCollection<Invoice> invoices)
        {
            InitializeComponent();
            DataContext = new JobOrderInvoiceByPriceViewModel(invoice, invoices, this);
        }
    }
}
