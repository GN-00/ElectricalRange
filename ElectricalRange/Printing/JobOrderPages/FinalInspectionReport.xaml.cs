using ProjectsNow.Data.JobOrders;

namespace ProjectsNow.Printing.JobOrderPages
{
    public partial class FinalInspectionReport : PageBase
    {
        public FinalInspectionReport(QCPanel panel)
        {
            InitializeComponent();
            DataContext = new FinalInspectionReportViewModel(panel);
        }
    }
}
