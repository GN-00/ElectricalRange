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
    public class ProformaInvoicesViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ProformaInvoice _SelectedItem;
        private ObservableCollection<ProformaInvoice> _Invoices;
        private double _TotalInvoiced;

        private string _Code;
        private string _Items;
        private string _DateInfo;
        private string _NetPrice;
        private string _VATValue;
        private string _GrossPrice;
        private string _Amount;
        private string _Description;


        private ICollectionView _ItemsCollection;

        public ProformaInvoicesViewModel(JobOrder order, IView view)
        {
            ViewData = view;
            OrderData = order;

            GetData(order);

            AddCommand = new RelayCommand(Add, CanAdd);
            InfoCommand = new RelayCommand<ProformaInvoice>(OrderPanels, CanAccessOrderPanels);
            InvoiceCommand = new RelayCommand<ProformaInvoice>(Invoice, CanInvoice);
            ExportCommand = new RelayCommand(Export, CanExport);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserDaata => Navigation.UserData;
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
        public ProformaInvoice SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<ProformaInvoice> Invoices
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

        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand<ProformaInvoice> InfoCommand { get; }
        public RelayCommand<ProformaInvoice> InvoiceCommand { get; }
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
        public string Amount
        {
            get => _Amount;
            set
            {
                if (SetValue(ref _Amount, value))
                {
                    FilterProperty = nameof(Amount);
                    ItemsCollection.Refresh();
                    UpdateTotal();
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
            query = $"Select * From [Finance].[ProformaInvoices] " +
                    $"Where JobOrderId = {order.ID}" +
                    $"Order By Date Desc; ";
            Invoices = new ObservableCollection<ProformaInvoice>(connection.Query<ProformaInvoice>(query));

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
            TotalInvoiced = Invoices.Sum(i => i.Amount);
        }


        private void Add()
        {
            ProformaInvoice invoice = new()
            {
                JobOrderId = OrderData.ID,
                JobOrderCode = OrderData.Code,

                VAT = (double)OrderData.VAT,

                CustomerId = OrderData.CustomerID,
                CustomerName = OrderData.CustomerName,
                CustomerNameArabic = OrderData.CustomerNameArabic,
                Project = OrderData.ProjectName,
                Contact = OrderData.Contact,

                Code = "-New Invoice-",
                Date = DateTime.Now,
            };
            Navigation.To(new ProformaInvoiceView(invoice, Invoices), ViewData);
        }
        private bool CanAdd()
        {
            if (!UserDaata.ModifyJobOrdersInvoices)
                return false;

            return true;
        }

        private void Invoice(ProformaInvoice invoice)
        {
            JobOrdersInvoicesServices.PrintProformaInvoice(invoice.Id, ViewData);
        }
        private bool CanInvoice(ProformaInvoice invoice)
        {
            if (invoice == null)
                return false;

            return true;
        }

        private void OrderPanels(ProformaInvoice invoice)
        {
            Navigation.To(new ProformaInvoiceView(invoice, Invoices), ViewData);
        }
        private bool CanAccessOrderPanels(ProformaInvoice invoice)
        {
            if (invoice == null)
                return false;

            return true;
        }


        private class ExInvoice
        {
            public string Namber { get; set; }
            public string Date { get; set; }
            public int Panels { get; set; }
            public double Amount { get; set; }
            public string Description { get; set; }
        }
        private void Export()
        {
            string fileName;
            List<ExInvoice> invoices = new();

            foreach (ProformaInvoice invoice in Invoices)
            {
                invoices.Add(
                           new ExInvoice()
                           {
                               Namber = invoice.Code,
                               Date = invoice.Date.Date.ToString("dd-MM-yyyy"),
                               Panels = invoice.Panels,
                               Amount = invoice.Amount,
                               Description = invoice.Description,
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
            table.Columns["Amount"].SetOrdinal(3);
            table.Columns["Description"].SetOrdinal(4);

            string worksheetName = "Proforma Invoices";
            _ = workbook.Worksheets.Add(table, worksheetName);

            IXLWorksheet workSheet = workbook.Worksheet(worksheetName);
            _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

            _ = workSheet.Column(1).AdjustToContents();
            _ = workSheet.Column(2).AdjustToContents();
            _ = workSheet.Column(3).AdjustToContents();
            _ = workSheet.Column(4).AdjustToContents();
            _ = workSheet.Column(5).AdjustToContents();
            _ = workSheet.Column(6).AdjustToContents();

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
    }
}