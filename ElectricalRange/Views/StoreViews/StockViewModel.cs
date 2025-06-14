using ClosedXML.Excel;

using Dapper;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.JobOrdersViews;
using ProjectsNow.Windows.StoreWindows.ReturnItemsWindows;

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

namespace ProjectsNow.Views.StoreViews
{
    public class StockViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ItemStock _SelectedItem;
        private ObservableCollection<ItemStock> _Items;

        private string _Code;
        private string _Description;
        private string _Unit;
        private string _Qty;
        private string _AvgCost;
        private string _TotalAvgCost;
        private string _Brand;

        private ICollectionView _ItemsCollection;
        private JobOrder _JobOrderData;

        public StockViewModel(IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;

            if (JobOrderData == null)
            {
                _JobOrderData = new JobOrder();
                JobOrderData.Update(Database.Store);

                UserData.Access(JobOrderData);
            }

            //GetData();

            SelectCommand = new RelayCommand(Select, CanAccessSelect);
            PostCommand = new RelayCommand<JobOrder>(PostingItems, CanAccessPostingItems);
            PurchaseCommand = new RelayCommand<JobOrder>(Purchase, CanAccessPurchase);
            ReturnCommand = new RelayCommand<ItemStock>(Return, CanAccessReturn);
            ExportCommand = new RelayCommand(Export, CanAccessExport);
            GetDataCommand = new RelayCommand(GetData);

            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData { get; }
        public JobOrder JobOrderData
        {
            get => _JobOrderData;
            set
            {
                if (SetValue(ref _JobOrderData, value))
                {
                    GetData();
                }
            }
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
        public ItemStock SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<ItemStock> Items
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

        public RelayCommand SelectCommand { get; }
        public RelayCommand<JobOrder> PurchaseCommand { get; }
        public RelayCommand<JobOrder> PostCommand { get; }
        public RelayCommand<ItemStock> ReturnCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand GetDataCommand { get; }


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
        public string Description
        {
            get => _Description;
            set
            {
                if (SetValue(ref _Description, value))
                {
                    FilterProperty = nameof(Description);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Unit
        {
            get => _Unit;
            set
            {
                if (SetValue(ref _Unit, value))
                {
                    FilterProperty = nameof(Unit);
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
        public string AvgCost
        {
            get => _AvgCost;
            set
            {
                if (SetValue(ref _AvgCost, value))
                {
                    FilterProperty = nameof(AvgCost);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string TotalAvgCost
        {
            get => _TotalAvgCost;
            set
            {
                if (SetValue(ref _TotalAvgCost, value))
                {
                    FilterProperty = nameof(TotalAvgCost);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Brand
        {
            get => _Brand;
            set
            {
                if (SetValue(ref _Brand, value))
                {
                    FilterProperty = nameof(Brand);
                    ItemsCollection.Refresh();
                }
            }
        }

        private bool DataFilter(object item)
        {
            if (FilterProperty == null)
                return true;

            string columnName = FilterProperty;

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = "" + GetType().GetProperty(columnName).GetValue(this);

            if (!value.Contains(checkValue.ToUpper()))
                return false;

            return true;
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
            query = $"Select * From [Store].[JobOrderStock(AvgCost)] " +
                    $"Where JobOrderID = {JobOrderData.ID} " +
                    $"And Qty <> 0";
            Items = new ObservableCollection<ItemStock>(connection.Query<ItemStock>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += CollectionChanged;

            UpdateIndicator();
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }

        private void Select()
        {
            Navigation.OpenPopup(new SelectJobOrderView(this), PlacementMode.MousePoint, false);
        }
        private bool CanAccessSelect()
        {
            return true;
        }

        private void Purchase(JobOrder jobOrder)
        {
            Navigation.OpenPopup(new PurchaseView(jobOrder, ViewData), PlacementMode.MousePoint, false);
        }
        private bool CanAccessPurchase(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            return true;
        }

        private void PostingItems(JobOrder jobOrder)
        {
            Navigation.To(new StoreViews.InvoicesView(jobOrder), ViewData);
        }
        private bool CanAccessPostingItems(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            return true;
        }

        private void Return(ItemStock item)
        {
            ReturnItemsWindow returnItemsWindow = new()
            {
                UserData = UserData,
                ItemData = item,
                JobOrderData = JobOrderData,
            };
            _ = returnItemsWindow.ShowDialog();

            GetData();
        }
        private bool CanAccessReturn(ItemStock item)
        {
            if (item == null)
                return false;

            if (item.JobOrderID == 0)
                return false;

            return true;
        }

        private class ExcelStock
        {
            public string Code { get; set; }
            public string Description { get; set; }
            public string Unit { get; set; }
            public decimal Qty { get; set; }
            public decimal AvgCost { get; set; }
            public decimal TotalAvgCost { get; set; }
            public string Brand { get; set; }
        }
        private void Export()
        {
            try
            {
                LoadingIcon = System.Windows.Visibility.Visible;
                LoadingText = "Working...";
                Navigation.OpenLoading(LoadingIcon, LoadingText);

                List<ExcelStock> list = new();
                foreach (ItemStock item in ItemsCollection.Cast<ItemStock>())
                {
                    ExcelStock excelOrder = new()
                    {
                        Code = item.Code,
                        Description = item.Description,
                        Unit = item.Unit,
                        Qty = item.Qty,
                        AvgCost = item.AvgCost,
                        TotalAvgCost = item.TotalAvgCost,
                        Brand = item.Brand,
                    };

                    LoadingText = item.Code;
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
                    table.Columns["Code"].SetOrdinal(0);
                    table.Columns["Description"].SetOrdinal(1);
                    table.Columns["Unit"].SetOrdinal(2);
                    table.Columns["Qty"].SetOrdinal(3);
                    table.Columns["AvgCost"].SetOrdinal(4);
                    table.Columns["TotalAvgCost"].SetOrdinal(5);
                    table.Columns["Brand"].SetOrdinal(6);

                    string worksheetName = $"Stock";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    workSheet.Cell(1, 5).Value = "Avg Cost";
                    workSheet.Cell(1, 6).Value = "Total Avg Cost";

                    _ = workSheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Cell(1, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();
                    _ = workSheet.Column(7).AdjustToContents();

                    fileName = $"J.O.No. {JobOrderData.Code} {worksheetName} ({DateTime.Now:dd-MM-yyyy}).xlsx";
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
                    _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                }

                Navigation.CloseLoading();
            }
            catch (Exception ex)
            {
                _ = MessageView.Show("Error", ex.Message, MessageViewButton.OK, MessageViewImage.Warning);
                Navigation.CloseLoading();
            }
        }
        private bool CanAccessExport()
        {
            if (ItemsCollection == null)
                return false;

            if (ItemsCollection.Cast<ItemStock>().Count() == 0)
                return false;

            return true;
        }
    }
}