
using ProjectsNow.Data.Production;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class KanbanView : UserControl, IView
    {
        public KanbanView(Order order)
        {
            InitializeComponent();
            DataContext = new KanbanViewModel(order, this);
        }
    }
}
