using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class ClosingRequestsView : UserControl, IView
    {
        public ClosingRequestsView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new ClosingRequestsViewModel(order, null, this);
        }
    }
}