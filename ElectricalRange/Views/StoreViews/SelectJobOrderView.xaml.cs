using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.StoreViews
{
    public partial class SelectJobOrderView : UserControl, IPopup
    {
        public SelectJobOrderView(StockViewModel viewModel)
        {
            InitializeComponent();
            DataContext = new SelectJobOrderViewModel(viewModel);
        }
    }
}
