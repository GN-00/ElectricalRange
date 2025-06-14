using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class ReceiptsView : UserControl, IView
    {
        public ReceiptsView()
        {
            InitializeComponent();
            DataContext = new ReceiptsViewModel(this);
        }
    }
}
