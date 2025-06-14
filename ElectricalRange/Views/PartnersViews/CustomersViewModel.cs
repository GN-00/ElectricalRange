using Dapper;

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
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.PartnersViews
{
    public class CustomersViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Customer _SelectedItem;
        private ObservableCollection<Customer> _Items;
        private string _CustomerName;
        private string _CustomerNameArabic;
        private string _City;
        private string _Phone;
        private string _VATNumber;
        private string _CR;
        private ICollectionView _ItemsCollection;

        public CustomersViewModel(ObservableCollection<Customer> customers = null, IView view = null)
        {
            UserData = Navigation.UserData;
            AccessKeys.Add("CustomerId");
            ViewData = view;
            GetData(customers);
            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Customer>(Edit, CanEdit);
            ContactsCommand = new RelayCommand<Customer>(Contacts, CanAccessContacts);
            DeleteCommand = new RelayCommand<Customer>(Delete, CanDelete);
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            ClosingCommand = new RelayCommand(Closing, CanClosing);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        private User UserData { get; }
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
        public Customer SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Customer> Items
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
        public RelayCommand<Customer> EditCommand { get; }
        public RelayCommand<Customer> ContactsCommand { get; }
        public RelayCommand<Customer> DeleteCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand ClosingCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string CustomerName
        {
            get => _CustomerName;
            set
            {
                if (SetValue(ref _CustomerName, value))
                {
                    FilterProperty = nameof(CustomerName);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string CustomerNameArabic
        {
            get => _CustomerNameArabic;
            set
            {
                if (SetValue(ref _CustomerNameArabic, value))
                {
                    FilterProperty = nameof(CustomerNameArabic);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string City
        {
            get => _City;
            set
            {
                if (SetValue(ref _City, value))
                {
                    FilterProperty = nameof(City);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Phone
        {
            get => _Phone;
            set
            {
                if (SetValue(ref _Phone, value))
                {
                    FilterProperty = nameof(Phone);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string VATNumber
        {
            get => _VATNumber;
            set
            {
                if (SetValue(ref _VATNumber, value))
                {
                    FilterProperty = nameof(VATNumber);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string CR
        {
            get => _CR;
            set
            {
                if (SetValue(ref _CR, value))
                {
                    FilterProperty = nameof(CR);
                    ItemsCollection.Refresh();
                }
            }
        }

        private bool DataFilter(object customer)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{customer.GetType().GetProperty(columnName).GetValue(customer)}".ToUpper();
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

        private void GetData(ObservableCollection<Customer> customers = null)
        {
            if (customers == null)
            {
                using SqlConnection connection = new(Data.Database.ConnectionString);
                Items = new ObservableCollection<Customer>(CustomerController.GetCustomers(connection));
            }
            else
            {
                Items = customers;
            }
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("CustomerName", ListSortDirection.Ascending));
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
            Customer customer = new();
            Navigation.To(new CustomerView(customer, Items), ViewData);
        }
        private bool CanAdd()
        {
            return UserData.ModifyCustomers;
        }

        private void Edit(Customer customer)
        {
            if (UserData.Access(customer))
            {
                Navigation.To(new CustomerView(customer, Items), ViewData);
            }
        }
        private bool CanEdit(Customer customer)
        {
            if (customer == null)
                return false;

            if (!UserData.ModifyCustomers)
                return false;

            return true;
        }

        private void Contacts(Customer customer)
        {
            Navigation.To(new ContactsView(customer), ViewData);
        }
        private bool CanAccessContacts(Customer customer)
        {
            if (customer == null)
                return false;

            if (!UserData.AccessCustomersContacts)
                return false;

            return true;
        }

        private void Delete(Customer customer)
        {
            if (UserData.Access(customer))
            {
                MessageBoxResult result = MessageView.Show($"Delete",
                                                             $"Are you sure want to Delete\n{customer.CustomerName}?",
                                                             MessageViewButton.YesNo,
                                                             MessageViewImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using SqlConnection connection = new(Database.ConnectionString);
                    if (customer.AbilityToDelete(connection))
                    {
                        _ = connection.Execute($"Delete From [Customer].[Customers] Where CustomerID = {customer.CustomerID} ");
                        _ = connection.Execute($"Delete From [Customer].[Contacts] Where CustomerID = {customer.CustomerID} ");

                        _ = Items.Remove(customer);
                    }
                    else
                    {
                        _ = MessageView.Show("Deleting", $"You can't delete {customer.CustomerName} data \n Because its used in projects data", MessageViewButton.OK, MessageViewImage.Warning);
                    }
                }

                UserData.Exist(customer);
            }
        }

        private bool CanDelete(Customer customer)
        {
            if (customer == null)
                return false;

            if (!UserData.ModifyCustomers)
                return false;

            return true;
        }

        private void Refresh()
        {
            ItemsCollection.Refresh();
        }
        private bool CanRefresh()
        {
            return ItemsCollection != null;
        }

        private void Closing()
        {
            using SqlConnection connection = new(Data.Database.ConnectionString);
            UserData.CustomerId = null;
            UserController.UpdateCustomerID(connection, UserData);
        }
        private bool CanClosing()
        {
            return true;
        }
    }
}