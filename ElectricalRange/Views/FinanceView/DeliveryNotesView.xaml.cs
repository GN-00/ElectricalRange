using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class DeliveryNotesView : UserControl, IView
    {
        public DeliveryNotesView()
        {
            InitializeComponent();
            DataContext = new DeliveryNotesViewModel(this);
        }
    }
}
