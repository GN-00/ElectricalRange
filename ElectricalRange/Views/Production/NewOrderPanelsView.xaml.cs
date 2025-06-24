using ProjectsNow.Data.Production;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class NewOrderPanelsView : UserControl, IView
    {
        public NewOrderPanelsView(Order order)
        {
            InitializeComponent();
            DataContext = new NewOrderPanelsViewModel(order, null, this);
        }
    }
}
