using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    public partial class EstimatorView : UserControl, IPopup
    {
        public EstimatorView(ProjectsStatusViewModel model)
        {
            InitializeComponent();
            DataContext = new EstimatorVieModel(model);
        }
    }
}
