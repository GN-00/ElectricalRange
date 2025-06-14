using Dapper;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Services;
using ProjectsNow.Views.InquiriesViews;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectsNow.Views.QuotationsViews
{
    public class QuotationsViewModel : ViewModelBase
    {
        private Statuses _Status = Statuses.Running;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Quotation _SelectedItem;
        private ObservableCollection<Quotation> _Items;

        private int? _SelectedYear;
        private ObservableCollection<int> _Years;

        private string _QuotationCode;
        private string _CustomerName;
        private string _ProjectName;
        private string _EstimationCode;
        private string _RegisterDateInfo;
        private string _DuoDateInfo;
        private string _Priority;
        private string _QuotationStatus;

        private ICollectionView _ItemsView;
        public QuotationsViewModel(IView view)
        {
            ViewData = view;
            AccessKeys.Add("InquiryId");
            AccessKeys.Add("QuotationId");
            UserData = Navigation.UserData;

            GetData();

            NewProjectsCommand = new RelayCommand(NewProjects, CanAccessNewProjects);
            InfoCommand = new RelayCommand<Quotation>(Info, CanAccessInfo);
            TermsCommand = new RelayCommand<Quotation>(Terms, CanAccessTerms);
            PanelsCommand = new RelayCommand<Quotation>(Panels, CanAccessPanels);
            RevisionsCommand = new RelayCommand<Quotation>(Revisions, CanAccessRevisions);
            ItemsCommand = new RelayCommand<Quotation>(GetItems, CanAccessItems);
            CostSheetCommand = new RelayCommand<Quotation>(CostSheet, CanAccessCostSheet);
            DiscountCommand = new RelayCommand<Quotation>(Discount, CanAccessDiscount);
            PricesCommand = new RelayCommand<Quotation>(Prices, CanAccessPrices);
            PriceNoteCommand = new RelayCommand<Quotation>(PriceNote, CanAccessPriceNote);
            PrintCommand = new RelayCommand<Quotation>(Print, CanAccessPrint);
            OptionsCommand = new RelayCommand<Quotation>(Options, CanAccessOptions);
            SubmitCommand = new RelayCommand<Quotation>(Submit, CanAccessSubmit);
            StartCommand = new RelayCommand<Quotation>(Start, CanAccessStart);
            HoldCommand = new RelayCommand<Quotation>(Hold, CanAccessHold);
            CancelCommand = new RelayCommand<Quotation>(Cancel, CanAccessCancel);
            ReviseCommand = new RelayCommand<Quotation>(Revise, CanAccessRevise);
            ResetCommand = new RelayCommand<Quotation>(Reset, CanAccessReset);
            AllCommand = new RelayCommand(All, CanAccessAll);
            RunningCommand = new RelayCommand(Running, CanAccessRunning);
            SubmittedCommand = new RelayCommand(Submitted, CanAccessSubmitted);
            HoldedCommand = new RelayCommand(Holded, CanAccessHolded);
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
                    GetData();
                }
            }
        }
        public string StatusInfo => $"{Status} Quotations";
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
                else if (Status == Statuses.Submitted)
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
        public Quotation SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Quotation> Items
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

                    DeleteFilter();
                    GetData();
                    OnPropertyChanged(nameof(YearInfo));
                }
            }
        }
        public ObservableCollection<int> Years
        {
            get => _Years;
            private set => SetValue(ref _Years, value);
        }
        public ICollectionView ItemsView
        {
            get => _ItemsView;
            set => SetValue(ref _ItemsView, value);
        }

        public RelayCommand NewProjectsCommand { get; }
        public RelayCommand<Quotation> InfoCommand { get; }
        public RelayCommand<Quotation> TermsCommand { get; }
        public RelayCommand<Quotation> PanelsCommand { get; }
        public RelayCommand<Quotation> RevisionsCommand { get; }
        public RelayCommand<Quotation> ItemsCommand { get; }
        public RelayCommand<Quotation> CostSheetCommand { get; }
        public RelayCommand<Quotation> DiscountCommand { get; }
        public RelayCommand<Quotation> PricesCommand { get; }
        public RelayCommand<Quotation> PriceNoteCommand { get; }
        public RelayCommand<Quotation> PrintCommand { get; }
        public RelayCommand<Quotation> OptionsCommand { get; }
        public RelayCommand<Quotation> SubmitCommand { get; }
        public RelayCommand<Quotation> StartCommand { get; }
        public RelayCommand<Quotation> HoldCommand { get; }
        public RelayCommand<Quotation> CancelCommand { get; }
        public RelayCommand<Quotation> ReviseCommand { get; }
        public RelayCommand<Quotation> ResetCommand { get; }
        public RelayCommand AllCommand { get; }
        public RelayCommand RunningCommand { get; }
        public RelayCommand SubmittedCommand { get; }
        public RelayCommand HoldedCommand { get; }
        public RelayCommand CanceledCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string QuotationCode
        {
            get => _QuotationCode;
            set
            {
                if (SetValue(ref _QuotationCode, value))
                {
                    FilterProperty = nameof(QuotationCode);
                    ItemsView.Refresh();
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
                    ItemsView.Refresh();
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
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EstimationCode
        {
            get => _EstimationCode;
            set
            {
                if (SetValue(ref _EstimationCode, value))
                {
                    FilterProperty = nameof(EstimationCode);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string RegisterDateInfo
        {
            get => _RegisterDateInfo;
            set
            {
                if (SetValue(ref _RegisterDateInfo, value))
                {
                    FilterProperty = nameof(RegisterDateInfo);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string DuoDateInfo
        {
            get => _DuoDateInfo;
            set
            {
                if (SetValue(ref _DuoDateInfo, value))
                {
                    FilterProperty = nameof(DuoDateInfo);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Priority
        {
            get => _Priority;
            set
            {
                if (SetValue(ref _Priority, value))
                {
                    FilterProperty = nameof(Priority);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string QuotationStatus
        {
            get => _QuotationStatus;
            set
            {
                if (SetValue(ref _QuotationStatus, value))
                {
                    FilterProperty = nameof(QuotationStatus);
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
            using SqlConnection connection = new(Database.ConnectionString);
            if (UserData.AccessAllQuotations)
            {
                Items = QuotationController.GetQuotations(connection, SelectedYear ?? DateTime.Now.Year, Status);
            }
            else
            {
                Items = QuotationController.UserQuotations(connection, UserData.EmployeeId, SelectedYear ?? DateTime.Now.Year, Status);
            }

            if (Items.Count != 0)
            {
                SelectedItem = Items[0];
            }

            if (Years == null)
            {
                string query = $"Select QuotationYear From [Quotation].[Quotations(View)] " +
                               $"Where QuotationStatus != 'Revision' ";

                if (Status != Statuses.All)
                {
                    query += $"And QuotationStatus = '{Status}' ";
                }

                if (!UserData.AccessAllQuotations)
                {
                    query += $"And EstimationID = {UserData.EmployeeId} ";
                }

                query += $"Group By QuotationYear ";

                Years = new ObservableCollection<int>(connection.Query<int>(query));
            }
        }

        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.Filter = new Predicate<object>(DataFilter);
            ItemsView.SortDescriptions.Add(new SortDescription("QuotationYear", ListSortDirection.Descending));
            ItemsView.SortDescriptions.Add(new SortDescription("QuotationNumber", ListSortDirection.Descending));
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

        private void NewProjects()
        {
            Navigation.To(new QuoteView());
        }
        private bool CanAccessNewProjects()
        {
            if (!UserData.AccessQuote)
                return false;

            return true;
        }

        private void Info(Quotation quotation)
        {
            Inquiry inquiry;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Inquiry].[Inquiries(View)] Where InquiryID = {quotation.InquiryID}";
                inquiry = connection.QueryFirstOrDefault<Inquiry>(query);
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
        private bool CanAccessInfo(Quotation quotation)
        {
            return QuotationServices.CanAccessInfo(quotation);

        }

        private void Terms(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                QuotationServices.Terms(quotation, ViewData);
            }
        }
        private bool CanAccessTerms(Quotation quotation)
        {
            return QuotationServices.CanAccessTerms(quotation); 
        }

        private void Panels(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                QuotationServices.Panels(quotation, Items, ViewData);
            }
        }
        private bool CanAccessPanels(Quotation quotation)
        {
            return QuotationServices.CanAccessPanels(quotation);
        }

        private void Revisions(Quotation quotation)
        {
            QuotationServices.Revisions(quotation, ViewData);
        }
        private bool CanAccessRevisions(Quotation quotation)
        {
            return QuotationServices.CanAccessRevisions(quotation);
        }

        private void GetItems(Quotation quotation)
        {
            QuotationServices.GetItems(quotation, ViewData);
        }
        private bool CanAccessItems(Quotation quotation)
        {
            return QuotationServices.CanAccessGetItems(quotation);
        }

        private void CostSheet(Quotation quotation)
        {
            QuotationServices.CostSheet(quotation, ViewData);
        }
        private bool CanAccessCostSheet(Quotation quotation)
        {
            return QuotationServices.CanAccessCostSheet(quotation);
        }

        private void Discount(Quotation quotation)
        {
            Navigation.OpenPopup(new DiscountsView(quotation), PlacementMode.Center, true);
        }
        private bool CanAccessDiscount(Quotation quotation)
        {
            if (quotation == null)
                return false;

            if (quotation.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != quotation.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }
        private void Prices(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                QuotationServices.Prices(quotation);
            }
        }
        private bool CanAccessPrices(Quotation quotation)
        {
            return QuotationServices.CanAccessPrices(quotation);
        }

        private void PriceNote(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                QuotationServices.PriceNote(quotation);
            }
        }
        private bool CanAccessPriceNote(Quotation quotation)
        {
            return QuotationServices.CanAccessPriceNote(quotation);
        }

        private void Print(Quotation quotation)
        {
            QuotationServices.Print(quotation, ViewData);
        }
        private bool CanAccessPrint(Quotation quotation)
        {
            if (quotation == null)
                return false;

            return true;
        }

        private void Options(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                QuotationServices.Options(quotation, ViewData);
            }
        }
        private bool CanAccessOptions(Quotation quotation)
        {
            if (quotation == null)
                return false;

            return true;
        }

        private void Submit(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                QuotationServices.Submit(quotation, Items, Status);
            }
        }
        private bool CanAccessSubmit(Quotation quotation)
        {
            return QuotationServices.CanAccessSubmit(quotation);
        }

        private void Start(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                QuotationServices.Restart(quotation, Items, Status);
            }
        }
        private bool CanAccessStart(Quotation quotation)
        {
            if (quotation == null)
                return false;

            if (quotation.QuotationStatus == Statuses.Running.ToString())
                return false;

            if (quotation.QuotationStatus == Statuses.Submitted.ToString())
                return false;

            if (quotation.QuotationStatus == Statuses.Canceled.ToString())
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != quotation.EstimationID)
                return false;

            return true;
        }

        private void Hold(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                QuotationServices.Hold(quotation, Items, Status);
            }
        }
        private bool CanAccessHold(Quotation quotation)
        {
            if (quotation == null)
                return false;

            if (quotation.QuotationStatus == Statuses.Hold.ToString())
                return false;

            if (quotation.QuotationStatus == Statuses.Submitted.ToString())
                return false;

            if (quotation.QuotationStatus == Statuses.Canceled.ToString())
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != quotation.EstimationID)
                return false;

            return true;
        }

        private void Cancel(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                QuotationServices.Cancel(quotation, Items, Status);
            }
        }
        private bool CanAccessCancel(Quotation quotation)
        {
            if (quotation == null)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != quotation.EstimationID)
                return false;

            return true;
        }

        private void Revise(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                IsLoading = true;
                Events.ShowEvent.Do();

                QuotationServices.Revise(quotation);

                DeleteFilterCommand.Execute();
                Status = Statuses.Running;
                IsLoading = false;
            }
        }
        private bool CanAccessRevise(Quotation quotation)
        {
            if (quotation == null)
                return false;

            if (quotation.QuotationStatus != Statuses.Submitted.ToString())
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != quotation.EstimationID)
                return false;

            return true;
        }

        private void Reset(Quotation quotation)
        {
            if (UserData.Access(quotation))
            {
                IsLoading = true;
                //Events.ShowEvent.Do();

                QuotationServices.Reset(quotation);

                DeleteFilterCommand.Execute();
                Status = Statuses.Running;
                IsLoading = false;
            }
        }
        private bool CanAccessReset(Quotation quotation)
        {
            if (quotation == null)
                return false;

            if (quotation.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (quotation.QuotationRevise == 0)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != quotation.EstimationID)
                return false;

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

        private void Submitted()
        {
            Years = null;
            Status = Statuses.Submitted;
        }

        private bool CanAccessSubmitted()
        {
            return true;
        }

        private void Holded()
        {
            Years = null;
            Status = Statuses.Hold;
        }

        private bool CanAccessHolded()
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
    }
}