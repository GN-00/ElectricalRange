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
    public class PurchaseOrdersViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private PurchaseOrder _SelectedItem;
        private ObservableCollection<PurchaseOrder> _Orders;

        private string _SupplierName;
        private string _Code;
        private string _JobOrderCode;
        private string _Items;
        private string _Invoices;
        private string _GrossPrice;
        private string _Paid;
        private string _Balance;

        private ICollectionView _ItemsCollection;
        public PurchaseOrdersViewModel(IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;

            GetData();
            ExportCommand = new RelayCommand(Export, CanAccessExport);
            InvoicesCommand = new RelayCommand<PurchaseOrder>(PurchaseOrderInvoices, CanAccessPurchaseOrderInvoices);

            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

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

        public RelayCommand ExportCommand { get; }
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
        public string SupplierName
        {
            get => _SupplierName;
            set
            {
                if (SetValue(ref _SupplierName, value))
                {
                    FilterProperty = nameof(SupplierName);
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

        class ExcelPurchaseOrder
        {
            public string Supplier { get; set; }
            public string Code { get; set; }
            public string Date { get; set; }
            public string JobOrder { get; set; }
            public double Items { get; set; }
            public double Invoices { get; set; }
            public double ReceivedItems { get; set; }
            public double ReturnedItems { get; set; }
            public double Amount { get; set; }
            public double Paid { get; set; }
            public double Balance { get; set; }
        }

        private void Export()
        {
            try
            {
                List<ExcelPurchaseOrder> list = new();
                foreach (PurchaseOrder order in ItemsCollection.Cast<PurchaseOrder>())
                {
                    ExcelPurchaseOrder excelOrder = new()
                    {
                        Supplier = order.SupplierName,
                        Code = order.Code,
                        Date = order.Date.ToString("dd-MM-yyyy"),
                        JobOrder = order.JobOrderCode,
                        Items = order.Items,
                        Invoices = order.Invoices,
                        ReceivedItems = order.ReceivedItems,
                        ReturnedItems = order.ReturnedItems,
                        Amount = (double)order.GrossPrice,
                        Paid = (double)order.Paid,
                        Balance = (double)order.Balance,
                    };

                    list.Add(excelOrder);
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
                    table.Columns["Supplier"].SetOrdinal(0);
                    table.Columns["Code"].SetOrdinal(1);
                    table.Columns["Date"].SetOrdinal(2);
                    table.Columns["JobOrder"].SetOrdinal(3);
                    table.Columns["Items"].SetOrdinal(4);
                    table.Columns["Invoices"].SetOrdinal(5);
                    table.Columns["ReceivedItems"].SetOrdinal(6);
                    table.Columns["ReturnedItems"].SetOrdinal(7);
                    table.Columns["Amount"].SetOrdinal(8);
                    table.Columns["Paid"].SetOrdinal(9);
                    table.Columns["Balance"].SetOrdinal(10);

                    string worksheetName = $"Purchase Orders";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    workSheet.Cell(1, 4).Value = "Job Order";
                    workSheet.Cell(1, 7).Value = "Received Items";
                    workSheet.Cell(1, 8).Value = "Returned Items";

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();
                    _ = workSheet.Column(7).AdjustToContents();
                    _ = workSheet.Column(8).AdjustToContents();
                    _ = workSheet.Column(9).AdjustToContents();
                    _ = workSheet.Column(10).AdjustToContents();
                    _ = workSheet.Column(11).AdjustToContents();


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
                    _ = MessageWindow.Show("Items", "There is no items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
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

        private void PurchaseOrderInvoices(PurchaseOrder order)
        {
            string query;
            SupplierAccount account;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Finance].[SuppliersAccounts] Where ID = {order.SupplierID}";
                account = connection.QueryFirstOrDefault<SupplierAccount>(query);
            }

            Navigation.To(new SupplierInvoicesView(account, order), ViewData);
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