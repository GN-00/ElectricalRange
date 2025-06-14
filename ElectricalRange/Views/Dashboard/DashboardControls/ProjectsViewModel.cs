using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.JobOrdersViews;
using ProjectsNow.Windows.JobOrderWindows;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public class ProjectsViewModel : ViewModelBase
    {
        private double _MaxWidth = 1300;
        private User _UserData;

        public ProjectsViewModel()
        {
            UserData = Navigation.UserData;
            NewCommand = new RelayCommand(New, CanAccessNew);
            JobOrdersCommand = new RelayCommand(JobOrders, CanAccessJobOrders);

            int buttons = 0;
            if (CanAccessNew()) buttons += 1;
            if (CanAccessJobOrders()) buttons += 1;

            if (buttons == 4) MaxWidth = 900;
        }

        public double MaxWidth
        {
            get => _MaxWidth;
            set => SetValue(ref _MaxWidth, value);
        }
        public User UserData
        {
            get => _UserData;
            set => SetValue(ref _UserData, value);
        }

        public RelayCommand NewCommand { get; }
        public RelayCommand JobOrdersCommand { get; }

        public void New()
        {
            Navigation.To(new NewJobOrdersView());
        }
        public bool CanAccessNew()
        {
            return UserData.AccessNewJobOrder;
        }

        public void JobOrders()
        {
            //JobOrdersWindow jobOrdersWindow = new JobOrdersWindow() { UserData = UserData };
            //_ = jobOrdersWindow.ShowDialog();

            Navigation.To(new JobOrdersView());
        }
        public bool CanAccessJobOrders()
        {
            return UserData.AccessJobOrders;
        }
    }
}