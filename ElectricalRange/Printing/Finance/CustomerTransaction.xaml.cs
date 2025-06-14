using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Printing.Finance
{
    public partial class CustomerTransaction : UserControl
    {
        public CustomerTransaction(CustomerReceipt receipt)
        {
            InitializeComponent();
            DataContext = receipt;
        }
    }
}