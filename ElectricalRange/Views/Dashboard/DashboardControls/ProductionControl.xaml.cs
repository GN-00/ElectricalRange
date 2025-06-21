using System.Windows.Controls;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public partial class ProductionControl : UserControl
    {
        public ProductionControl()
        {
            InitializeComponent();
            DataContext = new ProductionViewModel();
        }
    }
}
