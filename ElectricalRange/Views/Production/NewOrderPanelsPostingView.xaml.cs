using ProjectsNow.Data.Production;
using System.Collections.ObjectModel;

namespace ProjectsNow.Views.Production
{
    public partial class NewOrderPanelsPostingView : System.Windows.Controls.UserControl, IPopup
    {
        public NewOrderPanelsPostingView(JobFile request, ObservableCollection<ProductionPanel> panels, ObservableCollection<ProductionPanel> orderPanels)
        {
            InitializeComponent();
            DataContext = new NewOrderPanelsPostingViewModel(request, panels, orderPanels);
        }
    }
}
