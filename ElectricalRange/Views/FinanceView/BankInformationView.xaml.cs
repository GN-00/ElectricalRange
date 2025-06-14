using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class BankInformationView : UserControl, IPopup
    {
        public BankInformationView(Account account)
        {
            InitializeComponent();
            DataContext = new BankInformationViewModel(account);
        }

        public BankInformationView(CustomerAccount account)
        {
            InitializeComponent();
            DataContext = new BankInformationViewModel(account);
        }
    }
}
