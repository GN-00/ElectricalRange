using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class PayInvoiceView : UserControl, IPopup
    {
        public PayInvoiceView(CustomerAccount customer, Invoice invoice)
        {
            InitializeComponent();
            DataContext = new PayInvoiceViewModel(customer, invoice);
        }
    }
}
