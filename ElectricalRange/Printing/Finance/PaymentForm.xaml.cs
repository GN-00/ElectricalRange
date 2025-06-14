using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Printing.Finance
{
    public partial class PaymentForm : UserControl
    {
        public PaymentForm(TransactionBase payment)
        {
            InitializeComponent();
            DataContext = payment;
        }
    }
}
