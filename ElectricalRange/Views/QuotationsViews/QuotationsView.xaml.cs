using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class QuotationsView : UserControl, IView
    {
        public QuotationsView()
        {
            InitializeComponent();
            DataContext = new QuotationsViewModel(this);
        }
    }
}
