using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Customers;
using ProjectsNow.Data.Inquiries;
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
    public class ConsultantsViewModel : ViewModelBase
    {
        private bool _IsWriting = false;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Consultant _NewData = new();
        private Consultant _SelectedItem;
        private ObservableCollection<Consultant> _Items;
        private string _ConsultantName;
        private string _Job;
        private ICollectionView _ItemsCollection;

        public ConsultantsViewModel()
        {
            LoadingText = "Updating....";
            UserData = Navigation.UserData;
            GetData();
            if (Items.Count != 0)
            {
                SelectedItem = Items[0];
            }
            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Consultant>(Edit, CanEdit);
            DeleteCommand = new RelayCommand<Consultant>(Delete, CanDelete);
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
        public Consultant NewData
        {
            get => _NewData;
            set => SetValue(ref _NewData, value);
        }
        public Consultant SelectedItem
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
        public ObservableCollection<Consultant> Items
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
        public RelayCommand<Consultant> EditCommand { get; }
        public RelayCommand<Consultant> DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string ConsultantName
        {
            get => _ConsultantName;
            set
            {
                if (SetValue(ref _ConsultantName, value))
                {
                    FilterProperty = nameof(ConsultantName);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Job
        {
            get => _Job;
            set
            {
                if (SetValue(ref _Job, value))
                {
                    FilterProperty = nameof(Job);
                    ItemsCollection.Refresh();
                }
            }
        }

        private bool DataFilter(object consultant)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{consultant.GetType().GetProperty(columnName).GetValue(consultant)}".ToUpper();
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
            Items = new ObservableCollection<Consultant>(ConsultantController.GetConsultants(connection));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("ConsultantName", ListSortDirection.Ascending));
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
            NewData = new Consultant();
            IsWriting = true;
        }
        private bool CanAdd()
        {
            if (!UserData.ModifyConsultants)
                return false;

            if (IsWriting)
                return false;

            return true;
        }

        private void Edit(Consultant consultant)
        {
            IsWriting = true;
        }
        private bool CanEdit(Consultant consultant)
        {
            if (consultant == null)
                return false;

            if (!UserData.ModifyConsultants)
                return false;

            if (IsWriting)
                return false;

            return true;
        }

        private void Delete(Consultant consultant)
        {
            MessageBoxResult result =
                MessageView.Show($"Delete",
                                 $"Are you sure want to Delete\n{consultant.ConsultantName}?",
                                 MessageViewButton.YesNo,
                                 MessageViewImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Inquiry checkConsultant;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Select * From [Inquiry].[Inquiries] " +
                                   $"Where ConsultantID = {consultant.ConsultantID} ";

                    checkConsultant = connection.QueryFirstOrDefault<Inquiry>(query);
                }

                if (checkConsultant != null)
                {
                    MessageView.Show("Delete",
                                       "Can't delete this Consultant!\nHis data are used in orders!",
                                       MessageViewButton.OK,
                                       MessageViewImage.Warning);

                    return;
                }

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Delete(consultant);
                }
                Items.Remove(consultant);

                if (Items.Count > 0)
                {
                    SelectedItem = Items[0];
                }
            }
        }
        private bool CanDelete(Consultant consultant)
        {
            return CanEdit(consultant);
        }

        private void Save()
        {
            bool isReady = true;
            string message = "Please Enter:";
            if (string.IsNullOrWhiteSpace(NewData.ConsultantName)) { message += $"\n  Name."; isReady = false; }
            else if (Items.Any(i => i.ConsultantName.ToLower() == NewData.ConsultantName.ToLower() && i.ConsultantID != NewData.ConsultantID))
            { message += $"\n  Name is already exist."; isReady = false; }
            if (string.IsNullOrWhiteSpace(NewData.Mobile)) { message += $"\n  Mobile."; isReady = false; }

            if (isReady)
            {
                if (NewData.ConsultantID == 0)
                {
                    Consultant consultant = new();
                    consultant.Update(NewData);
                    using (SqlConnection connection = new(Data.Database.ConnectionString))
                    {
                        _ = connection.Insert(consultant);
                    }

                    Items.Add(consultant);

                    SelectedItem = consultant;
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