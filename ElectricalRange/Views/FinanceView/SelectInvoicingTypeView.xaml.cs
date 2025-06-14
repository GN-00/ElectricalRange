using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class SelectInvoicingTypeView : UserControl, IPopup
    {
        public SelectInvoicingTypeView(Invoice invoice, ObservableCollection<Invoice> invoices, CustomerAccount customer, IView checkPoint)
        {
            InitializeComponent();
            DataContext = new SelectInvoicingTypeViewModel(invoice, invoices, customer, checkPoint);
        }
    }
}
