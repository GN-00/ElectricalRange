using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Customers;
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
    public class ContactsViewModel : ViewModelBase
    {
        private bool _IsWriting = false;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Contact _NewData = new();
        private Contact _SelectedItem;
        private ObservableCollection<Contact> _Items;
        private string _ContactName;
        private string _Job;
        private ICollectionView _ItemsCollection;

        public ContactsViewModel(Customer customer)
        {
            LoadingText = "Updating....";
            UserData = Navigation.UserData;
            CustomerData = customer;
            GetData();
            if (Items.Count != 0)
            {
                SelectedItem = Items[0];
            }
            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Contact>(Edit, CanEdit);
            DeleteCommand = new RelayCommand<Contact>(Delete, CanDelete);
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData { get; }
        public Customer CustomerData { get; private set; }
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
        public Contact NewData
        {
            get => _NewData;
            set => SetValue(ref _NewData, value);
        }
        public Contact SelectedItem
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
        public ObservableCollection<Contact> Items
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
        public RelayCommand<Contact> EditCommand { get; }
        public RelayCommand<Contact> DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string ContactName
        {
            get => _ContactName;
            set
            {
                if (SetValue(ref _ContactName, value))
                {
                    FilterProperty = nameof(ContactName);
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

        private bool DataFilter(object contact)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{contact.GetType().GetProperty(columnName).GetValue(contact)}".ToUpper();
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
            using SqlConnection connection = new(Data.Database.ConnectionString);
            Items = new ObservableCollection<Contact>(ContactController.GetCustomerContacts(connection, CustomerData.CustomerID));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("ContactName", ListSortDirection.Ascending));
            ItemsCollection.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
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
            NewData = new Contact() { CustomerID = CustomerData.CustomerID };
            IsWriting = true;
        }
        private bool CanAdd()
        {
            if (!UserData.ModifyCustomersContacts)
                return false;

            if (IsWriting)
                return false;

            return true;
        }

        private void Edit(Contact contact)
        {
            IsWriting = true;
        }
        private bool CanEdit(Contact contact)
        {
            if (contact == null)
                return false;

            if (!UserData.ModifyCustomersContacts)
                return false;

            if (IsWriting)
                return false;

            return true;
        }

        private void Delete(Contact contact)
        {
            MessageBoxResult result =
                MessageView.Show($"Delete",
                                 $"Are you sure want to Delete\n{contact.ContactName}?",
                                 MessageViewButton.YesNo,
                                 MessageViewImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Contact checkContact;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Select * From [Inquiry].[ProjectsContacts(View)] " +
                                   $"Where ContactID = {contact.ContactID} ";

                    checkContact = connection.QueryFirstOrDefault<Contact>(query);
                }

                if (checkContact != null)
                {
                    MessageView.Show("Delete",
                                     "Can't delete this contact!\nHis data are used in orders!",
                                     MessageViewButton.OK,
                                     MessageViewImage.Warning);

                    return;
                }


                using (SqlConnection connection = new(Data.Database.ConnectionString))
                {
                    _ = connection.Delete(contact);
                }
                Items.Remove(contact);

                if (Items.Count > 0)
                {
                    SelectedItem = Items[0];
                }
            }
        }
        private bool CanDelete(Contact contact)
        {
            return CanEdit(contact);
        }

        private void Save()
        {
            bool isReady = true;
            string message = "Please Enter:";
            if (string.IsNullOrWhiteSpace(NewData.ContactName)) { message += $"\n  Name."; isReady = false; }
            else if (Items.Any(i => i.ContactName.ToLower() == NewData.ContactName.ToLower() && i.ContactID != NewData.ContactID))
            { message += $"\n  Name is already exist."; isReady = false; }
            if (string.IsNullOrWhiteSpace(NewData.Mobile)) { message += $"\n  Mobile."; isReady = false; }

            if (isReady)
            {
                if (NewData.ContactID == 0)
                {
                    Contact contact = new();
                    contact.Update(NewData);
                    using (SqlConnection connection = new(Data.Database.ConnectionString))
                    {
                        _ = connection.Insert(contact);
                    }

                    Items.Add(contact);

                    SelectedItem = contact;
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