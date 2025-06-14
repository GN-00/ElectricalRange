using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class TargetPriceView : UserControl, IPopup
    {
        public TargetPriceView(Quotation quotation, ObservableCollection<QPanel> panels)
        {
            InitializeComponent();
            DataContext = new TargetPriceViewModel(quotation, panels);
        }

        private void NewData_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
