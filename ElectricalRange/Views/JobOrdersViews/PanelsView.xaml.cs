using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class PanelsView : UserControl, IView
    {
        public PanelsView(JobOrder jobOrder)
        {
            InitializeComponent();
            DataContext = new PanelsViewModel(jobOrder, this);
        }
    }
}