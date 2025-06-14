using ProjectsNow.Data.Quotations;

using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class RevisionsView : UserControl, IPopup
    {
        public RevisionsView(Quotation quotation, IView checkPoint)
        {
            InitializeComponent();
            DataContext = new RevisionsViewModel(quotation, checkPoint);
        }
    }
}
