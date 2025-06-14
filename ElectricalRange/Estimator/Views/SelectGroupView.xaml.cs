using ProjectsNow.Data.Quotations;
using ProjectsNow.Views;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Estimator.Views
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
