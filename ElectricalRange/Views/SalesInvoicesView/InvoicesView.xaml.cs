using System.Windows.Controls;

namespace ProjectsNow.Views.SalesInvoicesView
{
    public partial class InvoicesView : UserControl, IView
    {
        public InvoicesView()
        {
            InitializeComponent();
            DataContext = new InvoicesViewModel(this);
        }
    }
}
