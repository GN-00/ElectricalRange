
using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Data.Users;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ProjectsNow.Views.Production
{
    public class TasksManagementViewModel : ViewModelBase
    {
        private ObservableCollection<ProductionPanel> _panels;
        private ObservableCollection<Task> _tasks;
        private ProductionPanel _selectedPanel;
        private Task _selectedTask;
        private TaskKanbanView _kanbanView;

        public TasksManagementViewModel(Order order, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            OrderData = order;

            LoadData();
            InitializeKanbanView();

            AddTaskCommand = new RelayCommand(AddTask, CanAddTask);
            EditTaskCommand = new RelayCommand(EditTask, CanEditTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask, CanDeleteTask);
            AssignEmployeesCommand = new RelayCommand(AssignEmployees, CanAssignEmployees);
            RefreshCommand = new RelayCommand(LoadData);
        }

        public User UserData { get; }
        public Order OrderData { get; }

        public ObservableCollection<ProductionPanel> Panels
        {
            get => _panels;
            set => SetValue(ref _panels, value);
        }

        public ObservableCollection<Task> Tasks
        {
            get => _tasks;
            set => SetValue(ref _tasks, value);
        }

        public ProductionPanel SelectedPanel
        {
            get => _selectedPanel;
            set
            {
                if (SetValue(ref _selectedPanel, value))
                {
                    LoadTasksForPanel();
                }
            }
        }

        public Task SelectedTask
        {
            get => _selectedTask;
            set => SetValue(ref _selectedTask, value);
        }

        public TaskKanbanView KanbanView
        {
            get => _kanbanView;
            set => SetValue(ref _kanbanView, value);
        }

        public RelayCommand AddTaskCommand { get; }
        public RelayCommand EditTaskCommand { get; }
        public RelayCommand DeleteTaskCommand { get; }
        public RelayCommand AssignEmployeesCommand { get; }
        public RelayCommand RefreshCommand { get; }

        private void LoadData()
        {
            using SqlConnection connection = new(Database.ConnectionString);

            // Load panels
            string panelsQuery = $"SELECT * FROM [Production].[Panels(View)] WHERE JobOrderId = {OrderData.JobOrderId}";
            Panels = new ObservableCollection<ProductionPanel>(connection.Query<ProductionPanel>(panelsQuery));

            // Load all tasks for the job order
            LoadAllTasks();
        }

        private void LoadAllTasks()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            
            string tasksQuery = @"
                SELECT t.*, p.Name as PanelName, p.SN as PanelCode 
                FROM [Production].[Tasks] t
                LEFT JOIN [Production].[Panels(View)] p ON t.PanelId = p.PanelId
                WHERE t.JobOrderId = @JobOrderId";
            
            Tasks = new ObservableCollection<Task>(connection.Query<Task>(tasksQuery, new { JobOrderId = OrderData.JobOrderId }));
            
            // Update Kanban view
            if (KanbanView != null)
            {
                ((TaskKanbanViewModel)KanbanView.DataContext).RefreshTasks(Tasks);
            }
        }

        private void LoadTasksForPanel()
        {
            if (SelectedPanel == null)
            {
                LoadAllTasks();
                return;
            }

            using SqlConnection connection = new(Database.ConnectionString);
            
            string tasksQuery = @"
                SELECT t.*, p.Name as PanelName, p.SN as PanelCode 
                FROM [Production].[Tasks] t
                LEFT JOIN [Production].[Panels(View)] p ON t.PanelId = p.PanelId
                WHERE t.PanelId = @PanelId";
            
            Tasks = new ObservableCollection<Task>(connection.Query<Task>(tasksQuery, new { PanelId = SelectedPanel.PanelId }));
            
            // Update Kanban view
            if (KanbanView != null)
            {
                ((TaskKanbanViewModel)KanbanView.DataContext).RefreshTasks(Tasks);
            }
        }

        private void InitializeKanbanView()
        {
            KanbanView = new TaskKanbanView(OrderData);
        }

        private void AddTask()
        {
            if (SelectedPanel == null)
            {
                MessageView.Show("Selection Required", 
                               "Please select a panel first.", 
                               MessageViewButton.OK, 
                               MessageViewImage.Warning);
                return;
            }

            var newTask = new Task
            {
                PanelId = SelectedPanel.PanelId,
                JobOrderId = OrderData.JobOrderId,
                TaskName = "New Task",
                StartDate = DateTime.Now,
                Status = "To Do",
                Progress = 0
            };

            // Open task edit dialog
            var editWindow = new TaskEditWindow(newTask, Panels.ToList());
            if (editWindow.ShowDialog() == true)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                connection.Insert(newTask);
                LoadTasksForPanel();
            }
        }

        private bool CanAddTask() => SelectedPanel != null;

        private void EditTask()
        {
            if (SelectedTask == null) return;

            var editWindow = new TaskEditWindow(SelectedTask, Panels.ToList());
            if (editWindow.ShowDialog() == true)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                connection.Update(SelectedTask);
                LoadTasksForPanel();
            }
        }

        private bool CanEditTask() => SelectedTask != null;

        private void DeleteTask()
        {
            if (SelectedTask == null) return;

            var result = MessageView.Show("Delete Task", 
                                        $"Are you sure you want to delete the task '{SelectedTask.TaskName}'?", 
                                        MessageViewButton.YesNo, 
                                        MessageViewImage.Question);
            
            if (result == MessageViewResult.Yes)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                connection.Delete(SelectedTask);
                LoadTasksForPanel();
            }
        }

        private bool CanDeleteTask() => SelectedTask != null;

        private void AssignEmployees()
        {
            if (SelectedTask == null) return;

            var assignWindow = new AssignEmployeesWindow(SelectedTask);
            if (assignWindow.ShowDialog() == true)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                connection.Update(SelectedTask);
                LoadTasksForPanel();
            }
        }

        private bool CanAssignEmployees() => SelectedTask != null;
    }
}
