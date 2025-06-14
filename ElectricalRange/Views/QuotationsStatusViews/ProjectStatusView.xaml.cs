using ProjectsNow.Data.QuotationsStatus;

using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    public partial class ProjectStatusView : UserControl, IPopup
    {
        public ProjectStatusView(Inquiry inquiry, Quotation quotation = null)
        {
            InitializeComponent();
            DataContext = new ProjectStatusViewModel(inquiry, quotation);
        }
    }
}
