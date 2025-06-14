using ProjectsNow.Data.Finance;

using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class ProformaInvoicesView : UserControl, IView
    {
        public ProformaInvoicesView(JobOrder order)
        {
            InitializeComponent();
            DataContext = new ProformaInvoicesViewModel(order, this);
        }
    }
}
