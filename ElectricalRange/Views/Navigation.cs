using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Customers;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.Partners;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.References;
using ProjectsNow.Data.Suppliers;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.Dashboard;
using ProjectsNow.Views.UserViews;

using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using JobOrder = ProjectsNow.Data.JobOrders.JobOrder;

namespace ProjectsNow.Views
{
    public static class Navigation
    {
        public static User UserData { get; set; }
        public static DashboardView DashboardViewData { get; set; }
        public static MainViewModel MainPage { get; set; }

        private static Stack<IView> pages = new();

        public static void To(IView page, IView checkPoint = null)
        {
            MainPage.CurrentView = page;
            if (checkPoint != null)
            {
                pages.Push(checkPoint);
                CanBack();
            }

            UpdateHome();
        }
        public static void Back()
        {
            MainPage.CurrentView = pages.Pop();

            ResetCurrentViewAccessKey();

            CanBack();
        }
        public static void CanBack()
        {
            if (pages.Count > 0)
            {
                MainPage.Back = true;
            }
            else
            {
                MainPage.Back = false;
            }
        }

        public static Action ClosePopupEvent { get; set; }
        public static void OpenPopup(IPopup popupView, PlacementMode placement = PlacementMode.Center, bool staysOpen = false)
        {
            MainPage.PopupView = popupView;
            MainPage.OpenPopup = true;
            MainPage.MainPopup.StaysOpen = staysOpen;
            MainPage.MainPopup.Placement = placement;

            ResetCurrentViewAccessKey();
        }
        public static void ClosePopup()
        {
            MainPage.OpenPopup = false;
            ClosePopupEvent?.Invoke();
            ClosePopupEvent = null;
        }

        public static void OpenLoading(Visibility icon = Visibility.Visible, string Text = null)
        {
            MainPage.LoadingIcon = icon;
            MainPage.LoadingText = Text;
            MainPage.IsLoading = true;
            Events.ShowEvent.Do();
        }

        public static void LoadingText(string Text = null)
        {
            MainPage.LoadingText = Text;
            Events.ShowEvent.Do();
        }

        public static void CloseLoading()
        {
            MainPage.LoadingIcon = Visibility.Collapsed;
            MainPage.LoadingText = null;
            MainPage.IsLoading = false;
        }

        public static void SetUsername(string userName)
        {
            MainPage.Username = userName;
            MainPage.Version = AppData.Version;
        }

        private static void UpdateHome()
        {
            if (MainPage.CurrentView is DashboardView)
            {
                MainPage.Home = MainPage.Back = false;
                pages.Clear();
            }
            else if (MainPage.CurrentView is LoginView)
            {
                MainPage.Home = MainPage.Back = false;
                pages.Clear();
            }
            else
            {
                MainPage.Home = true;
            }
        }

        public static string PropertyAccess(IAccess accessTo)
        {
            if (accessTo is Inquiry)
            {
                return "InquiryId";
            }
            else if (accessTo is Quotation)
            {
                return "QuotationId";
            }
            else if (accessTo is JobOrder)
            {
                return "JobOrderId";
            }
            else if (accessTo is Customer)
            {
                return "CustomerId";
            }
            else if (accessTo is Data.Finance.CustomerAccount)
            {
                return "CustomerId";
            }
            else if (accessTo is Data.Finance.SupplierAccount)
            {
                return "SupplierId";
            }
            else if (accessTo is Supplier)
            {
                return "SupplierId";
            }
            else if (accessTo is Other)
            {
                return "OtherId";
            }
            else if (accessTo is Consultant)
            {
                return "ConsultantId";
            }
            else if (accessTo is Reference)
            {
                return "ReferenceId";
            }
            else if (accessTo is Data.Finance.JobOrder)
            {
                return "AccountantOrderId";
            }
            else if (accessTo is User)
            {
                return "UserId";
            }
            else if (accessTo is Data.Finance.Account)
            {
                return "AccountId";
            }
            else
            {
                return null;
            }
        }

        public static bool Access(this User user, IAccess accessTo)
        {
            bool result = false;
            string property = PropertyAccess(accessTo);

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                PropertyInfo accessId = accessTo.GetType().GetProperty("ID");

                if (accessId == null)
                    accessId = accessTo.GetType().GetProperty("Id");

                if (accessId == null)
                    accessId = accessTo.GetType().GetProperty(property);

                var accessIdValue = accessId.GetValue(accessTo);

                string query = $"Select * From [User].[Users] " +
                               $"Where {property} = {accessIdValue} And Id <> {user.Id}";

                User checkUser = connection.QueryFirstOrDefault<User>(query);

                if (checkUser == null)
                {
                    result = true;
                    user.GetType().GetProperty(property).SetValue(user, accessIdValue);
                    _ = connection.Update(user);
                }
                else
                {
                    _ = MessageView.Show($"Access",
                                         $"{checkUser.Name} workin on it!");
                }
            }
            return result;
        }
        public static void Exist(this User user, IAccess existFrom)
        {
            string property = PropertyAccess(existFrom);
            using SqlConnection connection = new(Database.ConnectionString);
            user.GetType().GetProperty(property).SetValue(user, null);
            _ = connection.Update(user);
        }
        public static void Exist(this User user, string AccessKey)
        {
            using SqlConnection connection = new(Database.ConnectionString);
            user.GetType().GetProperty(AccessKey).SetValue(user, null);
            _ = connection.Update(user);
        }
        public static void ResetAccessKeys()
        {
            if (UserData == null)
                return;

            using SqlConnection connection = new(Database.ConnectionString);
            UserData.InquiryId = null;
            UserData.QuotationId = null;
            UserData.JobOrderId = null;
            UserData.CustomerId = null;
            UserData.SupplierId = null;
            UserData.OtherId = null;
            UserData.ConsultantId = null;
            UserData.ReferenceId = null;
            UserData.AccountId = null;
            UserData.AccountantOrderId = null;
            UserData.UserId = null;
            _ = connection.Update(UserData);
        }

        public static void ResetCurrentViewAccessKey()
        {
            var data = (ViewModelBase)((UserControl)MainPage.CurrentView).DataContext;

            if (data.AccessKeys.Count != 0)
            {
                foreach (string key in data.AccessKeys)
                    UserData.Exist(key);
            }
        }
    }
}
