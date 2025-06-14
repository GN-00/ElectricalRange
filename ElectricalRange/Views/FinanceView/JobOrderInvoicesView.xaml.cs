using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class JobOrderInvoicesView : UserControl, IView
    {
        public JobOrderInvoicesView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new JobOrderInvoicesViewModel(order, this);
        }
    }
}
