using Dapper;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Suppliers;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.PartnersViews
{
    public class SuppliersViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Supplier _SelectedItem;
        private ObservableCollection<Supplier> _Items;
        private string _Name;
        private string _ArabicName;
        private string _City;
        private string _Phone;
        private string _VATNumber;
        private string _CR;
        private ICollectionView _ItemsCollection;

        public SuppliersViewModel(ObservableCollection<Supplier> suppliers = null, IView view = null)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            AccessKeys.Add("SupplierId");
            GetData(suppliers);
            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Supplier>(Edit, CanEdit);
            ContactsCommand = new RelayCommand<Supplier>(Contacts, CanAccessContacts);
            DeleteCommand = new RelayCommand<Supplier>(Delete, CanDelete);
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
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
        public Supplier SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Supplier> Items
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
        public RelayCommand<Supplier> EditCommand { get; }
        public RelayCommand<Supplier> ContactsCommand { get; }
        public RelayCommand<Supplier> DeleteCommand { get; }
        public RelayCommand RefreshCommand { get; }
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

        [FilterProperty]
        public string ArabicName
        {
            get => _ArabicName;
            set
            {
                if (SetValue(ref _ArabicName, value))
                {
                    FilterProperty = nameof(ArabicName);
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

        private bool DataFilter(object supplier)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{supplier.GetType().GetProperty(columnName).GetValue(supplier)}".ToUpper();
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

        private void GetData(ObservableCollection<Supplier> suppliers = null)
        {
            if (suppliers == null)
            {
                using SqlConnection connection = new(Data.Database.ConnectionString);
                string query = $"Select * From [Supplier].[Suppliers(View)] " +
                               $"Order By Name";
                Items = new ObservableCollection<Supplier>(connection.Query<Supplier>(query));
            }
            else
            {
                Items = suppliers;
            }
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
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
            Supplier supplier = new();
            Navigation.To(new SupplierView(supplier, Items),ViewData);
        }
        private bool CanAdd()
        {
            return UserData.ModifySuppliers;
        }

        private void Edit(Supplier supplier)
        {
            if (UserData.Access(supplier))
            {
                Navigation.To(new SupplierView(supplier, Items), ViewData);
                Navigation.ClosePopupEvent += Refresh;
            }
        }
        private bool CanEdit(Supplier supplier)
        {
            if (supplier == null)
                return false;

            if (!UserData.ModifySuppliers)
                return false;

            return true;
        }

        private void Contacts(Supplier supplier)
        {
            Navigation.To(new SupplierContactsView(supplier), ViewData);
        }
        private bool CanAccessContacts(Supplier supplier)
        {
            if (supplier == null)
                return false;

            if (!UserData.AccessSuppliersContacts)
                return false;

            return true;
        }

        private void Delete(Supplier supplier)
        {
            if (UserData.Access(supplier))
            {
                MessageBoxResult result = MessageWindow.Show($"Delete",
                                                             $"Are you sure want to Delete\n{supplier.Name}?",
                                                             MessageWindowButton.YesNo,
                                                             MessageWindowImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool delete = false;
                    System.Collections.Generic.IEnumerable<int> checkSupplierUsage;
                    using (SqlConnection connection = new(Data.Database.ConnectionString))
                    {

                        checkSupplierUsage = connection.Query<int>($"Select SupplierID From [Store].[Invoices] Where SupplierID = {supplier.ID}");

                        if (checkSupplierUsage.Count() == 0 || checkSupplierUsage == null)
                        {
                            delete = true;
                            _ = connection.Execute($"Delete From [Supplier].[Suppliers] Where ID = {supplier.ID} ");
                            _ = connection.Execute($"Delete From [Supplier].[Contacts] Where SupplierID = {supplier.ID} ");

                            _ = Items.Remove(supplier);
                        }
                    }

                    if (!delete)
                    {
                        _ = MessageWindow.Show("Deleting", $"You can't delete {supplier.Name} data \n Because its used in projects data", MessageWindowButton.OK, MessageWindowImage.Warning);
                    }
                }

                UserData.Exist(supplier);
            }
        }
        private bool CanDelete(Supplier supplier)
        {
            if (supplier == null)
                return false;

            if (!UserData.ModifySuppliers)
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
    }
}