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
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    public class PanelsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ProductionPanel _SelectedItem;
        private ObservableCollection<ProductionPanel> _Items;

        private string _SN;
        private string _Name;
        private string _Qty;
        private string _EnclosureType;
        private string _EnclosureHeight;
        private string _EnclosureWidth;
        private string _EnclosureDepth;
        private string _EnclosureIP;

        private ICollectionView _ItemsCollection;

        public PanelsViewModel(Order order, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            OrderData = order;

            GetData();

            CheckItemsCommand = new RelayCommand(CheckItems);
            CopyNameCommand = new RelayCommand(CopyName);
            AddItemsCommand = new RelayCommand<ProductionPanel>(AddItems, CanAccessAddItems);
            ItemsCommand = new RelayCommand<ProductionPanel>(GetItems, CanAccessGetItems);
            ClosingCommand = new RelayCommand(Closing, CanAccessClosing);
            ExportCommand = new RelayCommand(Export, CanAccessExport);

            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }


        public User UserData { get; }
        public Order OrderData { get; }

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
        public ProductionPanel SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<ProductionPanel> Items
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
        public RelayCommand CopyNameCommand { get; }
        public RelayCommand<ProductionPanel> AddItemsCommand { get; }
        public RelayCommand<ProductionPanel> ItemsCommand { get; }
        public RelayCommand CheckItemsCommand { get; }
        public RelayCommand ClosingCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string SN
        {
            get => _SN;
            set
            {
                if (SetValue(ref _SN, value))
                {
                    FilterProperty = nameof(SN);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Name
        {
            get => _Name;
            set
            {
                if (SetValue(ref _Name, value))
                {
                    FilterProperty = nameof(Name);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Qty
        {
            get => _Qty;
            set
            {
                if (SetValue(ref _Qty, value))
                {
                    FilterProperty = nameof(Qty);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureType
        {
            get => _EnclosureType;
            set
            {
                if (SetValue(ref _EnclosureType, value))
                {
                    FilterProperty = nameof(EnclosureType);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureHeight
        {
            get => _EnclosureHeight;
            set
            {
                if (SetValue(ref _EnclosureHeight, value))
                {
                    FilterProperty = nameof(EnclosureHeight);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureWidth
        {
            get => _EnclosureWidth;
            set
            {
                if (SetValue(ref _EnclosureWidth, value))
                {
                    FilterProperty = nameof(EnclosureWidth);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureDepth
        {
            get => _EnclosureDepth;
            set
            {
                if (SetValue(ref _EnclosureDepth, value))
                {
                    FilterProperty = nameof(EnclosureDepth);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureIP
        {
            get => _EnclosureIP;
            set
            {
                if (SetValue(ref _EnclosureIP, value))
                {
                    FilterProperty = nameof(EnclosureIP);
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
            string query = $"SELECT * FROM [Production].[Panels(View)] WHERE JobOrderId = {OrderData.JobOrderId}";
            using SqlConnection connection = new(Database.ConnectionString);
            Items = new ObservableCollection<ProductionPanel>(connection.Query<ProductionPanel>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("SN", ListSortDirection.Ascending));
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

        public void CopyName()
        {
            if (SelectedItem == null)
                return;
            Clipboard.SetText(SelectedItem.Name);
        }

        public void AddItems(ProductionPanel panel)
        {
            Services.ProductionServices.AddItems(panel);
        }
        private bool CanAccessAddItems(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }


        private void GetItems(ProductionPanel panel)
        {
            Navigation.To(new ItemsView(panel), ViewData);
        }
        private bool CanAccessGetItems(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void Closing()
        {
            Navigation.To(new ClosingRequestsView(OrderData), ViewData);
        }
        private bool CanAccessClosing()
        {
            return true;
        }

        private void CheckItems()
        {
            string query;
            List<OrderItem> orderItems = [];
            List<ProductionPanel> closedPanels = [];
            List<ProductionPanel> RunningPanels = [];
            using SqlConnection connection = new(Database.ConnectionString);
            foreach (ProductionPanel panel in Items)
            {
                panel.MissingItems = panel.Items - panel.ReceivedItems;
                query = $"Delete From [Production].[PanelsItems(Used)] Where PanelId = {panel.PanelId}";
                connection.Execute(query);
                if (panel.ClosedQty == panel.Qty)
                    closedPanels.Add(panel);
                else
                    RunningPanels.Add(panel);

                query = $"Select * From [Production].[PanelsItems(View)] " +
                        $"Where PanelId = {panel.PanelId} " +
                        $"Order By Code";
                panel.ItemsList = connection.Query<Item>(query).ToList();

                foreach (Item item in panel.ItemsList)
                {
                    if (item.StockQty < item.Qty)
                        panel.MissingItems += item.Qty - item.StockQty;
                }

                //query = $"Update [JobOrder].[Panels(InProduction)] Set Missing = {panel.MissingItems} Where PanelId = {panel.PanelId};";
                //connection.Execute(query);
            }

            query = $"Select * From [Production].[OrdersItems(View)] Where JobOrderId = {OrderData.JobOrderId}";
            orderItems = [.. connection.Query<OrderItem>(query)];

            UpdateUsedItems(orderItems, closedPanels, connection);

            RunningPanels = RunningPanels.OrderBy(p => p.MissingItems).ToList();
            UpdateUsedItems(orderItems, RunningPanels, connection);

        }

        private void UpdateUsedItems(List<OrderItem> orderItems, List<ProductionPanel> panels, SqlConnection connection)
        {
            foreach (ProductionPanel panel in panels)
            {
                foreach (Item item in panel.ItemsList)
                {
                    UsedItem itemUsed = new();
                    OrderItem orderItem = orderItems.FirstOrDefault(i => i.Code == item.Code);
                    if (orderItem != null)
                    {
                        if (item.StockQty < item.Qty)
                        {
                            itemUsed.Qty = orderItem.Stock;
                            orderItem.Stock -= orderItem.Stock;
                        }
                        else
                        {
                            itemUsed.Qty = item.Qty;
                            orderItem.Stock -= item.Qty;
                        }
                    }

                    itemUsed.JobOrderId = OrderData.JobOrderId;
                    itemUsed.PanelId = panel.PanelId;
                    itemUsed.Code = item.Code;
                    itemUsed.Description = item.Description;
                    itemUsed.Date = DateTime.Now;
                    connection.Insert(itemUsed);
                }
            }
        }

        private class ExcelPanel
        {
            public int SN { get; set; }
            public string Name { get; set; }
            public int Qty { get; set; }
            public int ClosedQty { get; set; }
            public string Type { get; set; }
            public decimal? Height { get; set; }
            public decimal? Width { get; set; }
            public decimal? Depth { get; set; }
            public string IP { get; set; }
            public string Note { get; set; }
        }
        private void Export()
        {
            try
            {
                List<ExcelPanel> excelPanels = [];
                if (ItemsCollection.Cast<ProductionPanel>().ToList().Count == 0)
                {
                    _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                    return;
                }

                foreach (ProductionPanel panel in ItemsCollection.Cast<ProductionPanel>())
                {
                    ExcelPanel excelPanel = new()
                    {
                        SN = panel.SN,
                        Name = panel.Name,
                        Qty = panel.Qty,
                        ClosedQty = panel.ClosedQty,
                        Type = panel.EnclosureType,
                        Height = panel.EnclosureHeight,
                        Width = panel.EnclosureWidth,
                        Depth = panel.EnclosureDepth,
                        IP = panel.EnclosureIP,
                        Note = null
                    };

                    excelPanels.Add(excelPanel);
                }



                string fileName;
                string worksheetName = $"Job Orders";
                using XLWorkbook workbook = new();

                if (excelPanels.Count != 0)
                {
                    DataTable table = new();
                    using (ObjectReader reader = ObjectReader.Create(excelPanels))
                    {
                        table.Load(reader);
                    }
                    table.Columns["SN"].SetOrdinal(0);
                    table.Columns["Name"].SetOrdinal(1);
                    table.Columns["Qty"].SetOrdinal(2);
                    table.Columns["ClosedQty"].SetOrdinal(3);
                    table.Columns["Type"].SetOrdinal(4);
                    table.Columns["Height"].SetOrdinal(5);
                    table.Columns["Width"].SetOrdinal(6);
                    table.Columns["Depth"].SetOrdinal(7);
                    table.Columns["IP"].SetOrdinal(8);
                    table.Columns["Note"].SetOrdinal(9);

                    worksheetName = $"J.O {OrderData.Code}";
                    worksheetName = worksheetName.Replace("/", "-");
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    workSheet.Cell(1, 4).Value = "Closed Qty";

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();
                    _ = workSheet.Column(7).AdjustToContents();
                    _ = workSheet.Column(8).AdjustToContents();
                    _ = workSheet.Column(9).AdjustToContents();
                    _ = workSheet.Column(10).Width = 50;


                    workSheet.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }
                else
                {
                    _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                    return;
                }


                fileName = $"J.O {OrderData.Code} Panels.xlsx";
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