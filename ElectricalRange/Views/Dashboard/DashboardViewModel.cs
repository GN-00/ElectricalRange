using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.Dashboard.DashboardControls;
using ProjectsNow.Views.UserViews;

using System;
using Microsoft.Data.SqlClient;
using System.Windows.Controls;

namespace ProjectsNow.Views.Dashboard
{
    internal class DashboardViewModel : ViewModelBase
    {
        private UserControl _CurrentView;
        public DashboardViewModel(IView view)
        {
            ViewData = view;

            GetData();

            TendaringCommand = new RelayCommand(Tendaring, CanAccessTendaring);
            ProjectsCommand = new RelayCommand(Projects, CanAccessProjects);
            QuotationsStatusCommand = new RelayCommand(QuotationsStatus, CanAccessQuotationsStatus);
            StoreCommand = new RelayCommand(Store, CanAccessStore);
            FinanceCommand = new RelayCommand(Finance, CanAccessFinance);
            UsersCommand = new RelayCommand(Users, CanAccessUsers);
            PartnersCommand = new RelayCommand(Partners, CanAccessPartners);
            ProductionCommand = new RelayCommand(Production, CanAccessProduction);
            LogoutCommand = new RelayCommand(Logout, CanAccessLogout);
        }

        public User UserData => Navigation.UserData;
        public UserControl CurrentView
        {
            get => _CurrentView;
            set => SetValue(ref _CurrentView, value);
        }
        public RelayCommand TendaringCommand { get; }
        public RelayCommand ProjectsCommand { get; }
        public RelayCommand QuotationsStatusCommand { get; }
        public RelayCommand StoreCommand { get; }
        public RelayCommand FinanceCommand { get; }
        public RelayCommand UsersCommand { get; }
        public RelayCommand PartnersCommand { get; }
        public RelayCommand ProductionCommand { get; }
        public RelayCommand LogoutCommand { get; }

        private void GetData()
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                DateTime date = DateTime.Now;
                AppData.VAT = connection.QueryFirstOrDefault<decimal>($"Select MAX(VAT) AS VAT From [Finance].[VAT] Where Date <= @date", date);
            }

            if (UserData.AccessTendaring)
                Tendaring();
            else if (UserData.AccessProjects)
                Projects();
            else if (UserData.AccessQuotationsStatus)
                QuotationsStatus();
            else if (UserData.AccessStore)
                Store();
            else if (UserData.AccessFinance)
                Finance();
            else if (UserData.AccessPartners)
                Partners();
        }


        private void Tendaring()
        {
            CurrentView = new TendaringControl();
        }
        private bool CanAccessTendaring()
        {
            return UserData.AccessTendaring;
        }

        private void Projects()
        {
            CurrentView = new ProjectsControl();
        }
        private bool CanAccessProjects()
        {
            return UserData.AccessProjects;
        }

        private void QuotationsStatus()
        {
            CurrentView = new StatusControl();
        }
        private bool CanAccessQuotationsStatus()
        {
            return UserData.AccessQuotationsStatus;
        }

        private void Store()
        {
            CurrentView = new StoreControl();
        }
        private bool CanAccessStore()
        {
            return UserData.AccessStore;
        }

        private void Finance()
        {
            CurrentView = new FinanceControl();
        }
        private bool CanAccessFinance()
        {
            return UserData.AccessFinance;
        }

        private void Users()
        {
            Windows.UserWindows.UsersWindow usersWindow = new();
            usersWindow.ShowDialog();
        }
        private bool CanAccessUsers()
        {
            return UserData.Administrator;
        }

        private void Partners()
        {
            CurrentView = new PartnersControl();
        }
        private bool CanAccessPartners()
        {
            return UserData.AccessPartners;
        }

        private void Production()
        {
            CurrentView = new ProductionControl();
        }
        private bool CanAccessProduction()
        {
            if (UserData == null)
                return false;
            if (UserData.EmployeeId != 0)
                return false;

            return true;
        }

        private void Logout()
        {
            Navigation.To(new LoginView());
        }

        private bool CanAccessLogout()
        {
            return true;
        }
    }
}