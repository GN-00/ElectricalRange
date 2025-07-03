
using ProjectsNow.Data.Production;
using System.Windows;
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

        private void ListViewButton_Click(object sender, RoutedEventArgs e)
        {
            // Show list view, hide kanban view
            ListViewGrid.Visibility = Visibility.Visible;
            KanbanViewGrid.Visibility = Visibility.Collapsed;
            
            // Update button styles
            ListViewButton.Background = (System.Windows.Media.Brush)FindResource("Blue");
            ListViewButton.Foreground = System.Windows.Media.Brushes.White;
            KanbanViewButton.Background = System.Windows.Media.Brushes.LightGray;
            KanbanViewButton.Foreground = System.Windows.Media.Brushes.Black;
        }

        private void KanbanViewButton_Click(object sender, RoutedEventArgs e)
        {
            // Show kanban view, hide list view
            ListViewGrid.Visibility = Visibility.Collapsed;
            KanbanViewGrid.Visibility = Visibility.Visible;
            
            // Update button styles
            KanbanViewButton.Background = (System.Windows.Media.Brush)FindResource("Green");
            KanbanViewButton.Foreground = System.Windows.Media.Brushes.White;
            ListViewButton.Background = System.Windows.Media.Brushes.LightGray;
            ListViewButton.Foreground = System.Windows.Media.Brushes.Black;
        }
    }
}
