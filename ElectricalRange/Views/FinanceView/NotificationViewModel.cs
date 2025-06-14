using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.FinanceView
{
    internal class NotificationViewModel : ViewModelBase
    {
        private Notification _NewData;
        public NotificationViewModel(Notification notification, ObservableCollection<Notification> notifications)
        {
            UserData = Navigation.UserData;
            NotificationData = notification;
            NotificationsData = notifications;

            NewData = new Notification();
            NewData.Update(notification);

            if (notification.Id == 0)
            {
                NewData.Amount = notification.Balance;
            }

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public Notification NewData
        {
            get => _NewData;
            set => SetValue(ref _NewData, value);
        }
        public User UserData { get; }
        public Notification NotificationData { get; }
        public ObservableCollection<Notification> NotificationsData { get; }


        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public bool IsEditable => UserData.ModifyNotifyReceipts;

        private void Save()
        {
            if (NewData.Id == 0)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(NewData);
                }

                NotificationsData.Add(NewData);
            }
            else
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Update(NewData);
                }

                NotificationData.Update(NewData);
            }

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (NewData == null)
                return false;

            if (NewData.Date == null)
                return false;

            if (NewData.Code == null)
                return false;

            if (NewData.Amount == null)
                return false;

            if (NewData.Amount <= 0)
                return false;

            if (!UserData.ModifyNotifyReceipts)
                return false;

            return true;
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