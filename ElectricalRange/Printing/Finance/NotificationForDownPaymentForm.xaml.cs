using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Printing.Finance
{
    public partial class NotificationForDownPaymentForm : UserControl
    {
        public NotificationForDownPaymentForm()
        {
            InitializeComponent();
        }
        public NotificationForDownPaymentForm(DownPayment downPyment)
        {
            InitializeComponent();
            DataContext = downPyment;
        }
    }
}
