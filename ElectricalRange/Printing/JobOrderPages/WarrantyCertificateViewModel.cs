using ProjectsNow.Data.JobOrders;

namespace ProjectsNow.Printing.JobOrderPages
{
    internal class WarrantyCertificateViewModel: PageViewModelBase
    {
        public Warranty Data { get; set; }
        public WarrantyCertificateViewModel(Warranty warranty)
        {
            Data = warranty;
        }
    }
}