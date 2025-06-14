using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class SelectItemTypeView : UserControl, IPopup
    {
        public SelectItemTypeView(QItem item, ObservableCollection<QItem> items, ItemsViewModel itemsViewModel)
        {
            InitializeComponent();
            DataContext = new SelectItemTypeViewModel(item, items, itemsViewModel);
        }
    }
}
