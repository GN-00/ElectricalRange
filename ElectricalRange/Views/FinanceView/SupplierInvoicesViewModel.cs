using ClosedXML.Excel;

using Dapper;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;

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
    public class SupplierInvoicesViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private SupplierInvoice _SelectedItem;
        private ObservableCollection<SupplierInvoice> _Invoices;
        private double _TotalInvoiced;
        private double _TotalPaid;
        private double _TotalBalance;

        private string _Number;
        private string _DateInfo;
        private string _Items;
        private string _PurchaseOrder;
        private string _ReturnValue;
        private string _GrossPrice;
        private string _Paid;
        private string _Balance;
        private ICollectionView _ItemsCollection;

        public SupplierInvoicesViewModel(SupplierAccount account, PurchaseOrder order, IView view)
        {
            ViewData = view;
            SupplierData = account;
            OrderData = order;

            GetData();

            PayCommand = new RelayCommand<SupplierInvoice>(Pay, CanPay);
            InvoiceCommand = new RelayCommand<SupplierInvoice>(Invoice, CanInvoice);
            ExportCommand = new RelayCommand(Export, CanExport);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData { get; }
        public SupplierAccount SupplierData { get; }
        public PurchaseOrder OrderData { get; }

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
        public SupplierInvoice SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<SupplierInvoice> Invoices
        {
            get => _Invoices;
            private set
            {
                if (SetValue(ref _Invoices, value))
                {
                    CreateCollectionView();
                    UpdateTotal();
                }
            }
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public double TotalInvoiced
        {
            get => _TotalInvoiced;
            set => SetValue(ref _TotalInvoiced, value);
        }
        public double TotalPaid
        {
            get => _TotalPaid;
            set => SetValue(ref _TotalPaid, value);
        }
        public double TotalBalance
        {
            get => _TotalBalance;
            set => SetValue(ref _TotalBalance, value);
        }
        
        public RelayCommand<SupplierInvoice> PayCommand { get; }
        public RelayCommand<SupplierInvoice> InvoiceCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }


        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string Number
        {
            get => _Number;
            set
            {
                if (SetValue(ref _Number, value))
                {
                    FilterProperty = nameof(Number);
                    ItemsCollection.Refresh();
                    UpdateTotal();
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
        public string PurchaseOrder
        {
            get => _PurchaseOrder;
            set
            {
                if (SetValue(ref _PurchaseOrder, value))
                {
                    FilterProperty = nameof(PurchaseOrder);
                    ItemsCollection.Refresh();
                    UpdateTotal();
                }
            }
        }

        [FilterProperty]
        public string ReturnValue
        {
            get => _ReturnValue;
            set
            {
                if (SetValue(ref _ReturnValue, value))
                {
                    FilterProperty = nameof(ReturnValue);
                    ItemsCollection.Refresh();
                    UpdateTotal();
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
                    UpdateTotal();
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
                    UpdateTotal();
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
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Finance].[SuppliersInvoices(View)] " +
                    $"Where SupplierID = {SupplierData.Id} " +
                    $"{(OrderData != null ? $"And PurchaseOrderID = {OrderData.ID}" : "")}" +
                    $"Order By Date Desc";

            Invoices = new ObservableCollection<SupplierInvoice>(connection.Query<SupplierInvoice>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Invoices);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Year", ListSortDirection.Descending));
            ItemsCollection.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Descending));
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

        private void Pay(SupplierInvoice invoice)
        {
            Navigation.OpenPopup(new PaySupplierInvoiceView(SupplierData, invoice), System.Windows.Controls.Primitives.PlacementMode.Center, true);
            Navigation.ClosePopupEvent += UpdateTotal;
        }
        private bool CanPay(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.Balance < 1)
                return false;

            return true;
        }

        private void Invoice(SupplierInvoice invoice)
        {
            Navigation.To(new SupplierInvoiceView(invoice), ViewData);
        }
        private bool CanInvoice(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            return true;
        }

        private void UpdateTotal()
        {
            //TotalInvoiced = Invoices.Sum(i => i.GrossPrice);
            //TotalPaid = Invoices.Sum(i => i.Paid);
            //TotalBalance = Invoices.Sum(i => i.Balance);
        }

        private class ExInvoice
        {
            public string Code { get; set; }
            public string Date { get; set; }
            public string JobOrderCode { get; set; }
            public double Items { get; set; }
            public double GrossPrice { get; set; }
            public double Paid { get; set; }
            public double Balance { get; set; }
        }
        private void Export()
        {
            string fileName;
            List<ExInvoice> invoices = new();

            foreach (SupplierInvoice invoice in Invoices)
            {
                invoices.Add(
                           new ExInvoice()
                           {
                               Code = invoice.Number,
                               Date = invoice.Date.Date.ToString("dd-MM-yyyy"),
                               JobOrderCode = invoice.JobOrderCode,
                               Items = invoice.Items,
                               GrossPrice = invoice.GrossPrice,
                               Paid = invoice.Paid,
                               Balance = invoice.Balance,
                           });
            }

            using XLWorkbook workbook = new();
            DataTable table = new();
            using (ObjectReader reader = ObjectReader.Create(invoices))
            {
                table.Load(reader);
            }
            table.Columns["Code"].SetOrdinal(0);
            table.Columns["Date"].SetOrdinal(1);
            table.Columns["JobOrderCode"].SetOrdinal(2);
            table.Columns["Items"].SetOrdinal(3);
            table.Columns["GrossPrice"].SetOrdinal(4);
            table.Columns["Paid"].SetOrdinal(5);
            table.Columns["Balance"].SetOrdinal(6);

            string worksheetName = $"Invoices";
            _ = workbook.Worksheets.Add(table, worksheetName);

            IXLWorksheet workSheet = workbook.Worksheet(worksheetName);
            _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            _ = workSheet.Column(1).AdjustToContents();
            _ = workSheet.Column(2).AdjustToContents();
            _ = workSheet.Column(3).AdjustToContents();
            _ = workSheet.Column(4).AdjustToContents();
            _ = workSheet.Column(5).AdjustToContents();
            _ = workSheet.Column(6).AdjustToContents();
            _ = workSheet.Column(7).AdjustToContents();

            workSheet.Cell(1, 3).Value = "Job Order";
            workSheet.Cell(1, 5).Value = "Gross Price";

            fileName = $"{SupplierData.Name} {worksheetName}.xlsx";
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
            return Invoices.Count > 0;
        }
    }
}