using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ClosedXML.Excel;
using Dapper;
using FastMember;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Views.FinanceView
{
    public class CustomersAccountsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private CustomerAccount _SelectedItem;
        private ObservableCollection<CustomerAccount> _Items;

        private string _Name;
        private string _Balance;
        private string _Projects;
        private string _Invoices;
        private string _Account;
        private string _Debt;

        private ICollectionView _ItemsView;

        public CustomersAccountsViewModel(IView view = null)
        {
            ViewData = view;
            AccessKeys.Add("CustomerId");

            GetData();
            TransactionsCommand = new RelayCommand<CustomerAccount>(Transactions, CanAccessTransacctions);
            StatementsCommand = new RelayCommand<CustomerAccount>(Statements, CanAccessStatements);
            OrdersCommand = new RelayCommand<CustomerAccount>(Orders, CanAccessOrders);
            InvoicesCommand = new RelayCommand<CustomerAccount>(CustomerInvoices, CanAccessCustomerInvoices);
            BankCommand = new RelayCommand<CustomerAccount>(BankInfo, CanAccessBankInfo);
            ExportCommand = new RelayCommand(Export, CanAccessExport);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData => Navigation.UserData;
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
        public CustomerAccount SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<CustomerAccount> Items
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
        public ICollectionView ItemsView
        {
            get => _ItemsView;
            set => SetValue(ref _ItemsView, value);
        }

        public RelayCommand<CustomerAccount> StatementsCommand { get; }
        public RelayCommand<CustomerAccount> OrdersCommand { get; }
        public RelayCommand<CustomerAccount> InvoicesCommand { get; }
        public RelayCommand<CustomerAccount> BankCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand<CustomerAccount> TransactionsCommand { get; }
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
                    ItemsView.Refresh();
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
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Projects
        {
            get => _Projects;
            set
            {
                if (SetValue(ref _Projects, value))
                {
                    FilterProperty = nameof(Projects);
                    ItemsView.Refresh();
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
                    FilterProperty = nameof(CustomerInvoices);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Account
        {
            get => _Account;
            set
            {
                if (SetValue(ref _Account, value))
                {
                    FilterProperty = nameof(Account);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Debt
        {
            get => _Debt;
            set
            {
                if (SetValue(ref _Debt, value))
                {
                    FilterProperty = nameof(Debt);
                    ItemsView.Refresh();
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

        #endregion

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Finance].[CustomersAccounts(View)]" +
                    $"Order By Name";
            Items = new ObservableCollection<CustomerAccount>(connection.Query<CustomerAccount>(query));
        }
        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.Filter = new Predicate<object>(DataFilter);
            ItemsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            ItemsView.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsView);
        }

        private void Transactions(CustomerAccount account)
        {
            Navigation.To(new AccountTransactionsView(account), ViewData);
        }
        private bool CanAccessTransacctions(CustomerAccount account)
        {
            if (account == null)
                return false;

            return true;
        }

        private void Statements(CustomerAccount account)
        {
            Navigation.To(new CustomerStatementView(account), ViewData);
        }
        private bool CanAccessStatements(CustomerAccount account)
        {
            if (account == null)
                return false;

            return true;
        }

        private void Orders(CustomerAccount account)
        {
            if (UserData.Access(account))
            {
                Navigation.To(new JobOrdersView(account), ViewData);
            }
        }
        private bool CanAccessOrders(CustomerAccount account)
        {
            if (account == null)
                return false;

            return true;
        }

        private void CustomerInvoices(CustomerAccount account)
        {
            if (UserData.Access(account))
            {
                Navigation.To(new CustomerInvoicesView(account), ViewData);
            }
        }
        private bool CanAccessCustomerInvoices(CustomerAccount account)
        {
            if (account == null)
                return false;

            return true;
        }

        private void BankInfo(CustomerAccount account)
        {
            if (UserData.Access(account))
            {
                Navigation.OpenPopup(new BankInformationView(account), PlacementMode.Center, true);
            }
        }
        private bool CanAccessBankInfo(CustomerAccount account)
        {
            if (account == null)
                return false;

            return true;
        }



        class ExcelCustomer
        {
            public string Name { get; set; }
            public int Projects { get; set; }
            public int Invoices { get; set; }
            public double Account { get; set; }
            public double Debt { get; set; }
            public double Balance { get; set; }
        }

        private void Export()
        {
            try
            {
                List<ExcelCustomer> list = new();
                foreach (CustomerAccount customer in ItemsView.Cast<CustomerAccount>())
                {
                    ExcelCustomer excelCustomers = new()
                    {
                        Name = customer.Name,
                        Projects = customer.Projects,
                        Invoices = customer.Invoices,
                        Account = customer.Account,
                        Debt = customer.Debt,
                        Balance = customer.Balance,
                    };

                    list.Add(excelCustomers);
                }

                if (list.Count != 0)
                {
                    string fileName;
                    using XLWorkbook workbook = new();
                    DataTable table = new();
                    using (ObjectReader reader = ObjectReader.Create(list))
                    {
                        table.Load(reader);
                    }
                    table.Columns["Name"].SetOrdinal(0);
                    table.Columns["Projects"].SetOrdinal(1);
                    table.Columns["Invoices"].SetOrdinal(2);
                    table.Columns["Account"].SetOrdinal(3);
                    table.Columns["Debt"].SetOrdinal(4);
                    table.Columns["Balance"].SetOrdinal(5);

                    string worksheetName = $"Customers Accounts";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();

                    fileName = $"{DateTime.Now:dd-MM-yyyy} {worksheetName}.xlsx";
                    fileName = fileName.Replace("/", "-");

                    SaveFileDialog saveFileDialog = new()
                    {
                        FileName = fileName,
                        DefaultExt = ".xlsx",
                        Filter = "Excel Worksheets|*.xlsx",
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                    }
                }
                else
                {
                    _ = MessageWindow.Show("Items", "There is no records!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _ = MessageWindow.Show("Error", ex.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }

        private bool CanAccessExport()
        {
            return true;
        }
    }
}