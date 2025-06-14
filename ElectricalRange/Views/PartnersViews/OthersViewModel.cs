using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Partners;
using ProjectsNow.Data.Users;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Views.PartnersViews
{
    public class OthersViewModel : ViewModelBase
    {
        private bool _IsWriting = false;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Other _NewData = new();
        private Other _SelectedItem;
        private ObservableCollection<Other> _Items;
        private string _Name;
        private ICollectionView _ItemsCollection;

        public OthersViewModel()
        {
            LoadingText = "Updating....";
            UserData = Navigation.UserData;
            GetData();
            if (Items.Count != 0)
            {
                SelectedItem = Items[0];
            }
            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Other>(Edit, CanEdit);
            DeleteCommand = new RelayCommand<Other>(Delete, CanDelete);
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData { get; }
        public bool IsWriting
        {
            get => _IsWriting;
            set => SetValue(ref _IsWriting, value);
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
        public Other NewData
        {
            get => _NewData;
            set => SetValue(ref _NewData, value);
        }
        public Other SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (SetValue(ref _SelectedItem, value))
                {
                    if (SelectedItem != null)
                        NewData.Update(SelectedItem);
                }
            }
        }
        public ObservableCollection<Other> Items
        {
            get => _Items;
            private set
            {
                if (SetValue(ref _Items, value))
                {
                    CreateCollectionView();
                }
            }
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }
        public RelayCommand AddCommand { get; }
        public RelayCommand<Other> EditCommand { get; }
        public RelayCommand<Other> DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string Name
        {
            get => _Name;
            set
            {
                if (SetValue(ref _Name, value))
                {
                    FilterProperty = nameof(Name);
                    ItemsCollection.Refresh();
                }
            }
        }

        private bool DataFilter(object other)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{other.GetType().GetProperty(columnName).GetValue(other)}".ToUpper();
            string checkValue = "" + GetType().GetProperty(columnName).GetValue(this);

            if (!value.Contains(checkValue.ToUpper()))
            {
                result = false;
            }

            return result;
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
            ItemsCollection.Refresh();
        }

        #endregion

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = "Select * From [Partner].[Others] ";
            Items = new ObservableCollection<Other>(connection.Query<Other>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }

        private void Add()
        {
            NewData = new Other();
            IsWriting = true;
        }
        private bool CanAdd()
        {
            if (!UserData.ModifyOthers)
                return false;

            if (IsWriting)
                return false;

            return true;
        }

        private void Edit(Other other)
        {
            IsWriting = true;
        }
        private bool CanEdit(Other other)
        {
            if (other == null)
                return false;

            if (!UserData.ModifyOthers)
                return false;

            if (IsWriting)
                return false;

            return true;
        }

        private void Delete(Other other)
        {
            MessageBoxResult result =
                MessageView.Show($"Delete",
                                 $"Are you sure want to Delete\n{other.Name}?",
                                 MessageViewButton.YesNo,
                                 MessageViewImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                //Inquiry checkOther;
                //using (SqlConnection connection = new SqlConnection(Database.ConnectionString))
                //{
                //    string query = $"Select * From [Inquiry].[Inquiries] " +
                //                   $"Where OtherID = {other.OtherID} ";

                //    checkOther = connection.QueryFirstOrDefault<Inquiry>(query);
                //}

                //if (checkOther != null)
                //{
                    //MessageView.Show("Delete",
                    //                   "Can't delete this Other!\nHis data are used in orders!",
                    //                   MessageViewButton.OK,
                    //                   MessageViewImage.Warning);

                    //return;
                //}

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Delete(other);
                }

                Items.Remove(other);

                if (Items.Count > 0)
                {
                    SelectedItem = Items[0];
                }
            }
        }
        private bool CanDelete(Other other)
        {
            return CanEdit(other);
        }

        private void Save()
        {
            bool isReady = true;
            string message = "Please Enter:";
            if (string.IsNullOrWhiteSpace(NewData.Name)) { message += $"\n  Name."; isReady = false; }
            else if (Items.Any(i => i.Name.ToLower() == NewData.Name.ToLower() && i.Id != NewData.Id))
            { message += $"\n  Name is already exist."; isReady = false; }

            if (string.IsNullOrWhiteSpace(NewData.VATNumber)) { message += $"\n  VAT Number."; isReady = false; }
            else if (Items.Any(i => i.VATNumber?.ToLower() == NewData.VATNumber.ToLower() && i.Id != NewData.Id))
            { message += $"\n  VAT Number is already exist."; isReady = false; }
            else if (!NewData.VATNumber.Length.Equals(15)) { message += $"\n  VAT Number Must Be 15 Digit."; isReady = false; }

            if (isReady)
            {
                if (NewData.Id == 0)
                {
                    Other other = new();
                    other.Update(NewData);
                    using (SqlConnection connection = new(Data.Database.ConnectionString))
                    {
                        _ = connection.Insert(other);
                    }

                    Items.Add(other);

                    SelectedItem = other;
                }
                else
                {
                    using (SqlConnection connection = new(Data.Database.ConnectionString))
                    {
                        _ = connection.Update(NewData);
                    }

                    SelectedItem.Update(NewData);

                    ItemsCollection.Refresh();
                }

                IsWriting = false;
            }
            else
            {
                _ = MessageView.Show("Saving", message, MessageViewButton.OK, MessageViewImage.Information);
            }
        }
        private bool CanSave()
        {
            return IsWriting;
        }

        private void Cancel()
        {
            NewData.Update(SelectedItem);
            IsWriting = false;
        }
        private bool CanCancel()
        {
            return IsWriting;
        }
    }
}