using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class CustomerInvoicesView : UserControl, IView
    {
        public CustomerInvoicesView(CustomerAccount account)
        {
            InitializeComponent();
            DataContext = new CustomerInvoicesViewModel(account, this);
        }
    }
}
