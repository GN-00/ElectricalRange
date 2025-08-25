using ProjectsNow.Data.Library;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.LibraryViews
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
