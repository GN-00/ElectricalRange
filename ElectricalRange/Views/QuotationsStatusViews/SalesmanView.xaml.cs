using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    public partial class SalesmanView : UserControl, IPopup
    {
        public SalesmanView(ProjectsStatusViewModel model)
        {
            InitializeComponent();
            DataContext = new SalesmanVieModel(model);
        }
    }
}
