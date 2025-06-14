using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows.Controls;

using Panel = ProjectsNow.Data.JobOrders.TransactionPanel;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class JobFileRequestView : UserControl, IPopup
    {
        public JobFileRequestView(JobFileRequest request, ObservableCollection<Panel> panels, ObservableCollection<JPanel> orderPanels)
        {
            InitializeComponent();
            DataContext = new JobFileRequestViewModel(request, panels, orderPanels);
        }
    }
}
