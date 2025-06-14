using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Users;
using ProjectsNow.Views;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjectsNow.Windows.UserWindows
{
    public class UsersViewModel : ViewModelBase
    {
        private string _Code;
        private string _Name;

        private Visibility _IsUpdating = Visibility.Collapsed;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private User _SelectedItem;
        private ObservableCollection<User> _Items;

        public UsersViewModel()
        {
            UserData = Navigation.UserData;
            GetData();
            CreateCollectionView();
            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<User>(Edit, CanEdit);
            PermissionsCommand = new RelayCommand<User>(Permissions, CanPermissions);
            DeleteCommand = new RelayCommand<User>(Delete, CanDelete);
            DeleteFilterCommand = new RelayCommand(DeleteFilter, CanDeleteFilter);
            CompanyWatermarkCommand = new RelayCommand(CompanyWatermark, CanAccessCompanyWatermark);

            AttachCommand = new RelayCommand<User>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<User>(DeleteAttachment, CanAccessDeleteAttachment);
            ReadAttachmentCommand = new RelayCommand<User>(ReadAttachment, CanAccessReadAttachment);
        }

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [User].[Users(View)] " +
                           $"Order By Name";
            Items = new ObservableCollection<User>(connection.Query<User>(query));
        }
        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.Filter = new Predicate<object>(DataFilter);
            ItemsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            ItemsView.CollectionChanged += CollectionChanged;
        }
        private bool DataFilter(object item)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = "" + GetType().GetProperty(columnName).GetValue(this);

            if (!value.Contains(checkValue.ToUpper()))
            {
                result = false;
            }

            return result;
        }

        private void Add()
        {
            UserInfoWindow userInfoWindow = new() { UserData = null, UsersData = Items };
            userInfoWindow.ShowDialog();
        }
        private bool CanAdd()
        {
            return true;
        }

        private void Edit(User user)
        {
            UserInfoWindow userInfoWindow = new() { UserData = user };
            userInfoWindow.ShowDialog();
        }
        private bool CanEdit(User user)
        {
            if (user == null)
                return false;

            return true;
        }

        private void Permissions(User user)
        {
            UserWindow userWindow = new() { UserData = user };
            userWindow.ShowDialog();
        }
        private bool CanPermissions(User user)
        {
            if (user == null)
                return false;

            return true;
        }

        private void Delete(User user)
        {
            MessageBoxResult result = MessageWindows.MessageWindow.Show("Delete", $"Are you sure want to Delete\n{user.Name}?", MessageWindows.MessageWindowButton.YesNo, MessageWindows.MessageWindowImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Delete(user);
                }
                Items.Remove(user);
            }
        }
        private bool CanDelete(User user)
        {
            if (user == null)
                return false;

            return true;
        }


        private void DeleteFilter()
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                FilterProperty attribute = (FilterProperty)property.GetCustomAttribute(typeof(FilterProperty));
                if (attribute != null)
                {
                    property.SetValue(this, null);
                }
            }
            ItemsView.Refresh();
        }
        private bool CanDeleteFilter()
        {
            return true;
        }

        private void CompanyWatermark()
        {
            IsUpdating = Visibility.Visible;
            Events.ShowEvent.Do();

            if (AppData.CompanyData.WatermarkId == null)
            {
                CompanyAttachment attachment = new()
                {
                    CompanyId = AppData.CompanyData.Id,
                };

                if (Attachment.SaveFile<CompanyAttachment>(attachment, "png"))
                {
                    AppData.CompanyData.WatermarkId = attachment.Id;
                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        _ = connection.Update(AppData.CompanyData);
                    }

                    AppData.UserWatermark = (BitmapSource)new ImageSourceConverter().ConvertFrom(attachment.Data);
                }
            }
            else
            {
                CompanyAttachment attachment = new()
                {
                    Id = AppData.CompanyData.WatermarkId.GetValueOrDefault(),
                    CompanyId = AppData.CompanyData.Id,
                };

                if (Attachment.UpdateFile<CompanyAttachment>(attachment, "png"))
                {
                    AppData.UserWatermark = (BitmapSource)new ImageSourceConverter().ConvertFrom(attachment.Data);
                }
            }

            IsUpdating = Visibility.Collapsed;
        }
        private bool CanAccessCompanyWatermark()
        {
            return true;
        }

        private void Attach(User user)
        {
            IsUpdating = Visibility.Visible;
            Events.ShowEvent.Do();

            if (user.WatermarkId == null)
            {
                UserAttachment attachment = new()
                {
                    UserId = user.Id,
                };

                if (Attachment.SaveFile<UserAttachment>(attachment, "png"))
                {
                    user.WatermarkId = attachment.Id;
                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        _ = connection.Update(user);
                    }

                    AppData.UserWatermark = (BitmapSource)new ImageSourceConverter().ConvertFrom(attachment.Data);
                }
            }
            else
            {
                UserAttachment attachment = new()
                {
                    Id = user.WatermarkId.GetValueOrDefault(),
                    UserId = user.Id,
                };

                if (Attachment.UpdateFile<UserAttachment>(attachment, "png"))
                {
                    AppData.UserWatermark = (BitmapSource)new ImageSourceConverter().ConvertFrom(attachment.Data);
                }
            }

            IsUpdating = Visibility.Collapsed;
        }
        private bool CanAccessAttach(User user)
        {
            if (user == null)
                return false;

            return true;
        }

        private void DeleteAttachment(User user)
        {
            UserAttachment attachment = new()
            {
                Id = user.WatermarkId.GetValueOrDefault(),
                UserId = user.Id,
            };

            Attachment.DeleteFile<UserAttachment>(attachment);

            user.WatermarkId = null;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Update(user);
            }

            AppData.UserWatermark = null;
        }
        private bool CanAccessDeleteAttachment(User user)
        {
            if (user == null)
                return false;

            if (user.WatermarkId == null)
                return false;

            return true;
        }

        private void ReadAttachment(User user)
        {
            UserAttachment attachment = new()
            {
                Id = user.WatermarkId.GetValueOrDefault(),
                UserId = user.Id,
            };

            Attachment.OpenFile<UserAttachment>(attachment);
        }
        private bool CanAccessReadAttachment(User user)
        {
            if (user == null)
                return false;

            if (user.WatermarkId == null)
                return false;

            return true;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsView);
        }

        public Visibility IsUpdating
        {
            get => _IsUpdating;
            set => SetValue(ref _IsUpdating, value);
        }
        public User UserData { get; }
        public string Indicator
        {
            get => _Indicator;
            set => SetValue(ref _Indicator, value);
        }
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                if (SetValue(ref _SelectedIndex, value))
                {
                    UpdateIndicator();
                }
            }
        }
        public User SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<User> Items
        {
            get => _Items;
            private set => SetValue(ref _Items, value);
        }
        public ICollectionView ItemsView { get; set; }

        public RelayCommand AddCommand { get; }
        public RelayCommand<User> EditCommand { get; }
        public RelayCommand<User> PermissionsCommand { get; }
        public RelayCommand<User> DeleteCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }
        public RelayCommand CompanyWatermarkCommand { get; }
        public RelayCommand<User> AttachCommand { get; }
        public RelayCommand<User> DeleteAttachmentCommand { get; }
        public RelayCommand<User> ReadAttachmentCommand { get; }


        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string Code
        {
            get => _Code;
            set
            {
                if (SetValue(ref _Code, value))
                {
                    FilterProperty = nameof(Code);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Name
        {
            get => _Name;
            set
            {
                if (SetValue(ref _Name, value))
                {
                    FilterProperty = nameof(Name);
                    ItemsView.Refresh();
                }
            }
        }
    }
}