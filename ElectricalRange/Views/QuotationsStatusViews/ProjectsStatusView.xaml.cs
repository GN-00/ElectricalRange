using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    public partial class ProjectsStatusView : UserControl, IView
    {
        public ProjectsStatusView()
        {
            InitializeComponent();
            DataContext = new ProjectsStatusViewModel(this);
        }
    }
}
