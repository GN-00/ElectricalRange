using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class JobOrdersView : UserControl, IView
    {
        public JobOrdersView()
        {
            InitializeComponent();
            DataContext = new JobOrdersViewModel(this);
        }
    }
}
