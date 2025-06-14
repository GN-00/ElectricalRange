using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class WarrantiesView : UserControl, IView
    {
        public WarrantiesView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new WarrantiesViewModel(order, this);
        }
    }
}
