using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.UserWindows;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;


namespace ProjectsNow.ViewModels.UsersViews
{
    public class UsersView : Base
    {
        private User _user;
        private string _Code;
        private string _Name;

        private string _Indicator = "-";
        private int _SelectedIndex;
        private User _SelectedItem;
        private ObservableCollection<User> _Items;

        public UsersView(User user)
        {
            _user = user;
            GetData();
            CreateCollectionView();
            AddCommand = new RelayCommand<object>(Add, CanAdd);
            EditCommand = new RelayCommand<object>(Edit, CanEdit);
            PermissionsCommand = new RelayCommand<object>(Permissions, CanPermissions);
            DeleteCommand = new RelayCommand<object>(Delete, CanDelete);
            DeleteFilterCommand = new RelayCommand<object>(DeleteFilter, CanDeleteFilter);
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

        private void Add(object item)
        {
            UserInfoWindow userInfoWindow = new() { UserData = null, UsersData = Items };
            userInfoWindow.ShowDialog();
        }
        private bool CanAdd(object item)
        {
            return true;
        }

        private void Edit(object item)
        {
            UserInfoWindow userInfoWindow = new() { UserData = SelectedItem };
            userInfoWindow.ShowDialog();
        }
        private bool CanEdit(object item)
        {
            return SelectedItem != null;
        }

        private void Permissions(object item)
        {
            if (SelectedItem is User user)
            {
                UserWindow userWindow = new() { UserData = user };
                userWindow.ShowDialog();
            }
        }
        private bool CanPermissions(object item)
        {
            return SelectedItem != null;
        }

        private void Delete(object item)
        {
            if (SelectedItem is User user)
            {
                MessageBoxResult result = Windows.MessageWindows.MessageWindow.Show("Delete", $"Are you sure want to Delete\n{user.Name}?", Windows.MessageWindows.MessageWindowButton.YesNo, Windows.MessageWindows.MessageWindowImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        _ = connection.Delete(user);
                    }
                    Items.Remove(user);
                }
            }
        }
        private bool CanDelete(object item)
        {
            return SelectedItem != null;
        }


        private void DeleteFilter(object item)
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
        private bool CanDeleteFilter(object item)
        {
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
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand PermissionsCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DeleteFilterCommand { get; }

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
