using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class SpecialPanelView : UserControl, IPopup
    {
        public SpecialPanelView(Quotation quotation, QPanel panel, ObservableCollection<QPanel> panels)
        {
            InitializeComponent();
            DataContext = new SpecialPanelViewModel(quotation, panel, panels);
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
