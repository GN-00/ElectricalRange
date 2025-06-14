using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class JobFileRequestsView : UserControl, IView
    {
        public JobFileRequestsView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new JobFileRequestsViewModel(order, null, this);
        }
    }
}
