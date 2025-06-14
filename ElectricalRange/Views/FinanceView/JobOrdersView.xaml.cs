using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class JobOrdersView : UserControl, IView
    {
        public JobOrdersView(CustomerAccount account = null)
        {
            InitializeComponent();
            DataContext = new JobOrdersViewModel(account, this);
        }
    }
}
