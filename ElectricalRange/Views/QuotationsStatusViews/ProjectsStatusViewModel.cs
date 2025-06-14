using ClosedXML.Excel;

using Dapper;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.QuotationsStatus;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;

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

namespace ProjectsNow.Views.QuotationsStatusViews
{
    public class ProjectsStatusViewModel : ViewModelBase
    {
        private string _StatusGroup;
        private string _Status;
        private string _StatusInfo;
        private string _EstimationID;
        private string _SalesmanID;
        private ITable _CurrentView;

        private DateTime? _StartDate = DateTime.Parse($"{DateTime.Today.Year}-01-01");
        private DateTime? _EndDate = DateTime.Today;

        private ITable _InquiriesView = new OnPipeView();
        private string _InquiriesIndicator = "-";
        private int _SelectedInquiryIndex;
        private Inquiry _SelectedInquiry;
        private ObservableCollection<Inquiry> _Inquiries;

        private ITable _QuotationsView = new QuotationsView();
        private string _QuotationsIndicator = "-";
        private int _SelectedQuotationIndex;
        private Quotation _SelectedQuotation;
        private ObservableCollection<Quotation> _Quotations;
        private ICollectionView _InquiriesCollection;
        private ICollectionView _QuotationsCollection;

        public ProjectsStatusViewModel(IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            GetData();
            LoadCommand = new RelayCommand(Load, CanAccessLoad);
            OnPipeCommand = new RelayCommand(OnPipe, CanAccessOnPipe);
            QuotationsCommand = new RelayCommand(AllQuotations, CanAccessAllQuotations);

            WinCommand = new RelayCommand(Win, CanAccessWin);
            OnGoingCommand = new RelayCommand(OnGoing, CanAccessOnGoing);
            HoldCommand = new RelayCommand(Hold, CanAccessHold);
            CancelCommand = new RelayCommand(Cancel, CanAccessCancel);
            LostCommand = new RelayCommand(Lost, CanAccessLost);
            EstimatorCommand = new RelayCommand(Estimator, CanAccessEstimator);
            SalesmanCommand = new RelayCommand(Salesman, CanAccessSalesman);
            StatusCommand = new RelayCommand(UpdateStatus, CanAccessStatus);
            UpdatesCommand = new RelayCommand<Quotation>(Updates, CanAccessUpdates);
            ExportCommand = new RelayCommand(Export, CanAccessExport);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);

            OnPipe();
        }

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [QuotationsStatus].[Inquiries(View)] " +
                    $"Where Dated Between @StartDate And @EndDate " +
                    $"Order By RegisterYear Desc, RegisterNumber Desc";
            Inquiries = new ObservableCollection<Inquiry>(connection.Query<Inquiry>(query, this));

