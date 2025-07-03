
using ProjectsNow.Data.Production;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class TasksManagementView : UserControl, IView
    {
        public TasksManagementView(Order order)
        {
            InitializeComponent();
            DataContext = new TasksManagementViewModel(order, this);
        }
    }
}
