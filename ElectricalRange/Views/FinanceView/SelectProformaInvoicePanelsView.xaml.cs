using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class SelectProformaInvoicePanelsView : UserControl, IPopup
    {
        public SelectProformaInvoicePanelsView(ObservableCollection<ProformaInvoicePanel> panels, ObservableCollection<Data.JobOrders.JPanel> orderPanels)
        {
            InitializeComponent();
            DataContext = new SelectProformaInvoicePanelsViewModel(panels, orderPanels);
        }
    }
}
