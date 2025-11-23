using ProjectsNow.Data.Production;

using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class SelectingPanelsView : UserControl, IPopup
    {
        public SelectingPanelsView(Order order, IView checkPoint)
        {
            InitializeComponent();
            DataContext = new SelectingPanelsViewModel(order, checkPoint);
        }
    }
}
