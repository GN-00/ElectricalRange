using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class PurchaseView : UserControl, IPopup
    {
        public PurchaseView(JobOrder jobOrder, IView checkPoint)
        {
            InitializeComponent();
            DataContext = new PurchaseViewModel(jobOrder, checkPoint);
        }
    }
}
