using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ProjectsNow.Views.FinanceView
{
    internal class CompanyAccountViewModel : Base
    {
        private Account _NewData;
        public CompanyAccountViewModel(Account accountData, ObservableCollection<Account> accountsData)
        {
            UserData = Navigation.UserData;
            NewData = new Account();
            NewData.Update(accountData);
            AccountData = accountData;
            AccountsData = accountsData;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }
        public Account NewData
        {
            get => _NewData;
            set => SetValue(ref _NewData, value);
        }
        public Window WindowData { get; }
        public User UserData { get; }
        public Account AccountData { get; }
        public ObservableCollection<Account> AccountsData { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            if (NewData.ID == 0)
            {
                if (AccountsData.Any(a => a.Name == NewData.Name))
                {
                    MessageView.Show("Name",
                                     "The name already exists!",
                                     MessageViewButton.OK,
                                     MessageViewImage.Information);
                    return;
                }

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(NewData);
                }

                AccountsData.Add(NewData);
            }
            else
            {
                if (AccountsData.Any(a => a.Name == NewData.Name && a.ID != NewData.ID))
                {
                    MessageView.Show("Name",
                                      "The name already exists!",
                                      MessageViewButton.OK,
                                      MessageViewImage.Information);
                    return;
                }

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Update(NewData);
                }

                AccountData.Update(NewData);
            }

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (!string.IsNullOrWhiteSpace(NewData.Name))
            {
                if (NewData.Type == null)
                    return false;

                return true;
            }
            else
            {
                return false;
            }
        }

        private void Cancel()
        {
            Navigation.ClosePopup();
        }

        private bool CanCancel()
        {
            return true;
        }
    }
}