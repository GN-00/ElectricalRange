using System.Windows.Controls;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public partial class FinanceControl : UserControl
    {
        public FinanceControl()
        {
            InitializeComponent();
            DataContext = new FinanceViewModel();
        }
    }
}
