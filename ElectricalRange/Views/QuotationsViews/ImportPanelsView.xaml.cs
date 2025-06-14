using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class ImportPanelsView : UserControl, IView
    {
        public ImportPanelsView(Quotation quotation, ObservableCollection<QPanel> panels)
        {
            InitializeComponent();
            DataContext = new ImportPanelsViewModel(quotation, panels, this);
        }
    }
}
