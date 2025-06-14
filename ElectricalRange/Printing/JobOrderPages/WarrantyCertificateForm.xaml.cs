using ProjectsNow.Data.JobOrders;

namespace ProjectsNow.Printing.JobOrderPages
{
    public partial class WarrantyCertificateForm : PageBase
    {
        public WarrantyCertificateForm(Warranty warranty)
        {
            InitializeComponent();
            DataContext = new WarrantyCertificateViewModel(warranty);
        }
    }
}
