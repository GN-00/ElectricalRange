using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.StoreViews
{
    public partial class InvoicesView : UserControl, IView
    {
        public InvoicesView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new InvoicesViewModel(order, this);
        }
    }
}
