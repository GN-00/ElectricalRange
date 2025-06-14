using System.Windows.Controls;

namespace ProjectsNow.Views.Dashboard
{
    public partial class DashboardView : UserControl, IView
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel(this);
            Navigation.DashboardViewData = this;
        }
    }
}
