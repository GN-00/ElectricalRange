
using ProjectsNow.Data.Production;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Views.Production
{
    public partial class TaskKanbanView : UserControl
    {
        private bool _isDragging = false;
        private Point _startPoint;
        private Task _draggedTask;

        public TaskKanbanView(Order order)
        {
            InitializeComponent();
            DataContext = new TaskKanbanViewModel(order);
        }

        private void TaskCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            _draggedTask = ((Border)sender).DataContext as Task;
        }

        private void TaskCard_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
            {
                Point mousePos = e.GetPosition(null);
                Vector diff = _startPoint - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    _isDragging = true;
                    
                    var data = new DataObject("task", _draggedTask);
                    DragDrop.DoDragDrop((Border)sender, data, DragDropEffects.Move);
                    
                    _isDragging = false;
                }
            }
        }

        private void Column_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("task"))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Column_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("task"))
            {
                var task = e.Data.GetData("task") as Task;
                var border = sender as Border;
                
                string newStatus = "To Do";
                if (border.Tag.ToString() == "In Progress")
                    newStatus = "In Progress";
                else if (border.Tag.ToString() == "Done")
                    newStatus = "Done";

                var viewModel = DataContext as TaskKanbanViewModel;
                viewModel?.MoveTask(task, newStatus);
            }
        }

        private void AssignEmployees_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var task = ((ContextMenu)menuItem.Parent).PlacementTarget.DataContext as Task;
            
            var viewModel = DataContext as TaskKanbanViewModel;
            viewModel?.AssignEmployees(task);
        }

        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var task = ((ContextMenu)menuItem.Parent).PlacementTarget.DataContext as Task;
            
            var viewModel = DataContext as TaskKanbanViewModel;
            viewModel?.EditTask(task);
        }
    }
}
