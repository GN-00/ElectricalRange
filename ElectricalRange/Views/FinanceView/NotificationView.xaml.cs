using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class NotificationView : UserControl, IPopup
    {
        public NotificationView(Notification notification, ObservableCollection<Notification> notifications)
        {
            InitializeComponent();
            DataContext = new NotificationViewModel(notification, notifications);
        }

        private void Amount_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
