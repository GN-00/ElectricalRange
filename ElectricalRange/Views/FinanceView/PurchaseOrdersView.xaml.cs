using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class PurchaseOrdersView : UserControl, IView
    {
        public PurchaseOrdersView()
        {
            InitializeComponent();
            DataContext = new PurchaseOrdersViewModel(this);
        }
    }
}
