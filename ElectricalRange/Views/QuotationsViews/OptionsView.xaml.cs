using ProjectsNow.Data.Quotations;

using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class OptionsView : UserControl, IView
    {
        public OptionsView(Quotation quotation)
        {
            InitializeComponent();
            DataContext = new OptionsViewModel(quotation, this);
        }
    }
}
