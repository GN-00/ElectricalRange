using ProjectsNow.Data.JobOrders;

namespace ProjectsNow.Printing.JobOrderPages
{
    public partial class WarrantyCertificateDetailsForm : PageBase
    {
        public WarrantyCertificateDetailsForm(Warranty warranty)
        {
            InitializeComponent();
            DataContext = new WarrantyCertificateViewModel(warranty);
        }
    }
}
