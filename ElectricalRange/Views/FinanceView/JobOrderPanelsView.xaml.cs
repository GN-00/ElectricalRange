using ProjectsNow.Data.Finance;
using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class JobOrderPanelsView : UserControl, IPopup
    {
        public JobOrderPanelsView(ObservableCollection<InvoiceItem> transactions, ObservableCollection<JPanel> panels)
        {
            InitializeComponent();
            DataContext = new JobOrderPanelsViewModel(transactions, panels);
        }
    }
}
