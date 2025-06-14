using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class DownloadPanelView : UserControl, IPopup
    {
        public DownloadPanelView(Quotation quotation, QPanel panel, ObservableCollection<QPanel> panels)
        {
            InitializeComponent();
            DataContext = new DownloadPanelViewModel(quotation, panel, panels);
        }
    }
}
