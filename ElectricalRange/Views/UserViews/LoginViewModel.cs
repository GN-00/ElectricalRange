using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.References;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.Dashboard;

using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjectsNow.Views.UserViews
{
    public class LoginViewModel : ViewModelBase
    {
        private string _Username;
        private string _Password;
        private User _UserData;
        private Setting _SettingData;
        private string _Version = AppData.Version;
        private string _VersionInfo = $"Checking for update...";

        public LoginViewModel()
        {
            GetData();

            if (SettingData != null)
            {
                Username = SettingData.Username;
                Password = SettingData.Password;
            }

            LoginCommand = new RelayCommand(Login);
        }

        public string Version
        {
            get => _Version;
            set => SetValue(ref _Version, value);
        }

        public string VersionInfo
        {
            get => $"{Version} {_VersionInfo}";
            set => SetValue(ref _VersionInfo, value);
        }

        public string Username
        {
            get => _Username;
            set => SetValue(ref _Username, value);
        }
        public string Password
        {
            get => _Password;
            set => SetValue(ref _Password, value);
        }
        public User UserData
        {
            get => _UserData;
            set => SetValue(ref _UserData, value);
        }
        public Setting SettingData
        {
            get => _SettingData;
            set => SetValue(ref _SettingData, value);
        }
        public RelayCommand LoginCommand { get; private set; }

        public void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [User].[Computers] " +
                           $"Where [Name] ='{AppData.ComputerName}'";

            SettingData = connection.QueryFirstOrDefault<Setting>(query);

            query = "Select * From [Application].[Companies] Where Id = 1 ";
            AppData.CompanyData = connection.QueryFirstOrDefault<Company>(query);

            if (AppData.CompanyData.WatermarkId != null)
            {
                byte[] data;
                query = $"Select Data From [Application].[CompaniesAttachments] Where Id = {AppData.CompanyData.WatermarkId}";
                data = connection.QueryFirstOrDefault<byte[]>(query);
                AppData.CompanyWatermark = (BitmapSource)new ImageSourceConverter().ConvertFrom(data);
            }

            Task.Run(() =>
            {
                using SqlConnection connection = new(Database.ConnectionString);
                AppData.ReferencesListData =
                    new ObservableCollection<Reference>(ReferenceController.GetReferences(connection));


            });


            if (SettingData != null)
            {
                Username = SettingData.Username;
                Password = SettingData.Password;
            }
        }

        public void Login()
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [User].[Users(View)] " +
                               $"Where Username ='{Username}' " +
                               $"And Password ='{Password}'";

                UserData = connection.QueryFirstOrDefault<User>(query);
            }

            if (UserData != null)
            {
                if (SettingData == null)
                {
                    SettingData = new Setting()
                    {
                        UserId = UserData.Id,
                        Name = AppData.ComputerName,
                        ComputerName = AppData.ComputerName,
                        Username = Username,
                        Password = Password,
                    };

                    using SqlConnection connection = new(Data.Database.ConnectionString);
                    _ = connection.Insert(SettingData);
                }
                else
                {
                    SettingData.UserId = UserData.Id;
                    SettingData.ComputerName = AppData.ComputerName;
                    SettingData.Username = Username;
                    SettingData.Password = Password;

                    using SqlConnection connection = new(Data.Database.ConnectionString);
                    _ = connection.Update(SettingData);
                }

                if (UserData.WatermarkId != null)
                {
                    byte[] data;
                    string query = $"Select Data From [User].[Attachments] Where Id = {UserData.WatermarkId}";
                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        data = connection.QueryFirstOrDefault<byte[]>(query);
                    }
                    AppData.UserWatermark = (BitmapSource)new ImageSourceConverter().ConvertFrom(data);
                }

                Navigation.UserData = UserData;
                Navigation.SetUsername(UserData.Name);
                Navigation.To(new DashboardView());
            }
            else
            {
                _ = MessageView.Show("Login Error", "Incorrect username or password", MessageViewButton.OK, MessageViewImage.Warning);
            }
        }
    }
}