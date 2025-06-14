using ClosedXML.Excel;

using Dapper;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    public class AccountTransactionsViewModel : Base
    {
        private User _UserData;
        private string _DateInfo;
        private string _Client;
        private string _Description;
        private string _Amount;
        private string _Type;
        private string _Name;
        private Account _AccountData;
        private CustomerAccount _CustomerAccountData;
        private SupplierAccount _SupplierAccountData;

        private string _Indicator = "-";
        private int _SelectedIndex;
        private AccountTransaction _SelectedItem;
        private ObservableCollection<AccountTransaction> _Items;
        private double _Total;

        public AccountTransactionsViewModel(Account account)
        {
            AccountData = account;
            Name = AccountData.Name;
            GetData();
            CreateCollectionView();
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
            AllCommand = new RelayCommand(All);
            ReceiptsCommand = new RelayCommand(Receipts);
            PaymentsCommand = new RelayCommand(Payments);
            ExportCommand = new RelayCommand(Export);
        }

        public AccountTransactionsViewModel(CustomerAccount account)
        {
            CustomerAccountData = account;
            Name = CustomerAccountData.Name;
            GetData();
            CreateCollectionView();
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
            AllCommand = new RelayCommand(All);
            ReceiptsCommand = new RelayCommand(Receipts);
            PaymentsCommand = new RelayCommand(Payments);
            ExportCommand = new RelayCommand(Export);
        }
        public AccountTransactionsViewModel(SupplierAccount account)
        {
            SupplierAccountData = account;
            Name = SupplierAccountData.Name;
            GetData();
            CreateCollectionView();
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
            AllCommand = new RelayCommand(All);
            ReceiptsCommand = new RelayCommand(Receipts);
            PaymentsCommand = new RelayCommand(Payments);
            ExportCommand = new RelayCommand(Export);
        }

        private void GetData()
        {
            string query = "";
            using SqlConnection connection = new(Database.ConnectionString);
            if (AccountData != null)
            {
                query = $"Select * From [Finance].[AccountsTransactions(View)] " +
                        $"Where AccountId = {AccountData.ID} " +
                        $"Order By Date Desc";
            }

            else if (CustomerAccountData != null)
            {
                query = $"Select * From [Finance].[AccountsTransactions(View)] " +
                        $"Where CustomerId = {CustomerAccountData.Id} " +
                        $"Order By Date Desc";
            }

            else if (SupplierAccountData != null)
            {
                query = $"Select * From [Finance].[AccountsTransactions(View)] " +
                        $"Where SupplierId = {SupplierAccountData.Id} " +
                        $"Order By Date Desc";
            }

            Items = new ObservableCollection<AccountTransaction>(connection.Query<AccountTransaction>(query));
        }
        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.Filter = new Predicate<object>(DataFilter);
            ItemsView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            ItemsView.CollectionChanged += CollectionChanged;

            UpdateTotal();
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

        private void All()
        {
            Type = null;
        }

        private void Receipts()
        {
            Type = "Receipt";
        }

        private void Payments()
        {
            Type = "Payment";
        }

        private void Export()
        {
            try
            {
                List<ExcelTransaction> list = new();
                foreach (AccountTransaction transaction in ItemsView.Cast<AccountTransaction>())
                {
                    ExcelTransaction excelTransaction = new()
                    {
                        Date = transaction.DateInfo,
                        Client = transaction.Client,
                        Amount = (decimal)transaction.Amount,
                        Type = transaction.Type,
                        Description = transaction.Description,
                    };

                    if (transaction.Type != Enums.AccountingTransactions.Receipt.ToString())
                    {
                        excelTransaction.Amount *= -1;
                    }

                    list.Add(excelTransaction);
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
                    table.Columns["Date"].SetOrdinal(0);
                    table.Columns["Client"].SetOrdinal(1);
                    table.Columns["Description"].SetOrdinal(2);
                    table.Columns["Amount"].SetOrdinal(3);
                    table.Columns["Type"].SetOrdinal(4);

                    string worksheetName = $"Transactions";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();

                    decimal total = 0;
                    if (AccountData != null)
                        total = AccountData.Balance;
                    else if (CustomerAccountData != null)
                        total = (decimal)CustomerAccountData.Balance;
                    else if (SupplierAccountData != null)
                        total = (decimal)SupplierAccountData.Balance;

                    int lastRow = workSheet.Rows().Count() + 1;
                    workSheet.Cell(lastRow, 4).Value = total;

                    workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 3)).Merge();
                    workSheet.Cell(lastRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    workSheet.Cell(lastRow, 1).Value = "Total";

                    fileName = $"{DateTime.Now:dd-MM-yyyy} {AccountData.Name} {worksheetName}.xlsx";
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
                    _ = MessageWindow.Show("Items", "There is no items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _ = MessageWindow.Show("Error", ex.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }

        private void UpdateTotal()
        {
            if (ItemsView == null)
                return;

            double receipt = ItemsView.Cast<AccountTransaction>().Where(x => x.Type == "Receipt").Sum(x => x.Amount);
            double payment = ItemsView.Cast<AccountTransaction>().Where(x => x.Type != "Payment").Sum(x => x.Amount);
            Total = receipt - payment;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsView);
        }

        public string Name
        {
            get => _Name;
            set => SetValue(ref _Name, value);
        }
        public User UserData
        {
            get => _UserData;
            set => SetValue(ref _UserData, value);
        }
        public Account AccountData
        {
            get => _AccountData;
            set => SetValue(ref _AccountData, value);
        }
        public CustomerAccount CustomerAccountData
        {
            get => _CustomerAccountData;
            set => SetValue(ref _CustomerAccountData, value);
        }
        public SupplierAccount SupplierAccountData
        {
            get => _SupplierAccountData;
            set => SetValue(ref _SupplierAccountData, value);
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
        public AccountTransaction SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<AccountTransaction> Items
        {
            get => _Items;
            private set => SetValue(ref _Items, value);
        }
        public double Total
        {
            get => _Total;
            private set => SetValue(ref _Total, value);
        }


        public ICollectionView ItemsView { get; set; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }
        public RelayCommand AllCommand { get; }
        public RelayCommand ReceiptsCommand { get; }
        public RelayCommand PaymentsCommand { get; }


        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string DateInfo
        {
            get => _DateInfo;
            set
            {
                if (SetValue(ref _DateInfo, value))
                {
                    FilterProperty = nameof(DateInfo);
                    ItemsView.Refresh();
                }
            }
        }
        [FilterProperty]
        public string Client
        {
            get => _Client;
            set
            {
                if (SetValue(ref _Client, value))
                {
                    FilterProperty = nameof(Client);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Description
        {
            get => _Description;
            set
            {
                if (SetValue(ref _Description, value))
                {
                    FilterProperty = nameof(Description);
                    ItemsView.Refresh();
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
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Type
        {
            get => _Type;
            set
            {
                if (SetValue(ref _Type, value))
                {
                    FilterProperty = nameof(Type);
                    ItemsView.Refresh();
                }
            }
        }

    }

    class ExcelTransaction
    {
        public string Date { get; set; }
        public string Client { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
    }
}
