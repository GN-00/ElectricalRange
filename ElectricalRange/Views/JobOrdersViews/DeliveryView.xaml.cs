using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows.Controls;

using Panel = ProjectsNow.Data.JobOrders.TransactionPanel;


namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class DeliveryView : UserControl, IPopup
    {
        public DeliveryView(Delivery delivery, ObservableCollection<Panel> panels, ObservableCollection<JPanel> orderPanels)
        {
            InitializeComponent();
            DataContext = new DeliveryViewModel(delivery, panels, orderPanels);
        }
    }
}
