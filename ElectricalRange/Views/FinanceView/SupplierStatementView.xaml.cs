using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class SupplierStatementView : UserControl, IView
    {
        public SupplierStatementView(SupplierAccount account)
        {
            InitializeComponent();
            DataContext = new SupplierStatementViewModel(account, this);
        }
    }
}
