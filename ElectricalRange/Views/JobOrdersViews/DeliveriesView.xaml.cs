using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class DeliveriesView : UserControl, IView
    {
        public DeliveriesView(JobOrder order, ObservableCollection<JPanel> orderPanels = null)
        {
            InitializeComponent();
            DataContext = new DeliveriesViewModel(order, orderPanels, this);
        }
    }
}
