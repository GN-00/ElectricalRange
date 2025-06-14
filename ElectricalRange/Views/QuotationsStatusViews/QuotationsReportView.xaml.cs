using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    public partial class QuotationsReportView : UserControl, IView
    {
        public QuotationsReportView()
        {
            InitializeComponent();
            DataContext = new QuotationsReportViewModel(documentViewer);
        }
    }
}
