using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class PurchaseOrderView : UserControl, IView
    {
        public PurchaseOrderView(JobOrder jobOrder)
        {
            InitializeComponent();
            DataContext = new PurchaseOrderViewModel(jobOrder, this);
        }
    }
}
