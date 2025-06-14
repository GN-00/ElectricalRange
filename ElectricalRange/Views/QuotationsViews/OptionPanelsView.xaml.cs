using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class OptionPanelsView : UserControl, IPopup
    {
        public OptionPanelsView(QuotationOption option, ObservableCollection<QuotationOptionPanel> panels)
        {
            InitializeComponent();
            DataContext = new OptionPanelsViewModel(option, panels);
        }
    }
}
