using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class OrdersView : UserControl, IView
    {
        public OrdersView()
        {
            InitializeComponent();
            DataContext = new OrdersViewModel(this);
        }
    }
}
