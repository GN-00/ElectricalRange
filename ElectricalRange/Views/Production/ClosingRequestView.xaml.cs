using ProjectsNow.Data.Production;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class ClosingRequestView : UserControl, IPopup
    {
        public ClosingRequestView(CloseRequest request, ObservableCollection<ProductionPanel> panels, ObservableCollection<ProductionPanel> orderPanels)
        {
            InitializeComponent();
            DataContext = new ClosingRequestViewModel(request, panels, orderPanels);
        }
    }
}
