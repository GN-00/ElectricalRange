using System.Windows.Controls;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public partial class StoreControl : UserControl
    {
        public StoreControl()
        {
            InitializeComponent();
            DataContext = new StoreViewModel();
        }
    }
}
