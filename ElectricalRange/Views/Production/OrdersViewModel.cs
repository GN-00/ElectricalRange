using ClosedXML.Excel;
using Dapper;
using Dapper.Contrib.Extensions;
using FastMember;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Data.Users;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    public class OrdersViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Order _SelectedItem;
        private ObservableCollection<Order> _Items;

        private int? _SelectedYear;

        private string _Code;
        private string _QuotationCode;
        private string _CustomerName;
        private string _ProjectName;
        private string _EstimationName;
        private string _SalesmanName;

        private ICollectionView _ItemsCollection;
        public OrdersViewModel(IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;

            GetData();

            PanelsCommand = new RelayCommand<Order>(Panels, CanAccessPanels);
            SiteWorkCommand = new RelayCommand<Order>(SiteWork, CanAccessSiteWork);
            CloseCommand = new RelayCommand<Order>(Close, CanAccessClose);
            ExportCommand = new RelayCommand(Export, CanAccessExport);

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
        public Order SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Order> Items
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

        public RelayCommand<Order> PanelsCommand { get; }
        public RelayCommand<Order> SiteWorkCommand { get; }
        public RelayCommand<Order> CloseCommand { get; }
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
        public string ProjectName
        {
            get => _ProjectName;
            set
            {
                if (SetValue(ref _ProjectName, value))
                {
                    FilterProperty = nameof(ProjectName);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EstimationName
        {
            get => _EstimationName;
            set
            {
                if (SetValue(ref _EstimationName, value))
                {
                    FilterProperty = nameof(EstimationName);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string SalesmanName
        {
            get => _SalesmanName;
            set
            {
                if (SetValue(ref _SalesmanName, value))
                {
                    FilterProperty = nameof(SalesmanName);
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
            using SqlConnection connection = new(Database.ConnectionString);
            Items = new ObservableCollection<Order>(
                connection.Query<Order>(
                    $"Select * From [Production].[Orders(View)] Order By CodeYear Desc, CodeNumber Desc"));
        }

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

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

        private void Panels(Order order)
        {
            Navigation.To(new OrderPanelsView(order), ViewData);
        }
        private bool CanAccessPanels(Order order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void SiteWork(Order order)
        {
            order.IsSiteWork = true;
            using SqlConnection connection = new(Database.ConnectionString);
            connection.Update(order);
        }
        private bool CanAccessSiteWork(Order order)
        {
            if (order == null)
                return false;

            if (order.IsSiteWork)
                return false;

            return true;
        }

        private void Close(Order order)
        {
            order.IsClosed = true;
            order.CloseDate = DateTime.Now;
            using SqlConnection connection = new(Database.ConnectionString);
            connection.Update(order);
        }
        private bool CanAccessClose(Order order)
        {
            if (order == null)
                return false;

            if (order.IsClosed)
                return false;

            return true;
        }

        private class ExcelOrder
        {
            public string Code { get; set; }
            public string Quotation { get; set; }
            public string Customer { get; set; }
            public string Project { get; set; }
            public int Panels { get; set; }
            public string Note { get; set; }
        }
        private void Export()
        {
            try
            {
                List<ExcelOrder> factorylist = [];
                List<ExcelOrder> siteWorklist = [];

                if (ItemsCollection.Cast<Order>().ToList().Count == 0)
                {
                    _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                    return;
                }

                foreach (Order order in ItemsCollection.Cast<Order>())
                {
                    if (order.IsClosed)
                        continue;

                    ExcelOrder excelOrder = new()
                    {
                        Code = order.Code,
                        Quotation = order.Quotation,
                        Customer = order.Customer,
                        Project = order.Project,
                        Panels = order.Panels,
                    };

                    if (order.IsSiteWork)
                        siteWorklist.Add(excelOrder);
                    else
                        factorylist.Add(excelOrder);
                }

                if (factorylist.Count == 0 && siteWorklist.Count == 0)
                {
                    _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                    return;
                }

                string fileName;
                string worksheetName = $"Job Orders";
                using XLWorkbook workbook = new();

                if (factorylist.Count != 0)
                {
                    DataTable table = new();
                    using (ObjectReader reader = ObjectReader.Create(factorylist))
                    {
                        table.Load(reader);
                    }
                    table.Columns["Code"].SetOrdinal(0);
                    table.Columns["Quotation"].SetOrdinal(1);
                    table.Columns["Customer"].SetOrdinal(2);
                    table.Columns["Project"].SetOrdinal(3);
                    table.Columns["Panels"].SetOrdinal(4);

                    worksheetName = $"Job Orders";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    workSheet.Cell(1, 1).Value = "Job Order";

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).Width = 50;


                    workSheet.Cell(1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    workSheet.Cell(1, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }


                if (siteWorklist.Count != 0)
                {
                    DataTable table = new();
                    using (ObjectReader reader = ObjectReader.Create(siteWorklist))
                    {
                        table.Load(reader);
                    }
                    table.Columns["Code"].SetOrdinal(0);
                    table.Columns["Quotation"].SetOrdinal(1);
                    table.Columns["Customer"].SetOrdinal(2);
                    table.Columns["Project"].SetOrdinal(3);
                    table.Columns["Panels"].SetOrdinal(4);

                    worksheetName = $"Site Work";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    workSheet.Cell(1, 1).Value = "Job Order";

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).Width = 50;


                    workSheet.Cell(1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    workSheet.Cell(1, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                }

                fileName = $"{DateTime.Now:dd-MM-yyyy} Job Orders.xlsx";
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
            catch (Exception ex)
            {
                _ = MessageView.Show("Error", ex.Message, MessageViewButton.OK, MessageViewImage.Warning);
            }
        }
        private bool CanAccessExport()
        {
            return true;
        }
    }
}