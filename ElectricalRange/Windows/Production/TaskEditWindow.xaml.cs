
using ProjectsNow.Data.Production;
using System.Collections.Generic;
using System.Windows;

namespace ProjectsNow.Views.Production
{
    public partial class TaskEditWindow : Window
    {
        public TaskEditWindow(Task task, List<ProductionPanel> panels)
        {
            InitializeComponent();
            DataContext = new TaskEditViewModel(task, panels);
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

    public class TaskEditViewModel : ViewModelBase
    {
        private Task _task;
        private List<ProductionPanel> _panels;

        public TaskEditViewModel(Task task, List<ProductionPanel> panels)
        {
            _task = task;
            _panels = panels;
        }

        public string TaskName
        {
            get => _task.TaskName;
            set
            {
                _task.TaskName = value;
                OnPropertyChanged();
            }
        }

        public int PanelId
        {
            get => _task.PanelId;
            set
            {
                _task.PanelId = value;
                OnPropertyChanged();
            }
        }

        public DateTime? StartDate
        {
            get => _task.StartDate;
            set
            {
                _task.StartDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? EndDate
        {
            get => _task.EndDate;
            set
            {
                _task.EndDate = value;
                OnPropertyChanged();
            }
        }

        public int Progress
        {
            get => _task.Progress;
            set
            {
                _task.Progress = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _task.Status;
            set
            {
                _task.Status = value;
                OnPropertyChanged();
            }
        }

        public string AssignedEmployees
        {
            get => _task.AssignedEmployees;
            set
            {
                _task.AssignedEmployees = value;
                OnPropertyChanged();
            }
        }

        public List<ProductionPanel> Panels => _panels;
    }
}
