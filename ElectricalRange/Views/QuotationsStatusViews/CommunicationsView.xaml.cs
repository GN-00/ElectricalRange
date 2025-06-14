using ProjectsNow.Data.QuotationsStatus;

using System.ComponentModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    public partial class CommunicationsView : UserControl, IView
    {
        public CommunicationsView(Quotation quotation, ICollectionView quotationsCollection)
        {
            InitializeComponent();
            DataContext = new CommunicationsViewModel(quotation, quotationsCollection, this);
        }

        public CommunicationsView(int inquiryId)
        {
            InitializeComponent();
            DataContext = new CommunicationsViewModel(inquiryId, this);
        }
    }
}
