using ProjectsNow.Data.Quotations;

using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class DiscountsView : UserControl, IPopup
    {
        public DiscountsView(Quotation quotation)
        {
            InitializeComponent();
            DataContext = new DiscountsViewModel(quotation);
        }
    }
}
