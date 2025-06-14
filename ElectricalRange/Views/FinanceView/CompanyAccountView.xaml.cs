using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class CompanyAccountView : UserControl, IPopup
    {
        public CompanyAccountView(Account accountData, ObservableCollection<Account> accountsData)
        {
            InitializeComponent();
            DataContext = new CompanyAccountViewModel(accountData, accountsData);
        }
    }
}
