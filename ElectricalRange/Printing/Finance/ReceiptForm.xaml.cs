using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Printing.Finance
{
    public partial class ReceiptForm : UserControl
    {
        public ReceiptForm(TransactionBase receipt)
        {
            InitializeComponent();
            DataContext = receipt;
        }
    }
}
