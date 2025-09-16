using ClosedXML.Excel;

using Dapper;
using Dapper.Contrib.Extensions;

using FastMember;

using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.References;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;
using ProjectsNow.Windows.ReferencesWindows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.ReferencesViews
{
    public class ReferencesViewModel : ViewModelBase
    {
        private string _Code;
        private string _Description;
        private string _Unit;
        private string _Cost;
        private string _Discount;
        private string _Brand;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Reference _SelectedItem;
        private ObservableCollection<Reference> _Items;

        private ICollectionView _ItemsView;
        public ReferencesViewModel(IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            GetData();

            NewCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Reference>(Edit, CanEdit);
            DeleteCommand = new RelayCommand<Reference>(Delete, CanDelete);
            DiscountCommand = new RelayCommand(UpdateDiscount, CanAccessUpdateDiscount);
            PricesCommand = new RelayCommand(Prices, CanAccessPrices);
            AddCodesCommand = new RelayCommand(AddCodes);
            CopperCommand = new RelayCommand(Copper, CanAccessCopper);
            HistoryCommand = new RelayCommand<Reference>(History, CanAccessHistory);

            ClosingCommand = new RelayCommand(Closing, CanAccessClosing);
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
        public Reference SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Reference> Items
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
        public ICollectionView ItemsView
        {
            get => _ItemsView;
            set => SetValue(ref _ItemsView, value);
        }

        public RelayCommand NewCommand { get; }
        public RelayCommand<Reference> EditCommand { get; }
        public RelayCommand<Reference> DeleteCommand { get; }
        public RelayCommand DiscountCommand { get; }
        public RelayCommand PricesCommand { get; }
        public RelayCommand AddCodesCommand { get; }
        public RelayCommand CopperCommand { get; }
        public RelayCommand<Reference> HistoryCommand { get; }
        public RelayCommand ClosingCommand { get; }

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
                    ItemsView.Refresh();
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
                    ItemsView.Refresh();
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
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Cost
        {
            get => _Cost;
            set
            {
                if (SetValue(ref _Cost, value))
                {
                    FilterProperty = nameof(Cost);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Discount
        {
            get => _Discount;
            set
            {
                if (SetValue(ref _Discount, value))
                {
                    FilterProperty = nameof(UpdateDiscount);
                    ItemsView.Refresh();
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
                    ItemsView.Refresh();
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
            ItemsView.Refresh();
        }

        #endregion

        private void GetData()
        {
            Items = AppData.ReferencesListData;
            if (Items == null)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                Items =
                    AppData.ReferencesListData =
                        new ObservableCollection<Reference>(ReferenceController.GetReferences(connection));
            }
        }
        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.Filter = new Predicate<object>(DataFilter);
            ItemsView.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsView.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsView);
        }


        private void Add()
        {
            Reference reference = new();
            Navigation.To(new ReferenceView(reference, Items),ViewData);
        }
        private bool CanAdd()
        {
            return true;
        }

        private void Edit(Reference reference)
        {
            Navigation.To(new ReferenceView(reference, Items),ViewData);
        }
        private bool CanEdit(Reference reference)
        {
            if (reference == null)
                return false;

            return true;
        }

        private void Delete(Reference reference)
        {
            if (reference.Type == "Smart")
            {
                _ = MessageWindow.Show("Delete", "Can't delete this reference!!");
                return;
            }

            MessageBoxResult result = MessageWindow.Show("Deleting", $"Are you sure to delete:\n{reference.Code}\n{reference.Description} ?!", MessageWindowButton.YesNo, MessageWindowImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Delete(reference);
                }

                _ = Items.Remove(reference);
            }
        }
        private bool CanDelete(Reference reference)
        {
            return CanEdit(reference);
        }

        private void UpdateDiscount()
        {
            CategoriesDiscountsWindow categoriesDiscountsWindow = new();
            _ = categoriesDiscountsWindow.ShowDialog();
        }
        private bool CanAccessUpdateDiscount()
        {
            if (!UserData.ReferencesDiscount)
                return false;

            return true;
        }

        private class ExcelRow
        {
            public string Code { get; set; }
            public decimal Cost { get; set; }
        }

        private class AddExcelRow
        {
            public string Code { get; set; }
            public string  Description { get; set; }
            public decimal Cost { get; set; }
        }

        private void Prices()
        {
            UpdatePricesGuideWindow updatePricesGuideWindow = new();
            updatePricesGuideWindow.ShowDialog();

            Navigation.OpenLoading(Visibility.Visible, "Working....");

            OpenFileDialog path = new() { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm" };
            _ = path.ShowDialog();

            string filePath = $@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={path.FileName};" +
                              $@"Extended Properties='Excel 8.0;HDR=Yes;'";

            int errorIndex = 0;
            try
            {
                DataTable excelData = new();
                using (OleDbConnection connection = new(filePath))
                {
                    connection.Open();
                    OleDbDataAdapter oleAdpt = new("select Code, Cost from [Sheet1$]", connection); //here we read data from sheet1  
                    _ = oleAdpt.Fill(excelData);
                }

                if (excelData.Rows.Count == 0)
                {
                    _ = MessageWindow.Show("Data Error", "No data!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    return;
                }

                List<ExcelRow> excelList = new();
                for (int i = 0; i < excelData.Rows.Count; i++)
                {
                    ExcelRow excelRow = new();
                    excelRow.Code = excelData.Rows[i]["Code"].ToString();

                    excelRow.Cost = Convert.ToDecimal(excelData.Rows[i]["Cost"]);
                    excelList.Add(excelRow);

                    errorIndex++;
                }

                int itemsCount = 0;
                string updateTable = "";
                foreach (ExcelRow row in excelList)
                {
                    List<Reference> itemsToUpdate = Items.Where(r => r.Code.Trim() == row.Code.Trim()).ToList();
                    foreach (Reference reference in itemsToUpdate)
                    {
                        itemsCount++;
                        reference.Cost = row.Cost;
                        updateTable += $"Update [Reference].[References] Set Cost = {reference.Cost} Where ReferenceID = {reference.ReferenceID}; ";
                    }

                }

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Execute(updateTable);
                }

                _ = MessageWindow.Show("Data", $"({itemsCount}) references updated!", MessageWindowButton.OK, MessageWindowImage.Information);
            }
            catch (Exception exception)
            {
                _ = MessageWindow.Show($"Error-{errorIndex}", exception.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
            }

            Navigation.CloseLoading();
        }
        private bool CanAccessPrices()
        {
            return true;
        }

        private void Copper()
        {
            UpdateCopperWindow updateCopperWindow = new(Items);
            updateCopperWindow.ShowDialog();
        }
        private bool CanAccessCopper()
        {
            return true;
        }

        private void Closing()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            AppData.ReferencesListData =
                ReferenceController.GetReferences(connection);
        }
        private bool CanAccessClosing()
        {
            return AppData.ReferencesListData != null;
        }


        private void AddCodes()
        {
            UpdatePricesGuideWindow updatePricesGuideWindow = new();
            updatePricesGuideWindow.ShowDialog();

            Navigation.OpenLoading(Visibility.Visible, "Working....");

            OpenFileDialog path = new() { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm" };
            _ = path.ShowDialog();

            string filePath = $@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={path.FileName};" +
                              $@"Extended Properties='Excel 8.0;HDR=Yes;'";

            try
            {
                DataTable excelData = new();
                using (OleDbConnection oleDConnection = new(filePath))
                {
                    oleDConnection.Open();
                    OleDbDataAdapter oleAdpt = new("select Code, Cost, Description from [Sheet1$]", oleDConnection); //here we read data from sheet1  
                    _ = oleAdpt.Fill(excelData);
                }

                if (excelData.Rows.Count == 0)
                {
                    _ = MessageWindow.Show("Data Error", "No data!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    return;
                }

                List<AddExcelRow> excelList = new();
                for (int i = 0; i < excelData.Rows.Count; i++)
                {
                    AddExcelRow excelRow = new();
                    excelRow.Code = excelData.Rows[i]["Code"].ToString();
                    excelRow.Description = excelData.Rows[i]["Description"].ToString();

                    excelRow.Cost = Convert.ToDecimal(excelData.Rows[i]["Cost"]);
                    excelList.Add(excelRow);
                }

                using SqlConnection connection = new(Database.ConnectionString);

                int itemsUpdateCount = 0;
                int itemsAddingCount = 0;
                string updateTable = "";
                foreach (AddExcelRow row in excelList)
                {
                    string code = connection.QueryFirstOrDefault<string>($"Select Code From [Reference].[References] Where Code ='{row.Code}'");
                    if (code == null)
                    {
                        Reference reference = new()
                        {
                            Code = row.Code,
                            Cost = row.Cost,
                            Description = row.Description,
                            Unit = "No",
                            Type = null,
                            Brand = "Siemens",
                            Category = "Siemens",
                            Article1 = null,
                            Article2 = null,
                            Discount = 50,
                            SearchKeys = "Siemens"
                        };
                        _ = connection.Insert(reference);
                        itemsAddingCount++;
                    }
                    else
                    {
                        itemsUpdateCount++;
                        updateTable = $"Update [Reference].[References] Set Cost = {row.Cost} Where Code = '{row.Code}'; ";
                        _ = connection.Execute(updateTable);
                    }
                }

                _ = MessageWindow.Show("Data", $" Adding: {itemsAddingCount}\n Update: {itemsUpdateCount} \n references updated!", MessageWindowButton.OK, MessageWindowImage.Information);
            }
            catch (Exception exception)
            {
                _ = MessageWindow.Show("Error", exception.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
            }

            Navigation.CloseLoading();
        }

        private class ExcelItem
        {
            public string PO { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public double Qty { get; set; }
            public DateTime Date { get; set; }
        }
        private void History(Reference reference)
        {
            try
            {
                List<ExcelItem> items = [];
                using SqlConnection connection = new(Database.ConnectionString);
                string query = $"Select * From [Purchase].[Items(History)] Where Code = '{reference.Code}' Order By Date";
                items = connection.Query<ExcelItem>(query).ToList();

                if (items.Count == 0)
                {
                    _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                    return;
                }

                string fileName;
                string worksheetName = $"Purchase Order History";
                using XLWorkbook workbook = new();

                if (items.Count != 0)
                {
                    DataTable table = new();
                    using (ObjectReader reader = ObjectReader.Create(items))
                    {
                        table.Load(reader);
                    }

                    table.Columns["PO"].SetOrdinal(0);
                    table.Columns["Code"].SetOrdinal(1);
                    table.Columns["Description"].SetOrdinal(2);
                    table.Columns["Qty"].SetOrdinal(3);
                    table.Columns["Date"].SetOrdinal(4);

                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    workSheet.Cell(1, 1).Value = "Purchase Order";

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();

                    workSheet.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    workSheet.Cell(1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }

                fileName = $"{DateTime.Now:dd-MM-yyyy} {reference.Code} History.xlsx";
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


        private bool CanAccessHistory(Reference reference)
        {
            if (reference == null)
                return false;

            return true;
        }
    }
}