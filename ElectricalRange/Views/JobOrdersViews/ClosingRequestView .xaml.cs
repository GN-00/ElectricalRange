using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows.Controls;

using Panel = ProjectsNow.Data.JobOrders.TransactionPanel;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class ClosingRequestView : UserControl, IPopup
    {
        public ClosingRequestView(ClosingRequest request, ObservableCollection<Panel> panels, ObservableCollection<JPanel> orderPanels)
        {
            InitializeComponent();
            DataContext = new ClosingRequestViewModel(request, panels, orderPanels);
        }
    }
}
