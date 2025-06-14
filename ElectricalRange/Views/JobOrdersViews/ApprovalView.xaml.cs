using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows.Controls;

using Panel = ProjectsNow.Data.JobOrders.TransactionPanel;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class ApprovalView : UserControl, IPopup
    {
        public ApprovalView(ApprovalRequest request, ObservableCollection<Panel> panels, ObservableCollection<JPanel> orderPanels)
        {
            InitializeComponent();
            DataContext = new ApprovalViewModel(request, panels, orderPanels);
        }
    }
}
