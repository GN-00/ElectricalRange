using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class SelectAcknowledgmentView : UserControl, IPopup
    {
        public SelectAcknowledgmentView(JobOrder jobOrder, IView checkPoint)
        {
            InitializeComponent();
            DataContext = new SelectAcknowledgmentViewModel(jobOrder, checkPoint);
        }
    }
}
