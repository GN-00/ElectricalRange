using ProjectsNow.Data.Quotations;

using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class PriceView : UserControl, IPopup
    {
        public PriceView(Quotation quotation)
        {
            InitializeComponent();
            DataContext = new PriceViewModel(quotation);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }

    }
}
