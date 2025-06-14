using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class PaymentView : UserControl, IPopup
    {
        public PaymentView(AccountTransaction transaction, ObservableCollection<AccountTransaction> transactions)
        {
            InitializeComponent();
            DataContext = new PaymentViewModel(transaction, transactions);
        }

        private void Amount_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
