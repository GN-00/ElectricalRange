using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class WarrantyPanelsView : UserControl, IPopup
    {
        public WarrantyPanelsView(ObservableCollection<WPanel> warrantyPanels, ObservableCollection<WPanel> orderPanels)
        {
            InitializeComponent();
            DataContext = new WarrantyPanelsViewModel(warrantyPanels, orderPanels);
        }
    }
}
