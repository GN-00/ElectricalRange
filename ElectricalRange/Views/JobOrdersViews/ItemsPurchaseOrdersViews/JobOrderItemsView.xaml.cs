using ProjectsNow.Data;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews
{
    public partial class JobOrderItemsView : UserControl, IView
    {
        public JobOrderItemsView(CompanyPO order, ObservableCollection<CompanyPOTransaction> items, ObservableCollection<ItemPurchased> jobOrderItems)
        {
            InitializeComponent();
            DataContext = new JobOrderItemsViewModel(order, items, jobOrderItems);
        }
    }
}
