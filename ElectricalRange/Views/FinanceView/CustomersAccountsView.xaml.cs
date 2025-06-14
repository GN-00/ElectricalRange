using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class CustomersAccountsView : UserControl, IView
    {
        public CustomersAccountsView()
        {
            InitializeComponent();
            DataContext = new CustomersAccountsViewModel(this);
        }
    }
}
