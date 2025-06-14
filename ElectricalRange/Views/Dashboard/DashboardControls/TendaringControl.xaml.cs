using System.Windows.Controls;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public partial class TendaringControl : UserControl
    {
        public TendaringControl()
        {
            InitializeComponent();
            DataContext = new TendaringViewModel();
        }
    }
}
