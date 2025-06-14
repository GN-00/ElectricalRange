using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class ApprovalsView : UserControl, IView
    {
        public ApprovalsView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new ApprovalsViewModel(order, null, this);
        }
    }
}
