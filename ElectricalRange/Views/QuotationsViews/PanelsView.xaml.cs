using ProjectsNow.Data.Quotations;

using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class PanelsView : UserControl, IView
    {
        public PanelsView(Quotation quotation)
        {
            InitializeComponent();
            DataContext = new PanelsViewModel(quotation, this);
        }
    }
}
