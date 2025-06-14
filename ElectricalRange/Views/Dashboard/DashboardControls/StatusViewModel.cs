using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.QuotationsStatusViews;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public class StatusViewModel : ViewModelBase
    {
        private double _MaxWidth = 1300;
        private User _UserData;

        public StatusViewModel()
        {
            UserData = Navigation.UserData;
            ProjectsCommand = new RelayCommand(Projects, CanAccessProjects);
            QuotationsCommand = new RelayCommand(Quotations, CanAccessQuotations);

            int buttons = 0;
            if (CanAccessProjects()) buttons += 1;
            if (CanAccessQuotations()) buttons += 1;

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

        public RelayCommand ProjectsCommand { get; }
        public RelayCommand QuotationsCommand { get; }

        public void Projects()
        {
            Navigation.To(new ProjectsStatusView());
        }
        public bool CanAccessProjects()
        {
            return true;
        }

        public void Quotations()
        {
            Navigation.To(new QuotationsReportView());
        }
        public bool CanAccessQuotations()
        {
            return true;
        }
    }
}