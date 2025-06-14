using ClosedXML.Excel;

using Dapper;

using DocumentFormat.OpenXml.Wordprocessing;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Services;
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
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    public class CustomerStatementViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Statement _SelectedItem;
        private ObservableCollection<Statement> _Items;
        private double _TotalDebit;
        private double _TotalCredit;
        private double _TotalBalance;

        private string _DateInfo;
        private string _Description;
        private string _Debit;
        private string _Credit;
        private string _BalanceView;
        private ICollectionView _ItemsCollection;

        private DateTime _StartDate = DateTime.Parse($"{DateTime.Now.Year}-01-01");
        private DateTime _EndDate = DateTime.Now;

        public CustomerStatementViewModel(CustomerAccount account, IView View)
        {
            ViewData = View;
            CustomerData = account;

            GetData();

            GetDataCommand = new RelayCommand(GetData);
            PrintCommand = new RelayCommand(Print, CanPrint);
            ExportCommand = new RelayCommand(Export, CanExport);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public CustomerAccount CustomerData { get; }
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
        public Statement SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Statement> Items
        {
            get => _Items;
            private set
            {
                if (SetValue(ref _Items, value))
                {
                    CreateCollectionView();
                    UpdateTotal();
                }
            }
        }

        public double TotalDebit
        {
            get => _TotalDebit;
            set => SetValue(ref _TotalDebit, value);
        }
        public double TotalCredit
        {
            get => _TotalCredit;
            set => SetValue(ref _TotalCredit, value);
        }

        public double TotalBalance
        {
            get => _TotalBalance;
            set => SetValue(ref _TotalBalance, value);
        }

        public DateTime StartDate
        {
            get => _StartDate;
            set => SetValue(ref _StartDate, value);
        }
        public DateTime EndDate
        {
            get => _EndDate;
            set => SetValue(ref _EndDate, value);
        }

        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand PrintCommand { get; }
        public RelayCommand GetDataCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }


        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string Description
        {
            get => _Description;
            set
            {
                if (SetValue(ref _Description, value))
                {
                    FilterProperty = nameof(Description);
                    ItemsCollection.Refresh();
                    UpdateTotal();
                }
            }
        }

        [FilterProperty]
        public string Debit
        {
            get => _Debit;
            set
            {
                if (SetValue(ref _Debit, value))
                {
                    FilterProperty = nameof(Debit);
                    ItemsCollection.Refresh();
                    UpdateTotal();
                }
            }
        }

        [FilterProperty]
        public string DateInfo
        {
            get => _DateInfo;
            set
            {
                if (SetValue(ref _DateInfo, value))
                {
                    FilterProperty = nameof(DateInfo);
                    ItemsCollection.Refresh();
                    UpdateTotal();
                }
            }
        }

        [FilterProperty]
        public string Credit
        {
            get => _Credit;
            set
            {
                if (SetValue(ref _Credit, value))
                {
                    FilterProperty = nameof(Credit);
                    ItemsCollection.Refresh();
                    UpdateTotal();
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
                    UpdateTotal();
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

            ItemsCollection.Refresh();
        }

        #endregion

        private void GetData()
        {
            string query;
            double balanceLastYears;
            List<Statement> debit;
            List<Statement> credit;
            List<Statement> debitLastYears;
            List<Statement> creditLastYears;
            List<Statement> statements;

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Finance].[CustomersDebts(View)] " +
                        $"Where CustomerId = {CustomerData.Id} " +
                        $"And Date >= @StartDate And Date <= @EndDate";
                debit = connection.Query<Statement>(query, new { StartDate, EndDate }).ToList();

                query = $"Select * From [Finance].[CustomersCredits(View)] " +
                        $"Where CustomerId = {CustomerData.Id} " +
                        $"And Date >= @StartDate And Date <= @EndDate";
                credit = connection.Query<Statement>(query, new { StartDate, EndDate }).ToList();

                query = $"Select * From [Finance].[CustomersDebts(View)] " +
                        $"Where CustomerId = {CustomerData.Id} " +
                        $"And Date < @StartDate";
                debitLastYears = connection.Query<Statement>(query, new { StartDate }).ToList();

                query = $"Select * From [Finance].[CustomersCredits(View)] " +
                        $"Where CustomerId = {CustomerData.Id} " +
                        $"And Date < @StartDate";
                creditLastYears = connection.Query<Statement>(query, new { StartDate }).ToList();
            }

            balanceLastYears = creditLastYears.Sum(s => s.Credit).GetValueOrDefault() - debitLastYears.Sum(s => s.Debt).GetValueOrDefault();

            statements = new List<Statement>();
            statements.AddRange(debit);
            statements.AddRange(credit);
            statements.Sort((x, y) => x.Date.CompareTo(y.Date));

            if (StartDate.Day == 1 && StartDate.Month == 1)
            {
                statements.Insert(0, new Statement()
                {
                    Date = StartDate,
                    Description = "Last Years Closing Balance",
                    Balance = balanceLastYears,
                    Debt = debitLastYears.Sum(s => s.Debt).GetValueOrDefault(),
                    Credit = creditLastYears.Sum(s => s.Credit).GetValueOrDefault(),
                }
                );
            }
            else
            {
                statements.Insert(0, new Statement()
                {
                    Date = StartDate,
                    Description = "Previous Balance",
                    Balance = balanceLastYears,
                    Debt = debitLastYears.Sum(s => s.Debt).GetValueOrDefault(),
                    Credit = creditLastYears.Sum(s => s.Credit).GetValueOrDefault(),
                }
                );
            }

            double balance = 0;
            foreach (Statement statement in statements)
            {
                statement.SN = statements.IndexOf(statement) + 1;
                statement.Balance = balance = balance + statement.Credit.GetValueOrDefault() - statement.Debt.GetValueOrDefault();
            }

            
            TotalCredit = statements.Sum(s => s.Credit).GetValueOrDefault();
            TotalDebit = statements.Sum(s => s.Debt).GetValueOrDefault();
            TotalBalance = TotalCredit - TotalDebit;

            Items = new ObservableCollection<Statement>(statements);
        }

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
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

        private void Print()
        {
            const double rows = 32;
            double pages = Items.Count / rows;

            if (pages - Math.Truncate(pages) != 0)
            {
                pages = Math.Truncate(pages) + 1;
            }

            List<FrameworkElement> elements = new();
            if (pages != 0)
            {
                for (int i = 1; i <= pages; i++)
                {
                    Printing.Finance.CustomerStatement statement = new()
                    {
                        CustomerName = CustomerData.Name,
                        StartDate = StartDate,
                        EndDate = EndDate,
                        VATNumber = CustomerData.VATNumber,
                        Debit = Items.Sum(s => s.Debt).GetValueOrDefault(),
                        Credit = Items.Sum(s => s.Credit).GetValueOrDefault(),

                        Statements = Items.Where(s => s.SN > ((i - 1) * rows) && s.SN <= (i * rows)).ToList(),
                        Page = i,
                        Pages = Convert.ToInt32(pages)
                    };
                    elements.Add(statement);
                }

                Printing.Print.PrintPreview(elements, $"Statement-{CustomerData.Name}", ViewData);
            }
            else
            {
                _ = MessageWindow.Show("Statement", "There is no items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }
        private bool CanPrint()
        {
            return Items.Count > 0;
        }

        private void UpdateTotal()
        {

        }

        private class ExPrint
        {
            public string Date { get; set; }
            public string Description { get; set; }
            public string Debt { get; set; }
            public string Credit { get; set; }
            public string Balance { get; set; }
        }
        private void Export()
        {
            string fileName;
            List<ExPrint> statements = new();

            foreach (Statement statement in Items)
            {
                statements.Add(
                           new ExPrint()
                           {
                               Date = statement.Date.Date.ToString("dd-MM-yyyy"),
                               Description = statement.Description,
                               Debt = statement.DebtView,
                               Credit = statement.CreditView,
                               Balance = statement.BalanceView,
                           });
            }

            using XLWorkbook workbook = new();
            DataTable table = new();
            using (ObjectReader reader = ObjectReader.Create(statements))
            {
                table.Load(reader);
            }
            table.Columns["Date"].SetOrdinal(0);
            table.Columns["Description"].SetOrdinal(1);
            table.Columns["Debt"].SetOrdinal(2);
            table.Columns["Credit"].SetOrdinal(3);
            table.Columns["Balance"].SetOrdinal(4);

            string worksheetName = "Statement";
            _ = workbook.Worksheets.Add(table, worksheetName);

            IXLWorksheet workSheet = workbook.Worksheet(worksheetName);
            _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            _ = workSheet.Column(1).AdjustToContents();
            _ = workSheet.Column(2).AdjustToContents();
            _ = workSheet.Column(3).AdjustToContents();
            _ = workSheet.Column(4).AdjustToContents();
            _ = workSheet.Column(5).AdjustToContents();

            int lastRow = workSheet.Rows().Count() + 1;
            workSheet.Cell(lastRow, 3).Value = TotalDebit;
            workSheet.Cell(lastRow, 4).Value = TotalCredit;
            workSheet.Cell(lastRow, 5).Value = TotalBalance;

            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 5)).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#4f81bd"));
            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 5)).Style.Font.FontColor = XLColor.White;
            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 5)).Style.Font.Bold = true;

            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 2)).Merge();
            workSheet.Cell(lastRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            workSheet.Cell(lastRow, 1).Value = "Total ";

            fileName = $"{CustomerData.Name}-{worksheetName}.xlsx";
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

        private bool CanExport()
        {
            return Items.Count > 0;
        }
    }
}