            query = $"Select * From [QuotationsStatus].[Quotations(View)] " +
                    $"Where DateIssued Between @StartDate And @EndDate " +
                    $"Order By QuotationYear Desc, QuotationNumber Desc";
            Quotations = new ObservableCollection<Quotation>(connection.Query<Quotation>(query, this));
        }
        private void CreateInquiriesCollectionView()
        {
            InquiriesCollection = CollectionViewSource.GetDefaultView(Inquiries);
            InquiriesCollection.Filter = new Predicate<object>(InquiriesDataFilter);
            InquiriesCollection.SortDescriptions.Add(new SortDescription("RegisterYear", ListSortDirection.Descending));
            InquiriesCollection.SortDescriptions.Add(new SortDescription("RegisterNumber", ListSortDirection.Descending));
            InquiriesCollection.CollectionChanged += InquiriesCollectionChanged;
        }
        private void CreateQuotationsCollectionView()
        {
            QuotationsCollection = CollectionViewSource.GetDefaultView(Quotations);
            QuotationsCollection.Filter = new Predicate<object>(QuotationsDataFilter);
            QuotationsCollection.SortDescriptions.Add(new SortDescription("QuotationYear", ListSortDirection.Descending));
            QuotationsCollection.SortDescriptions.Add(new SortDescription("QuotationNumber", ListSortDirection.Descending));
            QuotationsCollection.CollectionChanged += QuotationsCollectionChanged;
        }
        private void InquiriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateInquiriesIndicator();
        }
        private void UpdateInquiriesIndicator()
        {
            InquiriesIndicator = DataGridIndicator.Get(SelectedInquiryIndex, InquiriesCollection);
        }
        private void QuotationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateQuotationsIndicator();
        }
        private void UpdateQuotationsIndicator()
        {
            QuotationsIndicator = DataGridIndicator.Get(SelectedQuotationIndex, QuotationsCollection);
        }

        #region Filters
        private bool InquiriesDataFilter(object item)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            PropertyInfo valueProperty = item.GetType().GetProperty(columnName);
            
            if (valueProperty == null)
            {
                return true;
            }

            string value = $"{valueProperty.GetValue(item)}".ToUpper();
            string checkValue = "" + GetType().GetProperty(columnName).GetValue(this);

            if (!value.Contains(checkValue.ToUpper()))
            {
                result = false;
            }

            return result;
        }
        private bool QuotationsDataFilter(object item)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = "" + GetType().GetProperty(columnName).GetValue(this);

            if (string.IsNullOrWhiteSpace(checkValue))
            {
                result = true;
            }
            else if (!value.Equals(checkValue.ToUpper()))
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
            InquiriesCollection.Refresh();
            QuotationsCollection.Refresh();
        }

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string StatusGroup
        {
            get => _StatusGroup;
            set
            {
                if (SetValue(ref _StatusGroup, value))
                {
                    FilterProperty = nameof(StatusGroup);
                    QuotationsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EstimationID
        {
            get => _EstimationID;
            set
            {
                if (SetValue(ref _EstimationID, value))
                {
                    FilterProperty = nameof(EstimationID);
                    InquiriesCollection.Refresh();
                    QuotationsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string SalesmanID
        {
            get => _SalesmanID;
            set
            {
                if (SetValue(ref _SalesmanID, value))
                {
                    FilterProperty = nameof(SalesmanID);
                    InquiriesCollection.Refresh();
                    QuotationsCollection.Refresh();
                }
            }
        }

        #endregion

        public User UserData { get; }
        public DateTime? StartDate
        {
            get => _StartDate;
            set => SetValue(ref _StartDate, value);
        }
        public DateTime? EndDate
        {
            get => _EndDate;
            set => SetValue(ref _EndDate, value);
        }
        public ITable CurrentView
        {
            get => _CurrentView;
            set => SetValue(ref _CurrentView, value);
        }
        public string Status
        {
            get => _Status;
            set => SetValue(ref _Status, value);
        }
        public string StatusInfo
        {
            get => _StatusInfo;
            set => SetValue(ref _StatusInfo, value);
        }

        public string InquiriesIndicator
        {
            get => _InquiriesIndicator;
            set => SetValue(ref _InquiriesIndicator, value);
        }
        public int SelectedInquiryIndex
        {
            get => _SelectedInquiryIndex;
            set
            {
                if (SetValue(ref _SelectedInquiryIndex, value))
                {
                    UpdateInquiriesIndicator();
                }
            }
        }
        public Inquiry SelectedInquiry
        {
            get => _SelectedInquiry;
            set => SetValue(ref _SelectedInquiry, value);
        }
        public ObservableCollection<Inquiry> Inquiries
        {
            get => _Inquiries;
            set
            {
                if (SetValue(ref _Inquiries, value))
                {
                    CreateInquiriesCollectionView();
                }
            }
        }
        public ICollectionView InquiriesCollection
        {
            get => _InquiriesCollection;
            set
            {
                if (SetValue(ref _InquiriesCollection, value))
                {
                    UpdateInquiriesIndicator();
                }
            }
        }
        public ITable InquiriesView => _InquiriesView;

        public string QuotationsIndicator
        {
            get => _QuotationsIndicator;
            set => SetValue(ref _QuotationsIndicator, value);
        }
        public int SelectedQuotationIndex
        {
            get => _SelectedQuotationIndex;
            set
            {
                if (SetValue(ref _SelectedQuotationIndex, value))
                {
                    UpdateQuotationsIndicator();
                }
            }
        }
        public Quotation SelectedQuotation
        {
            get => _SelectedQuotation;
            set => SetValue(ref _SelectedQuotation, value);
        }
        public ObservableCollection<Quotation> Quotations
        {
            get => _Quotations;
            set
            {
                if (SetValue(ref _Quotations, value))
                {
                    CreateQuotationsCollectionView();
                }
            }
        }
        public ICollectionView QuotationsCollection
        {
            get => _QuotationsCollection;
            set
            {
                if (SetValue(ref _QuotationsCollection, value))
                {
                    UpdateQuotationsIndicator();
                }
            }
        }
        public ITable QuotationsView => _QuotationsView;

        public RelayCommand LoadCommand { get; }
        public RelayCommand OnPipeCommand { get; }
        public RelayCommand QuotationsCommand { get; }
        public RelayCommand WinCommand { get; }
        public RelayCommand OnGoingCommand { get; }
        public RelayCommand HoldCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand LostCommand { get; }
        public RelayCommand EstimatorCommand { get; }
        public RelayCommand SalesmanCommand { get; }
        public RelayCommand StatusCommand { get; }
        public RelayCommand<Quotation> UpdatesCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }


        private void Load()
        {
            GetData();
        }
        private bool CanAccessLoad()
        {
            if (StartDate == null)
                return false;

            if (EndDate == null)
                return false;

            return true;
        }

        private void OnPipe()
        {
            CurrentView = InquiriesView;
            Status = "On Pipe";
        }
        private bool CanAccessOnPipe()
        {
            return true;
        }

        private void AllQuotations()
        {
            DeleteFilter();
            CurrentView = QuotationsView;
            Status = "Quotations";
        }
        private bool CanAccessAllQuotations()
        {
            return true;
        }

        private void Win()
        {
            DeleteFilter();
            StatusGroup = "1";
            Status = "Win";
        }
        private bool CanAccessWin()
        {
            if (CurrentView != QuotationsView)
                return false;

            return true;
        }

        private void OnGoing()
        {
            DeleteFilter();
            StatusGroup = "5";
            Status = "On Going";
        }

        private bool CanAccessOnGoing()
        {
            if (CurrentView != QuotationsView)
                return false;

            return true;
        }

        private void Hold()
        {
            DeleteFilter();
            StatusGroup = "2";
            Status = "Hold";
        }
        private bool CanAccessHold()
        {
            if (CurrentView != QuotationsView)
                return false;

            return true;
        }

        private void Cancel()
        {
            DeleteFilter();
            StatusGroup = "3";
            Status = "Cancel";

        }
        private bool CanAccessCancel()
        {
            if (CurrentView != QuotationsView)
                return false;

            return true;
        }

        private void Lost()
        {
            DeleteFilter();
            StatusGroup = "4";
            Status = "Lost";

        }
        private bool CanAccessLost()
        {
            if (CurrentView != QuotationsView)
                return false;

            return true;
        }

        private void Estimator()
        {
            if (StatusInfo == null)
            {
                Navigation.OpenPopup(new EstimatorView(this));
            }
            else
            {
                SalesmanID = null;
                EstimationID = null;
                StatusInfo = null;
            }
        }
        private bool CanAccessEstimator()
        {
            return true;
        }

        private void Salesman()
        {
            if (StatusInfo == null)
            {
                Navigation.OpenPopup(new SalesmanView(this));
            }
            else
            {
                SalesmanID = null;
                EstimationID = null;
                StatusInfo = null;
            }
        }
        private bool CanAccessSalesman()
        {
            return true;
        }

        private void UpdateStatus()
        {
            if (CurrentView == QuotationsView)
            {
                Inquiry inquiry;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Select * From [Inquiry].[Inquiries(View)] Where InquiryID = {SelectedQuotation.InquiryID}";
                    inquiry = connection.QueryFirstOrDefault<Inquiry>(query);
                }

                Navigation.OpenPopup(new ProjectStatusView(inquiry, SelectedQuotation));
            }

            if (CurrentView == InquiriesView)
            {
                Navigation.OpenPopup(new ProjectStatusView(SelectedInquiry));
            }
        }
        private bool CanAccessStatus()
        {
            if (CurrentView == QuotationsView)
            {
                if (SelectedQuotation == null)
                    return false;
            }

            if (CurrentView == InquiriesView)
            {
                if (SelectedInquiry == null)
                    return false;
            }

            return true;
        }

        private void Updates(Quotation quotation)
        {
            Navigation.To(new CommunicationsView(quotation, QuotationsCollection), ViewData);
        }
        private bool CanAccessUpdates(Quotation quotation)
        {
            if (quotation == null)
                return false;

            if (UserData.ManageQuotationsUpdates)
                return true;

            if (UserData.EmployeeId == quotation.SalesmanID)
                return true;

            return false;
        }


        private class ExcelQuotation
        {
            public string Date { get; set; }
            public string Project { get; set; }
            public string Customer { get; set; }
            public string Quotation { get; set; }
            public string Estimation { get; set; }
            public string Salesman { get; set; }
            public string Type { get; set; }
            public double Total { get; set; }
            public string Status { get; set; }
            public string Comment { get; set; }
        }

        private void Export()
        {
            try
            {
                List<ExcelQuotation> list = new();
                foreach (Quotation quotation in QuotationsCollection.Cast<Quotation>())
                {
                    ExcelQuotation excelQuotation = new()
                    {
                        Date = quotation.DateIssued.ToString("dd-MM-yyyy"),
                        Project = quotation.Project,
                        Customer = quotation.Customer,
                        Quotation = quotation.QuotationCode,
                        Estimation = quotation.EstimationName,
                        Salesman = quotation.Salesman,
                        Type = quotation.ProjectStatus,
                        Total = quotation.QuotationEstimatedPrice,
                        Status = quotation.CurrentStatus,
                        Comment =  quotation.Note,
                    };

                    list.Add(excelQuotation);
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
                    table.Columns["Date"].SetOrdinal(0);
                    table.Columns["Project"].SetOrdinal(1);
                    table.Columns["Customer"].SetOrdinal(2);
                    table.Columns["Quotation"].SetOrdinal(3);
                    table.Columns["Estimation"].SetOrdinal(4);
                    table.Columns["Salesman"].SetOrdinal(5);
                    table.Columns["Type"].SetOrdinal(6);
                    table.Columns["Total"].SetOrdinal(7);
                    table.Columns["Status"].SetOrdinal(8);
                    table.Columns["Comment"].SetOrdinal(9);

                    string worksheetName = $"Quotations Report";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    _ = workSheet.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Cell(1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Cell(1, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

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

                    workSheet.Cell(1, 2).Value = "Customer Name";
                    workSheet.Cell(1, 3).Value = "Project Name";
                    workSheet.Cell(1, 4).Value = "Quotation Code";

                    workSheet.Row(1).InsertRowsAbove(3);

                    workSheet.Range(workSheet.Cell(2, 1), workSheet.Cell(2, 10)).Merge();
                    workSheet.Cell(2, 1).Value = "Quotations Report";
                    workSheet.Cell(2, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    workSheet.Cell(2, 1).Style.Font.FontSize = 24;
                    workSheet.Cell(2, 1).Style.Font.SetBold();

                    fileName = $"{worksheetName} {DateTime.Now:dd-MM-yyyy} .xlsx";
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
            if (CurrentView != QuotationsView)
                return false;

            if (QuotationsCollection.Cast<Quotation>().Count() == 0)
                return false;

            return true;
        }

    }
}