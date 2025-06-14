using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.Quotations;

using System.Windows.Controls;

namespace ProjectsNow.Views.InquiriesViews
{
    public partial class AssignView : UserControl, IPopup
    {
        public AssignView(Inquiry inquiry)
        {
            InitializeComponent();
            DataContext = new AssignViewModel(inquiry);
        }
    }
}
