using ProjectsNow.Data.Production;

using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class ReceiveItemsView : UserControl, IView
    {
        public ReceiveItemsView(Order order)
        {
            InitializeComponent();
            DataContext = new ReceiveItemsViewModel(order, this);
        }
    }
}
