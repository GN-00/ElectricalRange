using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews
{
    public partial class PurchaseOrdersView : UserControl, IView
    {
        public PurchaseOrdersView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new PurchaseOrdersViewModel(order, this);
        }
    }
}
