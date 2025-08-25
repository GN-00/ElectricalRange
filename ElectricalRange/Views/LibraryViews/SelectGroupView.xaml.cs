using ProjectsNow.Data.Library;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.LibraryViews
{
    public partial class SelectGroupView : UserControl, IPopup
    {
        public SelectGroupView(int panelId, ObservableCollection<QItem> items)
        {
            InitializeComponent();
            DataContext = new SelectGroupViewModel(panelId, items);
        }
    }
}
