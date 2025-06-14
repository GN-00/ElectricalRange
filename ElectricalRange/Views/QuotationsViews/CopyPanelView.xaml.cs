using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class CopyPanelView : UserControl, IPopup
    {
        public CopyPanelView(QPanel panel, ObservableCollection<QPanel> panels)
        {
            InitializeComponent();
            DataContext = new CopyPanelViewModel(panel, panels);
        }
    }
}
