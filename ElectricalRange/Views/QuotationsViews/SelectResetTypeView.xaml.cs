using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class SelectResetTypeView : UserControl, IPopup
    {
        public SelectResetTypeView(ObservableCollection<QItem> items)
        {
            InitializeComponent();
            DataContext = new SelectResetTypeViewModel(items);
        }
    }
}
