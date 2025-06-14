using ClosedXML.Excel;
using FastMember;
using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Data;
using ProjectsNow.Windows.MessageWindows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows.Data;
using System;
using Dapper;
using System.Linq;
using System.Data;
using Microsoft.Win32;
using ProjectsNow.Services;

namespace ProjectsNow.Views.FinanceView
{

    public class DeliveryNotesViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private DeliveryNote _SelectedItem;
        private ObservableCollection<DeliveryNote> _Notes;

        private string _Code;
        private string _JobOrderCode;
        private string _DateInfo;
        private string _Panels;
        private string _Customer;
        private string _Project;

        private ICollectionView _ItemsCollection;
        public DeliveryNotesViewModel(IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;

            GetData();
            ExportCommand = new RelayCommand(Export, CanAccessExport);
            PrintCommand = new RelayCommand<DeliveryNote>(Print, CanAccessPrint);

            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData { get; set; }
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
        public DeliveryNote SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<DeliveryNote> Notes
        {
            get => _Notes;
            private set
            {
                if (SetValue(ref _Notes, value))
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

        public RelayCommand ExportCommand { get; }
        public RelayCommand<DeliveryNote> PrintCommand { get; }
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
        public string JobOrderCode
        {
            get => _JobOrderCode;
            set
            {
                if (SetValue(ref _JobOrderCode, value))
                {
                    FilterProperty = nameof(JobOrderCode);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Panels
        {
            get => _Panels;
            set
            {
                if (SetValue(ref _Panels, value))
                {
                    FilterProperty = nameof(Panels);
                    ItemsCollection.Refresh();
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
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Finance].[DeliveryNotes(View)] " +
                    $"Order By Year Desc, Number Desc";
            Notes = new ObservableCollection<DeliveryNote>(connection.Query<DeliveryNote>(query));
        }

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Notes);
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

        class ExcelDeliveryNote
        {
            public string JobOrder { get; set; }
            public string Code { get; set; }
            public string Date { get; set; }
            public int Panels { get; set; }
            public string Customer { get; set; }
            public string Project { get; set; }
        }

        private void Export()
        {
            try
            {
                List<ExcelDeliveryNote> list = new();
                foreach (DeliveryNote note in ItemsCollection.Cast<DeliveryNote>())
                {
                    ExcelDeliveryNote excelOrder = new()
                    {
                        JobOrder = note.JobOrderCode,
                        Code = note.Code,
                        Date = note.Date.ToString("dd-MM-yyyy"),
                        Panels = note.Panels,
                        Customer = note.Customer,
                        Project = note.Project,
                    };

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
                    table.Columns["JobOrder"].SetOrdinal(0);
                    table.Columns["Code"].SetOrdinal(1);
                    table.Columns["Date"].SetOrdinal(2);
                    table.Columns["Panels"].SetOrdinal(3);
                    table.Columns["Customer"].SetOrdinal(4);
                    table.Columns["Project"].SetOrdinal(5);

                    string worksheetName = $"Delivery Notes";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    workSheet.Cell(1, 1).Value = "Job Order";

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();


                    fileName = $"{DateTime.Now:dd-MM-yyyy} {worksheetName}.xlsx";
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
                    _ = MessageWindow.Show("Items", "There is no items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _ = MessageWindow.Show("Error", ex.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }
        private bool CanAccessExport()
        {
            return true;
        }

        private void Print(DeliveryNote note)
        {
            var delivery = new Data.JobOrders.Delivery()
            {
                Number = note.Code,
                Date = note.Date,
            };

            DeliveryNoteSerices.Print(note.JobOrderId, delivery, ViewData);
        }
        private bool CanAccessPrint(DeliveryNote note)
        {
            if (note == null)
                return false;

            return true;
        }
    }
}