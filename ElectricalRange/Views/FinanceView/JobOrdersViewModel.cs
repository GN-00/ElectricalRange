using Dapper;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    public class JobOrdersViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private JobOrder _SelectedItem;
        private ObservableCollection<JobOrder> _Items;

        private int? _SelectedYear;
        private ObservableCollection<int> _Years;

        private string _Code;
        private string _QuotationCode;
        private string _CustomerName;
        private string _Amount;
        private string _Paid;
        private string _BalanceView;

        private ICollectionView _ItemsCollection;
        public JobOrdersViewModel(CustomerAccount account, IView view)
        {
            AccountData = account;
            ViewData = view;
            AccessKeys.Add("AccountantOrderId");
            UserData = Navigation.UserData;

            GetYearsData();
            TransactionsCommand = new RelayCommand<JobOrder>(Transactions, CanAccessTransactions);
            PostItemsCommand = new RelayCommand<JobOrder>(PostItems, CanAccessPostItems);
            InvoicingCommand = new RelayCommand<JobOrder>(Invoicing, CanAccessInvoicing);
            InvoicesCommand = new RelayCommand<JobOrder>(Invoices, CanAccessInvoices);
            ProformaCommand = new RelayCommand<JobOrder>(Proforma, CanAccessProforma);
            SuppliersCommand = new RelayCommand<JobOrder>(Suppliers, CanAccessSuppliers);
            NotifyDownPaymentCommand = new RelayCommand<JobOrder>(NotifyDownPayment, CanAccessNotifyDownPayment);
            NotifyReceiptCommand = new RelayCommand<JobOrder>(NotifyReceipt, CanAccessNotifyReceipt);

            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public string YearInfo => $"{SelectedYear ?? DateTime.Now.Year}";
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
        public JobOrder SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<JobOrder> Items
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
        public int? SelectedYear
        {
            get => _SelectedYear;
            set
            {
                if (SetValue(ref _SelectedYear, value))
                {
                    if (SelectedYear == null)
                        return;

                    Navigation.OpenLoading(Visibility.Visible, "Loading...");
                    Events.ShowEvent.Do();

                    DeleteFilter();
                    GetData(SelectedYear);
                    OnPropertyChanged(nameof(YearInfo));

                    Navigation.CloseLoading();
                }
            }
        }
        public ObservableCollection<int> Years
        {
            get => _Years;
            private set => SetValue(ref _Years, value);
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand<JobOrder> TransactionsCommand { get; }
        public RelayCommand<JobOrder> PostItemsCommand { get; }
        public RelayCommand<JobOrder> InvoicingCommand { get; }
        public RelayCommand<JobOrder> InvoicesCommand { get; }
        public RelayCommand<JobOrder> ProformaCommand { get; }
        public RelayCommand<JobOrder> SuppliersCommand { get; }
        public RelayCommand<JobOrder> NotifyDownPaymentCommand { get; }
        public RelayCommand<JobOrder> NotifyReceiptCommand { get; }
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
        public string QuotationCode
        {
            get => _QuotationCode;
            set
            {
                if (SetValue(ref _QuotationCode, value))
                {
                    FilterProperty = nameof(QuotationCode);
                    ItemsCollection.Refresh();
                }
            }
        }

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
        public string Amount
        {
            get => _Amount;
            set
            {
                if (SetValue(ref _Amount, value))
                {
                    FilterProperty = nameof(Amount);
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
        public string BalanceView
        {
            get => _BalanceView;
            set
            {
                if (SetValue(ref _BalanceView, value))
                {
                    FilterProperty = nameof(BalanceView);
                    ItemsCollection.Refresh();
                }
            }
        }

        public CustomerAccount AccountData { get; }

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

        private void GetYearsData()
        {
            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select Year From [Finance].[JobOrders(View)] " +
                        $"Group By Year " +
                        $"Order By Year Desc";

                Years = new ObservableCollection<int>(connection.Query<int>(query));
            }

            if (Years.Count != 0)
                SelectedYear = Years[0];
        }

        private void GetData(int? year)
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Finance].[JobOrders(View)] " +
                    $"Where Year = {year ?? DateTime.Now.Year} ";

            if (AccountData != null)
                query += $"And CustomerID = {AccountData.CustomerID} ";

            query += $"Order By Year Desc, CodeNumber Desc";
            Items = new ObservableCollection<JobOrder>(connection.Query<JobOrder>(query));
        }

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Year", ListSortDirection.Descending));
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

        private void Transactions(JobOrder order)
        {
            //if (UserData.Access(order))
            //{
            //    JobOrderTransactionsWindow jobOrderTransactionsWindow = new JobOrderTransactionsWindow(order);
            //    _ = jobOrderTransactionsWindow.ShowDialog();
            //}
        }
        private bool CanAccessTransactions(JobOrder order)
        {
            return false;

            //if (order == null)
            //    return false;

            //return true;
        }

        private void PostItems(JobOrder order)
        {
            string query;
            Data.JobOrders.JobOrder orderData;
            if (order.ID == 0) //Factory Store
            {
                orderData = Database.Store;
            }
            else
            {
                using SqlConnection connection = new(Database.ConnectionString);
                query = $"Select * From [JobOrder].[JobOrdersInformation] Where ID = {order.ID}";
                orderData = connection.QueryFirstOrDefault<Data.JobOrders.JobOrder>(query);
            }

            Navigation.To(new StoreViews.InvoicesView(orderData), ViewData);
        }
        private bool CanAccessPostItems(JobOrder order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void Invoicing(JobOrder order)
        {
            Navigation.OpenPopup(new SelectInvoiceTypeView(order, ViewData), PlacementMode.MousePoint, false);
        }
        private bool CanAccessInvoicing(JobOrder order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void Invoices(JobOrder order)
        {
            FinanceServices.GetJobOrderInvoices(order, ViewData);
        }
        private bool CanAccessInvoices(JobOrder order)
        {
            return FinanceServices.CanGetJobOrderInvoices(order);
        }

        private void Proforma(JobOrder order)
        {
            if (UserData.Access(order))
            {
                Navigation.To(new ProformaInvoicesView(order), ViewData);
            }
        }
        private bool CanAccessProforma(JobOrder order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void Suppliers(JobOrder order)
        {
            if (UserData.Access(order))
            {
                Navigation.To(new SuppliersInvoicesView(order), ViewData);
            }
        }
        private bool CanAccessSuppliers(JobOrder order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void NotifyDownPayment(JobOrder order)
        {
            DownPayment downPayment;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Finance].[NotificationForDawnPayment] Where JobOrderId = {order.ID}";
                downPayment = connection.QueryFirstOrDefault<DownPayment>(query);
            }

            if (downPayment != null)
            {
                Printing.Finance.NotificationForDownPaymentForm
                    notificationForDownPaymentForm = new(downPayment);
                string name = $"Notification For Down Payment Job Order No.{order.Code}";
                name = name.Replace("/", "-");
                Printing.Print.PrintPreview(notificationForDownPaymentForm, name, System.Printing.PageOrientation.Portrait, ViewData);
            }
        }
        private bool CanAccessNotifyDownPayment(JobOrder order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void NotifyReceipt(JobOrder order)
        {
            if (UserData.Access(order))
            {
                Navigation.To(new NotificationForReceiptView(order), ViewData);
            }
        }
        private bool CanAccessNotifyReceipt(JobOrder order)
        {
            if (order == null)
                return false;

            if (!UserData.AccessNotifyReceipts)
                return false;

            return true;
        }
    }
}