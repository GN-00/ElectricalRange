using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class SelectInvoiceTypeView : UserControl, IPopup
    {
        public SelectInvoiceTypeView(JobOrder order, IView checkPoint)
        {
            InitializeComponent();
            DataContext = new SelectInvoiceTypeViewModel(order, checkPoint);
        }
    }
}
