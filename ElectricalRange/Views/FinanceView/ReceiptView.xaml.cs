using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class ReceiptView : UserControl, IPopup
    {
        public ReceiptView(AccountTransaction transaction, ObservableCollection<AccountTransaction> transactions)
        {
            InitializeComponent();
            DataContext = new ReceiptViewModel(transaction, transactions);
        }

        private void Amount_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
