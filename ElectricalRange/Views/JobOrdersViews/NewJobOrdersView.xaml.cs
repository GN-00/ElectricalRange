using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class NewJobOrdersView : UserControl, IView
    {
        public NewJobOrdersView()
        {
            InitializeComponent();
            DataContext = new NewJobOrdersViewModel(this);
        }
    }
}
