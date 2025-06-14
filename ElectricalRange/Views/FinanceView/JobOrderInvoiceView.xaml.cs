using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class JobOrderInvoiceView : UserControl, IView
    {
        public JobOrderInvoiceView(Invoice invoice, ObservableCollection<Invoice> invoices, CustomerAccount customer)
        {
            InitializeComponent();
            DataContext = new JobOrderInvoiceViewModel(invoice, invoices, customer, this);
        }
    }
}
