using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class QualityView : UserControl, IView
    {
        public QualityView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new QualityViewModel(order, this);
        }
    }
}
