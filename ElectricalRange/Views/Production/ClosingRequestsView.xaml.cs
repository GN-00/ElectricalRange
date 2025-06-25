using ProjectsNow.Data.Production;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class ClosingRequestsView : UserControl, IView
    {
        public ClosingRequestsView(Order order)
        {
            InitializeComponent();
            DataContext = new ClosingRequestsViewModel(order, this);
        }
    }
}
