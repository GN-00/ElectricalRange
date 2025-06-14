using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows.Controls;

using Panel = ProjectsNow.Data.JobOrders.TransactionPanel;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class InspectionRequestView : UserControl, IPopup
    {
        public InspectionRequestView(InspectionRequest request, ObservableCollection<Panel> panels, ObservableCollection<JPanel> orderPanels)
        {
            InitializeComponent();
            DataContext = new InspectionRequestViewModel(request, panels, orderPanels);
        }
    }
}
