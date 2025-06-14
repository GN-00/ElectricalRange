using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class SuppliersInvoicesView : UserControl, IView
    {
        public SuppliersInvoicesView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new SuppliersInvoicesViewModel(order, this);
        }
    }
}
