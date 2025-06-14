using ProjectsNow.Data.Quotations;
using ProjectsNow.Estimator.References;
using ProjectsNow.Views;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Estimator.Views
{
    public partial class SelectItemsView : UserControl, IPopup
    {
        public SelectItemsView(string groupId, int panelId, ObservableCollection<QItem> items, Selection selection = null)
        {
            InitializeComponent();
            DataContext = new SelectItemsViewModel(groupId, panelId, items, selection);
        }
    }
}
