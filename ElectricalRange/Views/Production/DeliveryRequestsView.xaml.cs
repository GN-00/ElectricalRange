using ProjectsNow.Data.Production;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class DeliveryRequestsView : UserControl, IView
    {
        public DeliveryRequestsView(Order order)
        {
            InitializeComponent();
            DataContext = new DeliveryRequestsViewModel(order, this);
        }
    }
}
