using System.Windows.Controls;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public partial class StatusControl : UserControl
    {
        public StatusControl()
        {
            InitializeComponent();
            DataContext = new StatusViewModel();
        }
    }
}
