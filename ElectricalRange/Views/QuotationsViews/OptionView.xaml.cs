using System.Collections.ObjectModel;
using System.Windows.Controls;

using Option = ProjectsNow.Data.Quotations.QuotationOption;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class OptionView : UserControl, IPopup
    {
        public OptionView(Option option, ObservableCollection<Option> options)
        {
            InitializeComponent();
            DataContext = new OptionViewModel(option, options);
        }
    }
}
