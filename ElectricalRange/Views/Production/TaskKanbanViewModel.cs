
using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProjectsNow.Views.Production
{
    public class TaskKanbanViewModel : ViewModelBase
    {
        private ObservableCollection<Task> _toDoTasks;
        private ObservableCollection<Task> _inProgressTasks;
        private ObservableCollection<Task> _doneTasks;

        public TaskKanbanViewModel(Order order)
        {
            OrderData = order;
            LoadTasks();
        }

        public Order OrderData { get; }

        public ObservableCollection<Task> ToDoTasks
        {
            get => _toDoTasks;
            set => SetValue(ref _toDoTasks, value);
        }

        public ObservableCollection<Task> InProgressTasks
        {
            get => _inProgressTasks;
            set => SetValue(ref _inProgressTasks, value);
        }

        public ObservableCollection<Task> DoneTasks
        {
            get => _doneTasks;
            set => SetValue(ref _doneTasks, value);
        }

        private void LoadTasks()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            
            string query = @"
                SELECT t.*, p.Name as PanelName, p.SN as PanelCode 
                FROM [Production].[Tasks] t
                LEFT JOIN [Production].[Panels(View)] p ON t.PanelId = p.PanelId
                WHERE t.JobOrderId = @JobOrderId";
            
            var allTasks = connection.Query<Task>(query, new { JobOrderId = OrderData.JobOrderId }).ToList();

            ToDoTasks = new ObservableCollection<Task>(allTasks.Where(t => t.Status == "To Do"));
            InProgressTasks = new ObservableCollection<Task>(allTasks.Where(t => t.Status == "In Progress"));
            DoneTasks = new ObservableCollection<Task>(allTasks.Where(t => t.Status == "Done"));
        }

        public void RefreshTasks(ObservableCollection<Task> tasks)
        {
            ToDoTasks = new ObservableCollection<Task>(tasks.Where(t => t.Status == "To Do"));
            InProgressTasks = new ObservableCollection<Task>(tasks.Where(t => t.Status == "In Progress"));
            DoneTasks = new ObservableCollection<Task>(tasks.Where(t => t.Status == "Done"));
        }

        public void MoveTask(Task task, string newStatus)
        {
            if (task == null || task.Status == newStatus) return;

            // Update task status
            task.Status = newStatus;
            
            // Update progress based on status
            if (newStatus == "To Do")
                task.Progress = 0;
            else if (newStatus == "In Progress" && task.Progress == 0)
                task.Progress = 50;
            else if (newStatus == "Done")
                task.Progress = 100;

            // Update in database
            using SqlConnection connection = new(Database.ConnectionString);
            connection.Update(task);

            // Refresh the kanban board
            LoadTasks();
        }

        public void AssignEmployees(Task task)
        {
            if (task == null) return;

            var assignWindow = new AssignEmployeesWindow(task);
            if (assignWindow.ShowDialog() == true)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                connection.Update(task);
                LoadTasks();
            }
        }

        public void EditTask(Task task)
        {
            if (task == null) return;

            // Get panels for the dropdown
            using SqlConnection connection = new(Database.ConnectionString);
            string panelsQuery = $"SELECT * FROM [Production].[Panels(View)] WHERE JobOrderId = {OrderData.JobOrderId}";
            var panels = connection.Query<ProductionPanel>(panelsQuery).ToList();

            var editWindow = new TaskEditWindow(task, panels);
            if (editWindow.ShowDialog() == true)
            {
                using SqlConnection connection2 = new(Database.ConnectionString);
                connection2.Update(task);
                LoadTasks();
            }
        }
    }
}
