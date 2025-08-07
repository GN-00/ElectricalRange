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
using ProjectsNow.Windows.MessageWindows;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    public class OrdersViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Order _SelectedItem;
        private ObservableCollection<Order> _Items;

        private string _Code;
        private string _Quotation;
        private string _Customer;
        private string _Project;

        private ICollectionView _ItemsCollection;
        public OrdersViewModel(IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;

            PanelsCommand = new RelayCommand<Order>(Panels, CanAccessPanels);
            SiteWorkCommand = new RelayCommand<Order>(SiteWork, CanAccessSiteWork);
            CloseCommand = new RelayCommand<Order>(Close, CanAccessClose);
            DeliveryCommand = new RelayCommand<Order>(Delivery, CanAccessDelivery);
            AddStockCommand = new RelayCommand<Order>(AddStockToOrder, CanAccessAddStockToOrder);
            CheckItemsListCommand = new RelayCommand<Order>(CheckItemsList, CanAccessCheckItemsList);
            ExportCommand = new RelayCommand(Export, CanAccessExport);
            UpdateCommand = new RelayCommand(GetData);
            AllOrdersCommand = new RelayCommand(AllOrdersData, CanAccessAllOrdersData);

            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData { get; }
        public bool IsAllOrder { get; set; } = false;
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
        public RelayCommand<Order> DeliveryCommand { get; }
        public RelayCommand<Order> AddStockCommand { get; }
        public RelayCommand<Order> CheckItemsListCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand AllOrdersCommand { get; }
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
        public string Quotation
        {
            get => _Quotation;
            set
            {
                if (SetValue(ref _Quotation, value))
                {
                    FilterProperty = nameof(Quotation);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Customer
        {
            get => _Customer;
            set
            {
                if (SetValue(ref _Customer, value))
                {
                    FilterProperty = nameof(Customer);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Project
        {
            get => _Project;
            set
            {
                if (SetValue(ref _Project, value))
                {
                    FilterProperty = nameof(Project);
                    ItemsCollection.Refresh();
                }
            }
        }

        private bool DataFilter(object item)
        {
            if (!IsAllOrder)
            {
                if (((Order)item).IsComplete)
                    return false;
            }


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
            Navigation.To(new PanelsView(order), ViewData);
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
            Navigation.To(new ClosingRequestsView(order), ViewData);
        }
        private bool CanAccessClose(Order order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void Delivery(Order order)
        {
            Navigation.To(new DeliveryRequestsView(order), ViewData);
        }
        private bool CanAccessDelivery(Order order)
        {
            if (order == null)
                return false;

            return true;
        }


        private void AddStockToOrder(Order order)
        {
            Navigation.To(new ReceiveItemsView(order), ViewData);
        }
        private bool CanAccessAddStockToOrder(Order order)
        {
            if (order == null)
                return false;

            return true;
        }


        //private class ExcelItem
        //{
        //    public string Code { get; set; }
        //    public string Quotation { get; set; }
        //    public string Customer { get; set; }
        //    public string Project { get; set; }
        //    public int Panels { get; set; }
        //    public int Closed { get; set; }
        //    public string Note { get; set; }
        //}
        private void CheckItemsList(Order order)
        {
            Navigation.OpenLoading(Visibility.Visible, "Working....");

            OpenFileDialog path = new() { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm" };
            _ = path.ShowDialog();

            string filePath = $@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={path.FileName};" +
                              $@"Extended Properties='Excel 8.0;HDR=Yes;'";

            try
            {
                DataTable excelData = new();
                using (OleDbConnection OleDbConnection = new(filePath))
                {
                    OleDbConnection.Open();
                    OleDbDataAdapter oleAdpt = new("select Code, Description, Qty from [Sheet1$]", OleDbConnection); //here we read data from sheet1  
                    _ = oleAdpt.Fill(excelData);
                }

                if (excelData.Rows.Count == 0)
                {
                    _ = MessageWindow.Show("Data Error", "No data!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    Navigation.CloseLoading();
                }

                string query = $"SELECT * FROM [Production].[OrdersItems(View)] " +
                               $"WHERE JobOrderId = {order.JobOrderId} " +
                               $"And Type = 'Base'";
                using SqlConnection connection = new(Database.ConnectionString);
                ObservableCollection<CheckItem> orderItems = new ObservableCollection<CheckItem>(connection.Query<CheckItem>(query));

                query = $"SELECT * FROM [Production].[OrdersItems(View)] " +
                        $"WHERE JobOrderId = {order.JobOrderId} " +
                        $"And Type = 'FMR'";
                ObservableCollection<CheckItem> orderRequestItems = new ObservableCollection<CheckItem>(connection.Query<CheckItem>(query));

                List<CheckItem> excelList = new();
                for (int i = 0; i < excelData.Rows.Count; i++)
                {
                    CheckItem excelRow = new();
                    excelRow.Code = excelData.Rows[i]["Code"].ToString();
                    excelRow.Description = excelData.Rows[i]["Description"].ToString();
                    excelRow.Design = Convert.ToDouble(excelData.Rows[i]["Qty"]);

                    CheckItem item = orderItems.Where(x => x.Code == excelData.Rows[i]["Code"].ToString()).FirstOrDefault();
                    if (item != null)
                        item.Design = excelRow.Design;
                    else
                        orderItems.Add(excelRow);
                }

                List<CheckItem> missingList = [];
                foreach(CheckItem item in orderItems)
                {
                    if (item.Missing > 0)
                        missingList.Add(item);
                }

                try
                {
                    if (missingList.Count == 0)
                    {
                        _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                        return;
                    }

                    string fileName;
                    string worksheetName;
                    using XLWorkbook workbook = new();

                    if (missingList.Count != 0)
                    {
                        DataTable table = new();
                        using (ObjectReader reader = ObjectReader.Create(missingList))
                        {
                            table.Load(reader);
                        }

                        table.Columns["Code"].SetOrdinal(0);
                        table.Columns["Description"].SetOrdinal(1);
                        table.Columns["Qty"].SetOrdinal(2);
                        table.Columns["Design"].SetOrdinal(3);
                        table.Columns["Missing"].SetOrdinal(4);

                        worksheetName = $"Items Notes";
                        _ = workbook.Worksheets.Add(table, worksheetName);

                        IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                        _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        workSheet.Cell(1, 3).Value = "Factory";

                        _ = workSheet.Column(1).AdjustToContents();
                        _ = workSheet.Column(2).AdjustToContents();
                        _ = workSheet.Column(3).AdjustToContents();
                        _ = workSheet.Column(4).AdjustToContents();
                        _ = workSheet.Column(5).AdjustToContents();


                        workSheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        workSheet.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }


                    if (orderRequestItems.Count != 0)
                    {
                        DataTable table = new();
                        using (ObjectReader reader = ObjectReader.Create(orderRequestItems))
                        {
                            table.Load(reader);
                        }

                        table.Columns["Code"].SetOrdinal(0);
                        table.Columns["Description"].SetOrdinal(1);
                        table.Columns["Qty"].SetOrdinal(2);

                        worksheetName = $"Factory Request Items";
                        _ = workbook.Worksheets.Add(table, worksheetName);

                        IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                        workSheet.Column(4).Delete();
                        workSheet.Column(4).Delete();

                        _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        _ = workSheet.Column(1).AdjustToContents();
                        _ = workSheet.Column(2).AdjustToContents();
                        _ = workSheet.Column(3).AdjustToContents();



                        workSheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        workSheet.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }

                    fileName = $"{DateTime.Now:dd-MM-yyyy} JO.{order.Code} Analysis.xlsx";
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


                Navigation.CloseLoading();

            }
            catch (Exception exception)
            {
                Navigation.CloseLoading();
            }
        }
        private bool CanAccessCheckItemsList(Order order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void AllOrdersData()
        {
            IsAllOrder = !IsAllOrder;
            ItemsCollection.Filter = new Predicate<object>(DataFilter);
        }
        private bool CanAccessAllOrdersData()
        {
            return true;
        }



        private class ExcelOrder
        {
            public string Code { get; set; }
            public string Quotation { get; set; }
            public string Customer { get; set; }
            public string Project { get; set; }
            public int Panels { get; set; }
            public int Closed { get; set; }
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
                        Closed = order.ClosedPanels,
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
                    table.Columns["Closed"].SetOrdinal(5);
                    table.Columns["Note"].SetOrdinal(6);

                    worksheetName = $"Job Orders";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    workSheet.Cell(1, 1).Value = "Job Order";

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();
                    _ = workSheet.Column(7).Width = 50;


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
                    table.Columns["Closed"].SetOrdinal(5);
                    table.Columns["Note"].SetOrdinal(6);

                    worksheetName = $"Site Work";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    workSheet.Cell(1, 1).Value = "Job Order";

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();
                    _ = workSheet.Column(7).Width = 50;


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