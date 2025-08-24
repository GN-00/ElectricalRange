using ProjectsNow.Data.Library;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.LibraryViews
{
    public partial class SelectGroupView : UserControl
    {
        public SelectGroupView(int panelId, ObservableCollection<DesignItem> items)
        {
            InitializeComponent();
            DataContext = new SelectGroupViewModel(panelId, items);
        }
    }
}
