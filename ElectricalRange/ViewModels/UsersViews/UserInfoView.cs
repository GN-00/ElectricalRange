using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Windows.MessageWindows;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.ViewModels.UsersViews
{
    public class UserInfoView : Base
    {
        private Actions _actions;

        public UserInfoView(User user, ObservableCollection<User> users = null)
        {
            UserData = user;
            UsersData = users;
            if (UserData == null)
            {
                _actions = Actions.New;
                UserData = new User();
            }
            else
            {
                _actions = Actions.Edit;
            }

            NewUser = new User();
            NewUser.Update(UserData);
            SaveCommand = new RelayCommand<object>(Save, CanSave);
            CloseCommand = new RelayCommand<object>(Close, CanClose);
        }


        private void Save(object item)
        {
            bool isReady = true;
            string message = "Please Check:";

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (string.IsNullOrWhiteSpace(NewUser.Name)) { isReady = false; message += $"\n*Name."; }

                string query = $"Select * From [User].[Users] Where Name = '{NewUser.Name}' And Id <> {NewUser.Id} ";
                User checkName = connection.QueryFirstOrDefault<User>(query);
                if (checkName != null) { isReady = false; message += $"\n*The name is already taken!."; }

                if (string.IsNullOrWhiteSpace(NewUser.Code)) { isReady = false; message += $"\n*Code."; }

                query = $"Select * From [User].[Users] Where Code = '{NewUser.Code}' And Id <> {NewUser.Id} ";
                User checkCode = connection.QueryFirstOrDefault<User>(query);
                if (checkCode != null) { isReady = false; message += $"\n*The code is already taken!."; }

                if (string.IsNullOrWhiteSpace(NewUser.Username)) { isReady = false; message += $"\n*User Name."; }
                if (NewUser.Password.Length < 8)
                {
                    isReady = false;
                    message += $"\n*Password must be 8 characters or more!.";
                }
                if (!NewUser.Password.Equals(ConfirmPassword))
                {
                    isReady = false;
                    message += $"\n*Password is not the same confirmed password!.";
                }


                if (isReady)
                {
                    if (_actions == Actions.New)
                    {
                        if (UsersData != null)
                        {
                            Employee employee = new()
                            {
                                EmployeeName = NewUser.Name
                            };

                            employee.EmployeeJob = "";
                            if (NewUser.IsSalesman)
                            {
                                employee.EmployeeJob += "Salesman, ";
                            }
                            if (NewUser.IsEstimation)
                            {
                                employee.EmployeeJob += "Estimation, ";
                            }

                            _ = connection.Insert(employee);
                            NewUser.EmployeeId = employee.EmployeeID;

                            _ = connection.Insert(NewUser);
                            UsersData.Add(NewUser);
                        }
                    }
                    else
                    {
                        Employee employee = new()
                        {
                            EmployeeID = NewUser.EmployeeId,
                            EmployeeName = NewUser.Name
                        };

                        employee.EmployeeJob = "";
                        if (NewUser.IsSalesman)
                        {
                            employee.EmployeeJob += "Salesman, ";
                        }
                        if (NewUser.IsEstimation)
                        {
                            employee.EmployeeJob += "Estimation, ";
                        }

                        _ = connection.Update(employee);
                        _ = connection.Update(NewUser);
                        UserData.Update(NewUser);
                    }
                }
            }

            if (!isReady)
            {
                MessageWindow.Show("Error", message, MessageWindowButton.OK, MessageWindowImage.Warning);
            }

            if (WindowData != null && isReady)
            {
                WindowData.Close();
            }
        }
        private bool CanSave(object item)
        {
            return true;
        }

        private void Close(object item)
        {
            if (WindowData != null)
            {
                WindowData.Close();
            }
        }
        private bool CanClose(object item)
        {
            return true;
        }

        public User UserData { get; set; }
        public ObservableCollection<User> UsersData { get; set; }
        public User NewUser { get; set; }
        public string ConfirmPassword { get; set; }
        public Window WindowData { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand CloseCommand { get; }
    }
}
