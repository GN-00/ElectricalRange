using ClosedXML.Excel;
using Dapper;
using FastMember;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    internal class ReceiveItemsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private OrderItem _SelectedItem;
        private ObservableCollection<OrderItem> _Items;
        private ObservableCollection<AddStock> _ItemsToAdd = [];
        private ICollectionView _ItemsCollection;

        public ReceiveItemsViewModel(Order order, IView view)
        {
            OrderData = order;

            GetData();

            AddCommand = new RelayCommand<OrderItem>(Add, CanAdd);
            ExportCommand = new RelayCommand(Export, CanAccessExport);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }


        public Order OrderData { get; private set; }

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
        public OrderItem SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<OrderItem> Items
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
        public ObservableCollection<AddStock> ItemsToAdd
        {
            get => _ItemsToAdd;
            private set
            {
                if (SetValue(ref _ItemsToAdd, value))
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

        public RelayCommand<OrderItem> AddCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand CancelCommand { get; }


        private void GetData()
        {
            string query = $"SELECT * FROM [Production].[OrdersItems(View)] WHERE JobOrderId = {OrderData.JobOrderId}";
            using SqlConnection connection = new(Database.ConnectionString);
            Items = new ObservableCollection<OrderItem>(connection.Query<OrderItem>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += CollectionChanged;
        }
        private bool DataFilter(object obj)
        {
            if (!(obj is OrderItem item))
                return false;

            double value = item.Stock;
            double checkValue = item.Qty;

            if (value >= checkValue)
                return false;

            if (ItemsToAdd.Any(x => x.Code == item.Code))
                return false;

            return true;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }

        private void Add(OrderItem item)
        {
            Navigation.OpenPopup(new ItemQtyView(item), PlacementMode.Center, true);
            Navigation.ClosePopupEvent += UpdateList;
        }
        private void UpdateList()
        {
            ItemsCollection.Refresh();
        }
        private bool CanAdd(OrderItem item)
        {
            if (item == null)
                return false;

            if (item.Missing <= 0)
                return false;

            return true;
        }

        private void Cancel()
        {
            Navigation.Back();
        }
        private bool CanCancel()
        {
            return true;
        }


        public class ExcelItem
        {
            public string JobOrder { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public double Qty { get; set; }
            public double Stock { get; set; }
            public double Missing { get; set; }
            public double Percent { get; set; }
        }
        private void Export()
        {
            try
            {
                List<ExcelItem> excelItems = [];
                foreach (OrderItem item in Items)
                {
                    ExcelItem excelItem = new()
                    {
                        JobOrder = OrderData.Code,
                        Code = item.Code,
                        Description = item.Description,
                        Qty = item.Qty,
                        Stock = item.Stock,
                        Missing = item.Missing,
                        Percent = item.Percent
                    };

                    excelItems.Add(excelItem);
                }

                string fileName;
                string worksheetName = $"Items";
                using XLWorkbook workbook = new();

                if (Items.Count != 0)
                {
                    DataTable table = new();
                    using (ObjectReader reader = ObjectReader.Create(excelItems))
                    {
                        table.Load(reader);
                    }

                    table.Columns["JobOrder"].SetOrdinal(0);
                    table.Columns["Code"].SetOrdinal(1);
                    table.Columns["Description"].SetOrdinal(2);
                    table.Columns["Qty"].SetOrdinal(3);
                    table.Columns["Stock"].SetOrdinal(4);
                    table.Columns["Missing"].SetOrdinal(5);
                    table.Columns["Percent"].SetOrdinal(6);

                    _ = workbook.Worksheets.Add(table, worksheetName);
                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
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
                    _ = workSheet.Column(7).AdjustToContents();

                    workSheet.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }
                else
                {
                    _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                    return;
                }


                fileName = $"J.O {OrderData.Code} Items.xlsx";
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