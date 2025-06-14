using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class AccountTransactionsView : UserControl, IView
    {
        public AccountTransactionsView(Account account = null)
        {
            InitializeComponent();
            DataContext = new AccountTransactionsViewModel(account);
        }

        public AccountTransactionsView(CustomerAccount account = null)
        {
            InitializeComponent();
            DataContext = new AccountTransactionsViewModel(account);
        }

        public AccountTransactionsView(SupplierAccount account = null)
        {
            InitializeComponent();
            DataContext = new AccountTransactionsViewModel(account);
        }
    }
}
