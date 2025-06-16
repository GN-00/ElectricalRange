using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class OrdersNew : UserControl, IView
    {
        public OrdersNew()
        {
            InitializeComponent();
            DataContext = new OrdersNewViewModel(this);
        }
    }
}
