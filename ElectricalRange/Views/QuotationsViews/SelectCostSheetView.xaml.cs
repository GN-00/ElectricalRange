using ProjectsNow.Data.Quotations;

using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class SelectCostSheetView : UserControl, IPopup
    {
        public SelectCostSheetView(Quotation quotation, IView checkPoint)
        {
            InitializeComponent();
            DataContext = new SelectCostSheetViewModel(quotation, checkPoint);
        }

        public SelectCostSheetView(Quotation quotation, QPanel panel, IView checkPoint)
        {
            InitializeComponent();
            DataContext = new SelectCostSheetViewModel(quotation, panel, checkPoint);
        }
    }
}
