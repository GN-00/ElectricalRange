using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class PaymentsView : UserControl, IView
    {
        public PaymentsView()
        {
            InitializeComponent();
            DataContext = new PaymentsViewModel(this);
        }
    }
}
