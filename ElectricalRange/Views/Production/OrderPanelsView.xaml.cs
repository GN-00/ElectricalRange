using ProjectsNow.Data.Production;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class OrderPanelsView : UserControl, IView
    {
        public OrderPanelsView(Order order)
        {
            InitializeComponent();
            DataContext = new OrderPanelsViewModel(order, (IView)this);
        }
    }
}
