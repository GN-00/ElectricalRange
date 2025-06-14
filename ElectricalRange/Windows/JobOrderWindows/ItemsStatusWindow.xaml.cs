using ClosedXML.Excel;

using Dapper;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class ItemsStatusWindow : Window
    {
        public JobOrder JobOrderData { get; set; }
        public ICollectionView ItemsView { get; set; }

        private ObservableCollection<ItemPurchased> jobOrderItems;
        public ItemsStatusWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Store].[JobOrdersItems(PurchaseDetails)] " +
                               $"Where JobOrderID = {JobOrderData.ID}";
                jobOrderItems = new ObservableCollection<ItemPurchased>(connection.Query<ItemPurchased>(query));
            }

            ItemsView = CollectionViewSource.GetDefaultView(jobOrderItems);

            ItemsView.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("Status"));
            ItemsView.CollectionChanged += CollectionChanged;

            DataContext = new { JobOrderData, ItemsView };

            CollectionChanged(null, null);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = RecordsList.SelectedIndex;
            Navigation.Text = selectedIndex == -1 ? $"{ItemsView.Cast<object>().Count()}" : $"{selectedIndex + 1} / {ItemsView.Cast<object>().Count()}";
        }
        private void SelectedChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            CollectionChanged(null, null);
        }

        private void ExportItems_Click(object sender, RoutedEventArgs e)
        {
            ExportPopup.IsOpen = false;

            string fileName;
            int i = 0;
            List<ExcelItem> items = new();
            foreach (ItemPurchased item in jobOrderItems)
            {
                items.Add
                    (
                    new ExcelItem()
                    {
                        SN = ++i,
                        Code = item.Code,
                        Description = item.Description,
                        Unit = item.Unit,
                        Qty = item.Qty,
                        InOrderQty = item.InOrderQty,
                        PurchasedQty = item.PurchasedQty,
                        DamagedQty = item.DamagedQty,
                        RemainingQty = item.RemainingQty,
                        Status = item.Status,
                    }
                    ); 
            }

            using XLWorkbook workbook = new();
            DataTable table = new();
            using (ObjectReader reader = ObjectReader.Create(items))
            {
                table.Load(reader);
            }
            table.Columns["SN"].SetOrdinal(0);
            table.Columns["Code"].SetOrdinal(1);
            table.Columns["Description"].SetOrdinal(2);
            table.Columns["Unit"].SetOrdinal(3);
            table.Columns["Qty"].SetOrdinal(4);
            table.Columns["InOrderQty"].SetOrdinal(5);
            table.Columns["PurchasedQty"].SetOrdinal(6);
            table.Columns["DamagedQty"].SetOrdinal(7);
            table.Columns["RemainingQty"].SetOrdinal(8);
            table.Columns["Status"].SetOrdinal(9);

            string worksheetName = "Items";
            _ = workbook.Worksheets.Add(table, worksheetName);

            IXLWorksheet workSheet = workbook.Worksheet(worksheetName);
            _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            workSheet.Cell(1, 6).Value = "In Order";
            workSheet.Cell(1, 7).Value = "Have";
            workSheet.Cell(1, 8).Value = "Damaged";
            workSheet.Cell(1, 9).Value = "Remaining";

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

            fileName = $"J.O.No. {JobOrderData.Code} {worksheetName} Status.xlsx";
            fileName = fileName.Replace("/", "-");

            SaveFileDialog saveFileDialog = new()
            {
                FileName = fileName,
                DefaultExt = ".xlsx",
                Filter = "Excel Worksheets|*.xlsx",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageWindows.MessageWindow.Show("Error", ex.Message);
                }
            }

        }

        private void ExportMissing_Click(object sender, RoutedEventArgs e)
        {
            ExportPopup.IsOpen = false;

            string fileName;
            int i = 0;
            List<ExcelItem> items = new();
            foreach (ItemPurchased item in jobOrderItems.Where(n => n.RemainingQty > 0))
            {
                items.Add
                    (
                    new ExcelItem()
                    {
                        SN = ++i,
                        Code = item.Code,
                        Description = item.Description,
                        Unit = item.Unit,
                        Qty = item.Qty,
                        InOrderQty = item.InOrderQty,
                        PurchasedQty = item.PurchasedQty,
                        DamagedQty = item.DamagedQty,
                        RemainingQty = item.RemainingQty,
                        Status = item.Status,
                    }
                    );
            }

            using XLWorkbook workbook = new();
            DataTable table = new();
            using (ObjectReader reader = ObjectReader.Create(items))
            {
                table.Load(reader);
            }
            table.Columns["SN"].SetOrdinal(0);
            table.Columns["Code"].SetOrdinal(1);
            table.Columns["Description"].SetOrdinal(2);
            table.Columns["Unit"].SetOrdinal(3);
            table.Columns["Qty"].SetOrdinal(4);
            table.Columns["InOrderQty"].SetOrdinal(5);
            table.Columns["PurchasedQty"].SetOrdinal(6);
            table.Columns["DamagedQty"].SetOrdinal(7);
            table.Columns["RemainingQty"].SetOrdinal(8);
            table.Columns["Status"].SetOrdinal(9);

            string worksheetName = "Missing";
            _ = workbook.Worksheets.Add(table, worksheetName);

            IXLWorksheet workSheet = workbook.Worksheet(worksheetName);
            _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            _ = workSheet.Column(10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            workSheet.Cell(1, 6).Value = "In Order";
            workSheet.Cell(1, 7).Value = "Have";
            workSheet.Cell(1, 8).Value = "Damaged";
            workSheet.Cell(1, 9).Value = "Remaining";

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

            fileName = $"J.O.No. {JobOrderData.Code} {worksheetName} Status.xlsx";
            fileName = fileName.Replace("/", "-");

            SaveFileDialog saveFileDialog = new()
            {
                FileName = fileName,
                DefaultExt = ".xlsx",
                Filter = "Excel Worksheets|*.xlsx",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageWindows.MessageWindow.Show("Error", ex.Message);
                }
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            ExportPopup.IsOpen = true;
        }

        #region Filters
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is ItemPurchased item)
                {
                    if (item.PurchasedQty + item.InOrderQty >= item.Qty)
                    {
                        e.Accepted = false;
                        return;
                    }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }
        #endregion


        public class ExcelItem
        {
            public int SN { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public string Unit { get; set; }
            public decimal Qty { get; set; }
            public decimal PurchasedQty { get; set; }
            public decimal DamagedQty { get; set; }
            public decimal InOrderQty { get; set; }
            public decimal RemainingQty { get; set; }
            public string Status { get; set; }
        }
    }
}
