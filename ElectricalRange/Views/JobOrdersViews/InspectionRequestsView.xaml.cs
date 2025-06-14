using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class InspectionView : UserControl, IView
    {
        public InspectionView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new InspectionRequestsViewModel(order, null, this);
        }
    }
}
