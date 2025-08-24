using ProjectsNow.Data.Library;

using System.Collections.ObjectModel;

namespace ProjectsNow.Views.LibraryViews
{
    internal class SelectGroupViewModel
    {
        private int panelId;
        private ObservableCollection<DesignItem> items;

        public SelectGroupViewModel(int panelId, ObservableCollection<DesignItem> items)
        {
            this.panelId = panelId;
            this.items = items;
        }
    }
}