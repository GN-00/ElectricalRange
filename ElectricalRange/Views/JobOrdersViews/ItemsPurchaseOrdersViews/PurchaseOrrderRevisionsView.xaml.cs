using ProjectsNow.Data;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews
{
    public partial class PurchaseOrrderRevisionsView : UserControl, IPopup
    {
        public PurchaseOrrderRevisionsView(CompanyPO order, IView view)
        {
            InitializeComponent();
            DataContext = new PurchaseOrrderRevisionsViewModel(order, view);
        }
    }
}
