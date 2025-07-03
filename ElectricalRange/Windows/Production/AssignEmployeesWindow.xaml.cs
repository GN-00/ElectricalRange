
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
    public partial class AssignEmployeesWindow : Window
    {
        public AssignEmployeesWindow(Task task)
        {
            InitializeComponent();
            DataContext = new AssignEmployeesViewModel(task);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class AssignEmployeesViewModel : ViewModelBase
    {
        private Task _task;
        private ObservableCollection<Employee> _availableEmployees;
        private ObservableCollection<Employee> _assignedEmployees;
        private Employee _selectedAvailableEmployee;
        private Employee _selectedAssignedEmployee;

        public AssignEmployeesViewModel(Task task)
        {
            _task = task;
            LoadEmployees();

            AddEmployeeCommand = new RelayCommand(AddEmployee, CanAddEmployee);
            RemoveEmployeeCommand = new RelayCommand(RemoveEmployee, CanRemoveEmployee);
        }

        public string TaskName => _task.TaskName;

        public ObservableCollection<Employee> AvailableEmployees
        {
            get => _availableEmployees;
            set => SetValue(ref _availableEmployees, value);
        }

        public ObservableCollection<Employee> AssignedEmployees
        {
            get => _assignedEmployees;
            set => SetValue(ref _assignedEmployees, value);
        }

        public Employee SelectedAvailableEmployee
        {
            get => _selectedAvailableEmployee;
            set => SetValue(ref _selectedAvailableEmployee, value);
        }

        public Employee SelectedAssignedEmployee
        {
            get => _selectedAssignedEmployee;
            set => SetValue(ref _selectedAssignedEmployee, value);
        }

        public RelayCommand AddEmployeeCommand { get; }
        public RelayCommand RemoveEmployeeCommand { get; }

        private void LoadEmployees()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            
            // Load all employees
            var allEmployees = connection.Query<Employee>("SELECT * FROM [Users].[Employees] WHERE IsActive = 1").ToList();
            
            // Parse currently assigned employees
            var assignedEmployeeNames = string.IsNullOrEmpty(_task.AssignedEmployees) 
                ? new List<string>() 
                : _task.AssignedEmployees.Split(',').Select(x => x.Trim()).ToList();
            
            var assigned = allEmployees.Where(e => assignedEmployeeNames.Contains(e.Name)).ToList();
            var available = allEmployees.Where(e => !assignedEmployeeNames.Contains(e.Name)).ToList();

            AvailableEmployees = new ObservableCollection<Employee>(available);
            AssignedEmployees = new ObservableCollection<Employee>(assigned);
        }

        private void AddEmployee()
        {
            if (SelectedAvailableEmployee == null) return;

            AssignedEmployees.Add(SelectedAvailableEmployee);
            AvailableEmployees.Remove(SelectedAvailableEmployee);
            
            UpdateTaskAssignedEmployees();
        }

        private bool CanAddEmployee() => SelectedAvailableEmployee != null;

        private void RemoveEmployee()
        {
            if (SelectedAssignedEmployee == null) return;

            AvailableEmployees.Add(SelectedAssignedEmployee);
            AssignedEmployees.Remove(SelectedAssignedEmployee);
            
            UpdateTaskAssignedEmployees();
        }

        private bool CanRemoveEmployee() => SelectedAssignedEmployee != null;

        private void UpdateTaskAssignedEmployees()
        {
            _task.AssignedEmployees = string.Join(", ", AssignedEmployees.Select(e => e.Name));
        }
    }
}
