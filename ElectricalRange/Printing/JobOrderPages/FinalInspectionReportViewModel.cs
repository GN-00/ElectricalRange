using ProjectsNow.Data.JobOrders;

namespace ProjectsNow.Printing.JobOrderPages
{
    internal class FinalInspectionReportViewModel : PageViewModelBase
    {
        public FinalInspectionReportViewModel(QCPanel panel)
        {
            Data = panel;
        }

        public QCPanel Data { get; }
    }
}