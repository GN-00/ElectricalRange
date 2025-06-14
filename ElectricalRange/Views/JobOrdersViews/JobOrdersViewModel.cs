using ClosedXML.Excel;

using Dapper;
using Dapper.Contrib.Extensions;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Services;
using ProjectsNow.Views.InquiriesViews;
using ProjectsNow.Windows.JobOrderWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectsNow.Views.JobOrdersViews
{
    public class JobOrdersViewModel : ViewModelBase
    {
        private Statuses _Status = Statuses.Running;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private JobOrder _SelectedItem;
        private ObservableCollection<JobOrder> _Items;

        private int? _SelectedYear;
        private ObservableCollection<int> _Years;

        private string _Code;
        private string _QuotationCode;
        private string _CustomerName;
        private string _ProjectName;
        private string _EstimationName;
        private string _SalesmanName;

        private ICollectionView _ItemsCollection;
        public JobOrdersViewModel(IView view)
        {
            ViewData = view;
            AccessKeys.Add("InquiryId");
            AccessKeys.Add("QuotationId");
            AccessKeys.Add("JobOrderId");
            UserData = Navigation.UserData;

            GetYearsData(Status);

            InfoCommand = new RelayCommand<JobOrder>(Info, CanAccessInfo);
            AcknowledgmentCommand = new RelayCommand<JobOrder>(Acknowledgment, CanAccessAcknowledgment);
            PanelsCommand = new RelayCommand<JobOrder>(Panels, CanAccessPanels);
            PurchaseOrderCommand = new RelayCommand<JobOrder>(PurchaseOrder, CanAccessPurchaseOrder);
            ItemsStatusCommand = new RelayCommand<JobOrder>(ItemsStatus, CanAccessItemsStatus);
            PurchaseCommand = new RelayCommand<JobOrder>(Purchase, CanAccessPurchase);
            ApprovalCommand = new RelayCommand<JobOrder>(Approval, CanAccessApproval);
            ProductionCommand = new RelayCommand<JobOrder>(Production, CanAccessProduction);
            InspectionCommand = new RelayCommand<JobOrder>(Inspection, CanAccessInspection);
            ClosingCommand = new RelayCommand<JobOrder>(Closing, CanAccessClosing);
            WarrantiesCommand = new RelayCommand<JobOrder>(Warranties, CanAccessWarranties);
            QualityCommand = new RelayCommand<JobOrder>(Quality, CanAccessQuality);
            ExportCommand = new RelayCommand(Export, CanAccessExport);
            DeliveryCommand = new RelayCommand<JobOrder>(Delivery, CanAccessDelivery);
            CancelCommand = new RelayCommand<JobOrder>(Cancel, CanAccessCancel);

            NewOrdersCommand = new RelayCommand(NewOrders, CanAccessNewOrders);

            AllCommand = new RelayCommand(All, CanAccessAll);
            RunningCommand = new RelayCommand(Running, CanAccessRunning);
            ClosedCommand = new RelayCommand(Closed, CanAccessClosed);
            CanceledCommand = new RelayCommand(Canceled, CanAccessCanceled);

            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public Statuses Status
        {
            get => _Status;
            set
            {
                if (SetValue(ref _Status, value))
                {
                    OnPropertyChanged(nameof(StatusInfo));
                    OnPropertyChanged(nameof(StatusColor));
                    OnPropertyChanged(nameof(YearInfo));
                    GetYearsData(Status);
                }
            }
        }
        public string StatusInfo => $"{Status} Job Orders";
        public string YearInfo => $"{SelectedYear ?? DateTime.Now.Year}";
        public SolidColorBrush StatusColor
        {
            get
            {
                if (Status == Statuses.Running)
                    return Brushes.Green;
                else if (Status == Statuses.Hold)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFC800"));
                else if (Status == Statuses.Canceled)
                    return Brushes.Red;
                else if (Status == Statuses.Closed)
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#FF376CDC");
                else
                    return Brushes.Black;
            }
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
        public JobOrder SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<JobOrder> Items
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
        public int? SelectedYear
        {
            get => _SelectedYear;
            set
            {
                if (SetValue(ref _SelectedYear, value))
                {
                    if (SelectedYear == null)
                        return;

                    Navigation.OpenLoading(Visibility.Visible, "Loading...");
                    Events.ShowEvent.Do();

                    DeleteFilter();
                    GetData(SelectedYear.Value, Status);
                    OnPropertyChanged(nameof(YearInfo));

                    Navigation.CloseLoading();
                }
            }
        }
        public ObservableCollection<int> Years
        {
            get => _Years;
            private set => SetValue(ref _Years, value);
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand NewOrdersCommand { get; }
        public RelayCommand<JobOrder> InfoCommand { get; }
        public RelayCommand<JobOrder> AcknowledgmentCommand { get; }
        public RelayCommand<JobOrder> PanelsCommand { get; }
        public RelayCommand<JobOrder> PurchaseOrderCommand { get; }
        public RelayCommand<JobOrder> ItemsStatusCommand { get; }
        public RelayCommand<JobOrder> PurchaseCommand { get; }
        public RelayCommand<JobOrder> PostingItemsCommand { get; }
        public RelayCommand<JobOrder> ApprovalCommand { get; }
        public RelayCommand<JobOrder> ProductionCommand { get; }
        public RelayCommand<JobOrder> InspectionCommand { get; }
        public RelayCommand<JobOrder> ClosingCommand { get; }
        public RelayCommand<JobOrder> WarrantiesCommand { get; }
        public RelayCommand<JobOrder> QualityCommand { get; }
        public RelayCommand<JobOrder> DeliveryCommand { get; }
        public RelayCommand<JobOrder> CancelCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand AllCommand { get; }
        public RelayCommand RunningCommand { get; }
        public RelayCommand ClosedCommand { get; }
        public RelayCommand CanceledCommand { get; }
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

        private void GetYearsData(Statuses statuses)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (statuses == Statuses.All)
                    Years = JobOrderController.JobOrdersYears(connection);

                else if (statuses == Statuses.Running)
                    Years = JobOrderController.GetRunningJobOrdersYears(connection);

                else if (statuses == Statuses.Closed)
                    Years = JobOrderController.GetClosedJobOrdersYears(connection);

                else if (statuses == Statuses.Canceled)
                    Years = JobOrderController.GetCanceledJobOrdersYears(connection);
            }

            if (Years.Count != 0)
                SelectedYear = Years[0];
            else
                SelectedYear = DateTime.Now.Year;
        }

        private void GetData(int year, Statuses statuses)
        {
            using SqlConnection connection = new(Database.ConnectionString);
            if (statuses == Statuses.All)
                Items = JobOrderController.JobOrders(connection, year);

            else if (statuses == Statuses.Running)
                Items = JobOrderController.GetRunningJobOrders(connection, year);

            else if (statuses == Statuses.Closed)
                Items = JobOrderController.GetClosedJobOrders(connection, year);

            else if (statuses == Statuses.Canceled)
                Items = JobOrderController.GetCanceledJobOrders(connection, year);
        }

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("CodeYear", ListSortDirection.Descending));
            ItemsCollection.SortDescriptions.Add(new SortDescription("CodeNumber", ListSortDirection.Descending));
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

        private void NewOrders()
        {
            Navigation.To(new NewJobOrdersView());
        }
        private bool CanAccessNewOrders()
        {
            if (!UserData.AccessNewJobOrder)
                return false;

            return true;
        }

        private void Info(JobOrder jobOrder)
        {
            string query;
            Inquiry inquiry;
            Quotation quotation;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Inquiry].[Inquiries(View)] Where InquiryID = {jobOrder.InquiryID}";
                inquiry = connection.QueryFirstOrDefault<Inquiry>(query);

                query = $"Select * From [Quotation].[Quotations(View)] Where QuotationID = {jobOrder.QuotationID}";
                quotation = connection.QueryFirstOrDefault<Quotation>(query);
            }

            if (UserData.Access(inquiry))
            {
                if (UserData.Access(quotation))
                {
                    Navigation.To(new InquiryView(inquiry, quotation), ViewData);
                }
                else
                {
                    UserData.Exist(inquiry);
                }
            }
        }
        private bool CanAccessInfo(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            return true;
        }

        private void Acknowledgment(JobOrder jobOrder)
        {
            Navigation.OpenPopup(new SelectAcknowledgmentView(jobOrder, ViewData), PlacementMode.MousePoint, false);
        }
        private bool CanAccessAcknowledgment(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            return true;
        }

        private void Panels(JobOrder jobOrder)
        {
            if (UserData.Access(jobOrder))
            {
                Navigation.To(new PanelsView(jobOrder), ViewData);
            }
        }
        private bool CanAccessPanels(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            return true;
        }

        private void PurchaseOrder(JobOrder jobOrder)
        {
            if (UserData.Access(jobOrder))
            {
                Navigation.To(new PurchaseOrderView(jobOrder), ViewData);
            }
        }
        private bool CanAccessPurchaseOrder(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            return true;
        }

        private void ItemsStatus(JobOrder jobOrder)
        {
            ItemsStatusWindow itemsStatusWindow = new()
            {
                JobOrderData = jobOrder,
            };
            _ = itemsStatusWindow.ShowDialog();
        }
        private bool CanAccessItemsStatus(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

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
            if (UserData.Access(jobOrder))
            {
                Navigation.To(new StoreViews.InvoicesView(jobOrder), ViewData);
            }
        }
        private bool CanAccessPostingItems(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            return true;
        }

        private void Warranties(JobOrder jobOrder)
        {
            if (UserData.Access(jobOrder))
            {
                Navigation.To(new WarrantiesView(jobOrder), ViewData);
            }
        }
        private bool CanAccessWarranties(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            return true;
        }


        private void Quality(JobOrder jobOrder)
        {
            if (UserData.Access(jobOrder))
            {
                Navigation.To(new QualityView(jobOrder), ViewData);
            }
        }
        private bool CanAccessQuality(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            if (!UserData.AccessQuality)
                return false;

            return true;
        }

        private class ExcelOrder
        {
            public string Code { get; set; }
            public string QuotationCode { get; set; }
            public string Customer { get; set; }
            public string Project { get; set; }
            public string Estimation { get; set; }
        }
        private void Export()
        {
            try
            {
                List<ExcelOrder> list = new();
                foreach (JobOrder order in ItemsCollection.Cast<JobOrder>())
                {
                    ExcelOrder excelOrder = new()
                    {
                        Code = order.Code,
                        QuotationCode = order.QuotationCode,
                        Customer = order.CustomerName,
                        Project = order.ProjectName,
                        Estimation = order.EstimationName,
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
                    table.Columns["Code"].SetOrdinal(0);
                    table.Columns["QuotationCode"].SetOrdinal(1);
                    table.Columns["Customer"].SetOrdinal(2);
                    table.Columns["Project"].SetOrdinal(3);
                    table.Columns["Estimation"].SetOrdinal(4);

                    string worksheetName = $"Job Orders";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();

                    workSheet.Cell(1, 2).Value = "Quotation Code";

                    workSheet.Cell(1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    workSheet.Cell(1, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    workSheet.Cell(1, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


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
                    _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
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

        private void All()
        {
            Years = null;
            Status = Statuses.All;
        }

        private bool CanAccessAll()
        {
            return true;
        }

        private void Running()
        {
            Years = null;
            Status = Statuses.Running;
        }

        private bool CanAccessRunning()
        {
            return true;
        }

        private void Closed()
        {
            Years = null;
            Status = Statuses.Closed;
        }

        private bool CanAccessClosed()
        {
            return true;
        }

        private void Canceled()
        {
            Years = null;
            Status = Statuses.Canceled;
        }

        private bool CanAccessCanceled()
        {
            return true;
        }

        private void Delivery(JobOrder jobOrder)
        {
            Navigation.To(new DeliveriesView(jobOrder), ViewData);
        }
        private bool CanAccessDelivery(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            return true;
        }

        private void Cancel(JobOrder jobOrder)
        {
            JobOrder record;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [JobOrder].[JobOrdersInformation] " +
                               $"Where JobOrderId = {jobOrder.ID} " +
                               $"Order By CodeYear DESC, CodeNumber DESC";
                record = connection.QueryFirstOrDefault<JobOrder>(query);
            }

            if (record.Invoices != 0 || record.Deliveries != 0)
            {
                string message = $"Can't Cancel this order!" +
                                 $"\nDeliveries: {record.Deliveries}." +
                                 $"\nInvoices: {record.Invoices}.";
                MessageView.Show("Cancelation", message, MessageViewButton.OK, MessageViewImage.Information);
            }
            else
            {
                string message = $"Are you sure you want to cancel:" +
                                 $"\nJob Order: {record.Code}.";
                MessageBoxResult result = MessageView.Show("Cancelation", message, MessageViewButton.YesNo, MessageViewImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    CancelJobOrder cancelJobOrder = new()
                    {
                        JobOrderId = record.ID,
                        Date = DateTime.Now,
                    };

                    Communication communication = new()
                    {
                        Date = DateTime.Now,
                        Description = $"{DateTime.Now:dd/MM/yyyy} Job Order Canceled.",
                        Status = "Cancel",
                        Type = "Phone",
                        InquiryID = jobOrder.InquiryID,
                        SalesmanID = UserData.EmployeeId,
                    };

                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        _ = connection.Insert(cancelJobOrder);
                        _ = connection.Insert(communication);
                    }

                    Items.Remove(jobOrder);
                }
            }
        }
        private bool CanAccessCancel(JobOrder jobOrder)
        {
            if (jobOrder == null)
                return false;

            if (!UserData.JobOrdersCanCancel)
                return false;

            if (jobOrder.Canceled)
                return false;

            if (Status == Statuses.Closed)
                return false;

            if (Status == Statuses.Canceled)
                return false;

            return true;
        }


        private void Approval(JobOrder order)
        {
            JobOrderServices.Approvals(order, ViewData);
        }
        private bool CanAccessApproval(JobOrder order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void Production(JobOrder order)
        {
            JobOrderServices.Production(order, ViewData);
        }
        private bool CanAccessProduction(JobOrder order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void Inspection(JobOrder order)
        {
            JobOrderServices.Inspection(order, ViewData);
        }
        private bool CanAccessInspection(JobOrder order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void Closing(JobOrder order)
        {
            JobOrderServices.Closing(order, ViewData);
        }
        private bool CanAccessClosing(JobOrder order)
        {
            if (order == null)
                return false;

            return true;
        }
    }
}