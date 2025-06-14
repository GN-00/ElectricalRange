using ClosedXML.Excel;

using Dapper;
using Dapper.Contrib.Extensions;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    public class JobOrderInvoicesViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Invoice _SelectedItem;
        private ObservableCollection<Invoice> _Invoices;
        private double _TotalInvoiced;
        private double _TotalPaid;
        private double _TotalBalance;

        private string _Code;
        private string _Items;
        private string _DateInfo;
        private string _NetPrice;
        private string _VATValue;
        private string _GrossPrice;
        private string _Paid;
        private string _Balance;

        private ICollectionView _ItemsCollection;

        public JobOrderInvoicesViewModel(JobOrder order, IView view)
        {
            ViewData = view;
            OrderData = order;

            GetData(order);

            AddCommand = new RelayCommand(Add, CanAdd);
            InfoCommand = new RelayCommand<Invoice>(OrderPanels, CanAccessOrderPanels);
            ReturnCommand = new RelayCommand<Invoice>(Return, CanReturn);
            PayCommand = new RelayCommand<Invoice>(Pay, CanPay);
            InvoiceCommand = new RelayCommand<Invoice>(Invoice, CanInvoice);
            ExportCommand = new RelayCommand(Export, CanExport);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData => Navigation.UserData;
        public JobOrder OrderData { get; }
        public CustomerAccount CustomerData { get; private set; }

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
        public Invoice SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Invoice> Invoices
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

        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }
        public RelayCommand AddCommand { get; }
        public RelayCommand<Invoice> InfoCommand { get; }
        public RelayCommand<Invoice> ReturnCommand { get; }
        public RelayCommand<Invoice> PayCommand { get; }
        public RelayCommand<Invoice> InvoiceCommand { get; }
        public RelayCommand ExportCommand { get; }
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
        public string NetPrice
        {
            get => _NetPrice;
            set
            {
                if (SetValue(ref _NetPrice, value))
                {
                    FilterProperty = nameof(NetPrice);
                    ItemsCollection.Refresh();
                    UpdateTotal();
                }
            }
        }

        [FilterProperty]
        public string VATValue
        {
            get => _VATValue;
            set
            {
                if (SetValue(ref _VATValue, value))
                {
                    FilterProperty = nameof(VATValue);
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

        private void GetData(JobOrder order)
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Finance].[CustomersInvoices(View)] " +
                    $"Where JobOrderId = {order.ID}; ";
            Invoices = new ObservableCollection<Invoice>(connection.Query<Invoice>(query));

            query = $"Select * From [Finance].[CustomersAccounts(View)] " +
                    $"Where CustomerId = {order.CustomerID} ";
            CustomerData = connection.QueryFirstOrDefault<CustomerAccount>(query);
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Invoices);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            ItemsCollection.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
            UpdateTotal();
        }

        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }

        private void UpdateTotal()
        {
            TotalInvoiced = Invoices.Sum(i => i.GrossPrice);
            TotalPaid = Invoices.Sum(i => i.Paid);
            TotalBalance = Invoices.Sum(i => i.Balance);

            OrderData.Paid = (decimal)TotalPaid;
        }


        private void Add()
        {
            Invoice invoice = new()
            {
                VAT = (double)OrderData.VAT,
                VATPercentage = (double)OrderData.VATPercentage,
                JobOrderId = OrderData.ID,
                CustomerId = OrderData.CustomerID,
                JobOrderCode = OrderData.Code,
                Code = "-New Invoice-",
                Date = DateTime.Now,
                CustomerName = OrderData.CustomerName,
            };
            Navigation.OpenPopup(new SelectInvoicingTypeView(invoice, Invoices, CustomerData, ViewData), PlacementMode.MousePoint, false);
        }
        private bool CanAdd()
        {
            if (!UserData.ModifyJobOrdersInvoices)
                return false;

            return true;
        }

        private void Invoice(Invoice item)
        {
            JobOrdersInvoicesServices.PrintInvoice(item.Code, ViewData);
        }
        private bool CanInvoice(Invoice item)
        {
            if (item == null)
                return false;

            return true;
        }

        private void OrderPanels(Invoice item)
        {
            Navigation.To(new JobOrderInvoiceView(item, Invoices, CustomerData), ViewData);
        }
        private bool CanAccessOrderPanels(Invoice item)
        {
            if (item == null)
                return false;

            return true;
        }

        private void Pay(Invoice invoice)
        {
            Navigation.OpenPopup(new PayInvoiceView(CustomerData, invoice), PlacementMode.Center, true);
            Navigation.ClosePopupEvent += UpdateTotal;
        }

        private bool CanPay(Invoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.Balance < 1)
                return false;

            return true;
        }

        private class ExInvoice
        {
            public string Namber { get; set; }
            public string Date { get; set; }
            public double Panels { get; set; }
            public double NetPrice { get; set; }
            public double VATValue { get; set; }
            public double GrossPrice { get; set; }
            public double Paid { get; set; }
            public double Balance { get; set; }
        }
        private void Export()
        {
            string fileName;
            List<ExInvoice> invoices = new();

            foreach (Invoice invoice in Invoices)
            {
                invoices.Add(
                           new ExInvoice()
                           {
                               Namber = invoice.Code,
                               Date = invoice.Date.Date.ToString("dd-MM-yyyy"),
                               Panels = invoice.Items,
                               NetPrice = invoice.NetPrice,
                               VATValue = invoice.VATValue,
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
            table.Columns["Namber"].SetOrdinal(0);
            table.Columns["Date"].SetOrdinal(1);
            table.Columns["Panels"].SetOrdinal(2);
            table.Columns["NetPrice"].SetOrdinal(3);
            table.Columns["VATValue"].SetOrdinal(4);
            table.Columns["GrossPrice"].SetOrdinal(5);
            table.Columns["Paid"].SetOrdinal(6);
            table.Columns["Balance"].SetOrdinal(7);

            string worksheetName = "Invoices";
            _ = workbook.Worksheets.Add(table, worksheetName);

            IXLWorksheet workSheet = workbook.Worksheet(worksheetName);
            _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            workSheet.Cell(1, 4).Value = "Net Price";
            workSheet.Cell(1, 5).Value = "VAT Value";
            workSheet.Cell(1, 6).Value = "Gross Price";

            _ = workSheet.Column(1).AdjustToContents();
            _ = workSheet.Column(2).AdjustToContents();
            _ = workSheet.Column(3).AdjustToContents();
            _ = workSheet.Column(4).AdjustToContents();
            _ = workSheet.Column(5).AdjustToContents();
            _ = workSheet.Column(6).AdjustToContents();
            _ = workSheet.Column(7).AdjustToContents();
            _ = workSheet.Column(8).AdjustToContents();

            fileName = $"J.O.No.{OrderData.Code} {worksheetName}.xlsx";
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


        private void Return(Invoice invoice)
        {
            Invoice returnInvoice;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Finance].[CustomersInvoices(View)] " +
                               $"Where Code = 'R-{invoice.Code}'; ";

                returnInvoice = connection.QueryFirstOrDefault<Invoice>(query);
            }

            if (returnInvoice != null)
            {
                string message = $"It's already returned in: {returnInvoice.Date:dd/MM/yyyy}!";
                MessageView.Show("Invoices", message, MessageViewButton.OK, MessageViewImage.Error);
            }
            else
            {

                using SqlConnection connection = new(Database.ConnectionString);
                string query = $"Select * From [Finance].[CustomersInvoicesItems] " +
                               $"Where InvoiceId  = {invoice.Id} ";

                ObservableCollection<InvoiceItem> invoiceItems =
                    new(connection.Query<InvoiceItem>(query));

                returnInvoice = new Invoice();
                returnInvoice.Update(invoice);
                returnInvoice.NetPrice *= -1;
                returnInvoice.VATValue *= -1;
                returnInvoice.GrossPrice *= -1;
                returnInvoice.IsReturn = true;
                returnInvoice.Code = $"R-{returnInvoice.Code}";
                returnInvoice.Date = DateTime.Now;

                _ = connection.Insert(returnInvoice);

                Invoices.Add(returnInvoice);

                foreach (InvoiceItem item in invoiceItems)
                {
                    item.InvoiceId = returnInvoice.Id;
                    item.NetPrice *= -1;
                    item.VATValue *= -1;
                    item.GrossPrice *= -1;
                    item.UnitNetPrice *= -1;
                    item.UnitVATValue *= -1;
                    item.UnitGrossPrice *= -1;
                    item.UnitOriginalPrice *= -1;
                }

                _ = connection.Insert(invoiceItems);
            }
        }
        private bool CanReturn(Invoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.IsReturn)
                return false;

            if (!UserData.ModifyJobOrdersInvoices)
                return false;

            return true;
        }
    }
}