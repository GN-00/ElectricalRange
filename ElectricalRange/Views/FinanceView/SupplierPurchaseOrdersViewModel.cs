using Dapper;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    public class SupplierPurchaseOrdersViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private PurchaseOrder _SelectedItem;
        private ObservableCollection<PurchaseOrder> _Orders;

        private string _Code;
        private string _JobOrderCode;
        private string _Items;
        private string _Invoices;
        private string _ReceivedItems;
        private string _ReturndItems;
        private string _GrossPrice;
        private string _Paid;
        private string _Balance;

        private ICollectionView _ItemsCollection;
        public SupplierPurchaseOrdersViewModel(SupplierAccount account, IView view)
        {
            AccountData = account;
            ViewData = view;
            AccessKeys.Add("SupplierId");
            UserData = Navigation.UserData;

            GetData();
            InvoicesCommand = new RelayCommand<PurchaseOrder>(PurchaseOrderInvoices, CanAccessPurchaseOrderInvoices);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public SupplierAccount AccountData { get; }
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
        public PurchaseOrder SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<PurchaseOrder> Orders
        {
            get => _Orders;
            private set
            {
                if (SetValue(ref _Orders, value))
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

        public RelayCommand<PurchaseOrder> InvoicesCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }


        #region Data Filter

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
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string JobOrderCode
        {
            get => _JobOrderCode;
            set
            {
                if (SetValue(ref _JobOrderCode, value))
                {
                    FilterProperty = nameof(JobOrderCode);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Items
        {
            get => _Items;
            set
            {
                if (SetValue(ref _Items, value))
                {
                    FilterProperty = nameof(Items);
                    ItemsCollection.Refresh();
                }
            }
        }
        [FilterProperty]
        public string Invoices
        {
            get => _Invoices;
            set
            {
                if (SetValue(ref _Invoices, value))
                {
                    FilterProperty = nameof(Invoices);
                    ItemsCollection.Refresh();
                }
            }
        }
        [FilterProperty]
        public string ReceivedItems
        {
            get => _ReceivedItems;
            set
            {
                if (SetValue(ref _ReceivedItems, value))
                {
                    FilterProperty = nameof(ReceivedItems);
                    ItemsCollection.Refresh();
                }
            }
        }
        [FilterProperty]
        public string ReturndItems
        {
            get => _ReturndItems;
            set
            {
                if (SetValue(ref _ReturndItems, value))
                {
                    FilterProperty = nameof(ReturndItems);
                    ItemsCollection.Refresh();
                }
            }
        }


        [FilterProperty]
        public string GrossPrice
        {
            get => _GrossPrice;
            set
            {
                if (SetValue(ref _GrossPrice, value))
                {
                    FilterProperty = nameof(GrossPrice);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Paid
        {
            get => _Paid;
            set
            {
                if (SetValue(ref _Paid, value))
                {
                    FilterProperty = nameof(Paid);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Balance
        {
            get => _Balance;
            set
            {
                if (SetValue(ref _Balance, value))
                {
                    FilterProperty = nameof(Balance);
                    ItemsCollection.Refresh();
                }
            }
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
        private void DeleteFilter()
        {
            if (ItemsCollection == null)
                return;

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
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Purchase].[OrdersView] " +
                    $"Where SupplierID = {AccountData.Id} " +
                    $"Order By CodeYear Desc, CodeNumber Desc";
            Orders = new ObservableCollection<PurchaseOrder>(connection.Query<PurchaseOrder>(query));
        }

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Orders);
            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("CodeYear", ListSortDirection.Descending));
            ItemsCollection.SortDescriptions.Add(new SortDescription("CodeNumber", ListSortDirection.Descending));
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

        private void PurchaseOrderInvoices(PurchaseOrder order)
        {
            Navigation.To(new SupplierInvoicesView(AccountData, order), ViewData);
        }
        private bool CanAccessPurchaseOrderInvoices(PurchaseOrder order)
        {
            if (order == null)
                return false;

            if (order.Invoices == 0)
                return false;

            return true;
        }
    }
}