using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class PanelView : UserControl, IView
    {
        public PanelView(Quotation quotation, QPanel panel, ObservableCollection<QPanel> panels)
        {
            InitializeComponent();
            DataContext = new PanelViewModel(quotation, panel, panels);
        }

        private void IntOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 4);
        }
        private void DoubleOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
