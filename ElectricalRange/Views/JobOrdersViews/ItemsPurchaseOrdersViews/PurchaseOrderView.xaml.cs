using System.Collections.ObjectModel;
using System.Windows.Controls;

using PurchaseOrder = ProjectsNow.Data.CompanyPO;

namespace ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews
{
    public partial class PurchaseOrderView : UserControl, IView
    {
        public PurchaseOrderView(PurchaseOrder order, ObservableCollection<PurchaseOrder> orders)
        {
            InitializeComponent();
            DataContext = new PurchaseOrderViewModel(order, orders, this);
        }
    }
}
