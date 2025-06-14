using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class CompanyAccountsView : UserControl, IView
    {
        public CompanyAccountsView()
        {
            InitializeComponent();
            DataContext = new CompanyAccountsViewModel(this);
        }
    }
}
