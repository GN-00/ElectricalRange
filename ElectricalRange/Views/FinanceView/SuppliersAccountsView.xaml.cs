using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class SuppliersAccountsView : UserControl, IView
    {
        public SuppliersAccountsView()
        {
            InitializeComponent();
            DataContext = new SuppliersAccountsViewModel(this);
        }
    }
}
