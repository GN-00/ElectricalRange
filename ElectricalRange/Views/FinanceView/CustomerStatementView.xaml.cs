using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class CustomerStatementView : UserControl, IView
    {
        public CustomerStatementView(CustomerAccount account)
        {
            InitializeComponent();
            DataContext = new CustomerStatementViewModel(account, this);
        }
    }
}
