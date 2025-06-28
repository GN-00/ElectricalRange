using ProjectsNow.Data.Production;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class DeliveryRequestView : UserControl, IPopup
    {
        public DeliveryRequestView(DeliveryRequest request, ObservableCollection<ProductionPanel> panels, ObservableCollection<ProductionPanel> orderPanels)
        {
            InitializeComponent();
            DataContext = new DeliveryRequestViewModel(request, panels, orderPanels);
        }
    }
}
