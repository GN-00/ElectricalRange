using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class NotificationForReceiptView : UserControl, IView
    {
        public NotificationForReceiptView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new NotificationForReceiptViewModel(order, this);
        }
    }
}
