using ProjectsNow.Data.Quotations;

using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class TermsView : UserControl, IView
    {
        public TermsView(Quotation quotation)
        {
            InitializeComponent();
            DataContext = new TermsViewModel(quotation, this);
        }
    }
}
