using System.Windows.Controls;

namespace ProjectsNow.Views.ReferencesViews
{
    public partial class ReferencesView : UserControl, IView
    {
        public ReferencesView()
        {
            InitializeComponent();
            DataContext = new ReferencesViewModel();
        }
    }
}
