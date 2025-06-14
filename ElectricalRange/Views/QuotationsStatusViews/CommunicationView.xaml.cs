using ProjectsNow.Data.Inquiries;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    public partial class CommunicationView : UserControl, IPopup
    {
        public CommunicationView(Communication communicationData, ObservableCollection<Communication> communicationsData = null)
        {
            InitializeComponent();
            DataContext = new CommunicationViewModel(communicationData, communicationsData);
        }
    }
}
