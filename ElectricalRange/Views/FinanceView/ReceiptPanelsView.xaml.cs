using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class ReceiptPanelsView : UserControl, IPopup
    {
        public ReceiptPanelsView(Notification notification, ObservableCollection<ReceiptPanel> panels)
        {
            InitializeComponent();
            DataContext = new ReceiptPanelsViewModel(notification, panels);
        }
    }
}
