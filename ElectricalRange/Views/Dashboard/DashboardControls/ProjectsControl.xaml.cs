using System.Windows.Controls;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public partial class ProjectsControl : UserControl
    {
        public ProjectsControl()
        {
            InitializeComponent();
            DataContext = new ProjectsViewModel();
        }
    }
}
