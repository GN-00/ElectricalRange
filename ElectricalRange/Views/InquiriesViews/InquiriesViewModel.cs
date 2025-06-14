using Dapper;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.QuotationsStatusViews;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.InquiriesViews
{
    public class InquiriesViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedYear;
        private int _SelectedIndex;
        private Inquiry _SelectedItem;
        private ObservableCollection<Inquiry> _Items;
        private ObservableCollection<int> _Years;

        private string _RegisterCode;
        private string _CustomerName;
        private string _ProjectName;
        private string _EstimationCode;
        private string _SalesmanCode;
        private string _RegisterDateInfo;
        private string _DuoDateInfo;
        private string _Priority;
        private string _Status;

        private ICollectionView _ItemsView;

        public InquiriesViewModel(IView view)
        {
            ViewData = view;
            AccessKeys.Add("InquiryId");
            AccessKeys.Add("QuotationId");
            UserData = Navigation.UserData;
            _SelectedYear = DateTime.Today.Year;

            GetData();

            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Inquiry>(Edit, CanEdit);
            AssignCommand = new RelayCommand<Inquiry>(Assign, CanAccessAssign);
            PrintCommand = new RelayCommand<Inquiry>(Print, CanPrint);
            DeleteCommand = new RelayCommand<Inquiry>(Delete, CanDelete);
            UserInquiriesCommand = new RelayCommand(UserInquiries, CanAccessUserInquiries);
            CommunicationsCommand = new RelayCommand<Inquiry>(Communications, CanAccessCommunications);
            AllInquiriesCommand = new RelayCommand(AllInquiries);
            NewInquiriesCommand = new RelayCommand(NewInquiries);
            QuotationsCommand = new RelayCommand(Quotations);
            SubmittedCommand = new RelayCommand(Submitted);
            JobOrdersCommand = new RelayCommand(JobOrders);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
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
        public Inquiry SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Inquiry> Items
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
        public int SelectedYear
        {
            get => _SelectedYear;
            set
            {
                if (SetValue(ref _SelectedYear, value))
                {
                    DeleteFilter();
                    GetData(SelectedYear);
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

        public RelayCommand AddCommand { get; }
        public RelayCommand<Inquiry> EditCommand { get; }
        public RelayCommand<Inquiry> AssignCommand { get; }
        public RelayCommand<Inquiry> PrintCommand { get; }
        public RelayCommand<Inquiry> DeleteCommand { get; }
        public RelayCommand UserInquiriesCommand { get; }
        public RelayCommand<Inquiry> CommunicationsCommand { get; }
        public RelayCommand AllInquiriesCommand { get; }
        public RelayCommand NewInquiriesCommand { get; }
        public RelayCommand QuotationsCommand { get; }
        public RelayCommand SubmittedCommand { get; }
        public RelayCommand JobOrdersCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string RegisterCode
        {
            get => _RegisterCode;
            set
            {
                if (SetValue(ref _RegisterCode, value))
                {
                    FilterProperty = nameof(RegisterCode);
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
        public string SalesmanCode
        {
            get => _SalesmanCode;
            set
            {
                if (SetValue(ref _SalesmanCode, value))
                {
                    FilterProperty = nameof(SalesmanCode);
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
        public string Status
        {
            get => _Status;
            set
            {
                if (SetValue(ref _Status, value))
                {
                    OnPropertyChanged(nameof(StatusInfo));
                    FilterProperty = nameof(Status);
                    ItemsView.Refresh();
                }
            }
        }

        public string StatusInfo => $"{(Status ?? "All")} Inquiries {SelectedYear}";

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

        private void GetData(int? year = null)
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Inquiry].[Inquiries(View)] " +
                    $"Where RegisterYear = {year ?? DateTime.Today.Year} " +
                    $"Order By RegisterYear Desc, RegisterNumber Desc";

            Items = new ObservableCollection<Inquiry>(connection.Query<Inquiry>(query));

            if (Years == null)
            {
                query = $"Select * From [Inquiry].[InquiriesYears] ";
                Years = new ObservableCollection<int>(connection.Query<int>(query));
            }
        }
        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.Filter = new Predicate<object>(DataFilter);
            ItemsView.SortDescriptions.Add(new SortDescription("RegisterYear", ListSortDirection.Descending));
            ItemsView.SortDescriptions.Add(new SortDescription("RegisterNumber", ListSortDirection.Descending));
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
            Navigation.To(new InquiryView(new Inquiry(), Items), ViewData);
        }
        private bool CanAdd()
        {
            if (!UserData.ModifyInquiries)
                return false;

            return true;
        }

        private void Edit(Inquiry inquiry)
        {
            if (UserData.Access(inquiry))
            {
                if (inquiry.QuotationID != null)
                {
                    string query;
                    Quotation quotation;
                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        query = $"Select * From [Quotation].[Quotations(View)] " +
                                $"Where QuotationID = {inquiry.QuotationID} ";

                        quotation = connection.QueryFirstOrDefault<Quotation>(query);
                    }

                    if (UserData.Access(quotation))
                    {
                        Navigation.To(new InquiryView(inquiry, quotation), ViewData);
                    }
                }
                else
                {
                    Navigation.To(new InquiryView(inquiry, Items), ViewData);
                }
            }
        }
        private bool CanEdit(Inquiry item)
        {
            if (item == null)
                return false;

            if (item.Status == "Order")
                return false;

            if (item.Status == "Submitted")
                return false;

            return true;
        }

        private void Assign(Inquiry inquiry)
        {
            if (UserData.Access(inquiry))
            {
                if (inquiry.QuotationID != null)
                {
                    string query;
                    Quotation quotation;
                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        query = $"Select * From [Quotation].[Quotations(View)] " +
                                $"Where QuotationID = {inquiry.QuotationID} ";

                        quotation = connection.QueryFirstOrDefault<Quotation>(query);
                    }

                    if (UserData.Access(quotation))
                    {
                        Navigation.OpenPopup(new AssignView(inquiry), PlacementMode.Center, true);
                    }
                }
                else
                {
                    Navigation.OpenPopup(new AssignView(inquiry), PlacementMode.Center, true);
                }
            }
        }
        private bool CanAccessAssign(Inquiry item)
        {
            if (item == null)
                return false;

            if (item.Status == "Order")
                return false;

            if (item.Status == "Submitted")
                return false;

            if (!UserData.ModifyInquiries)
                return false;

            return true;
        }

        private void Print(Inquiry inquiry)
        {
            InquiryInfo info;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Inquiry].[Inquiries(Information)] " +
                               $"Where InquiryID = {inquiry.InquiryID}";
                info = connection.QueryFirstOrDefault<InquiryInfo>(query);
            }

            if (info != null)
            {
                Printing.InquiryPages.InquiryForm form = new(info);
                Printing.Print.PrintPreview(form, $"Inquiry{inquiry.RegisterCode}", ViewData);
            }
        }
        private bool CanPrint(Inquiry item)
        {
            if (item == null)
                return false;

            return true;
        }

        private void Delete(Inquiry inquiry)
        {
            if (UserData.Access(inquiry))
            {
                MessageBoxResult result = MessageView.Show($"Deleting",
                                                           $"Do you want to Delete Inquiy: \n{inquiry.RegisterCode}?",
                                                           MessageViewButton.YesNo,
                                                           MessageViewImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using SqlConnection connection = new(Database.ConnectionString);
                    _ = connection.Execute($"Delete From [Inquiry].[Inquiries] Where InquiryID = {inquiry.InquiryID}");
                    _ = connection.Execute($"Delete From [Inquiry].[ProjectsContacts] Where InquiryID = {inquiry.InquiryID}");
                    _ = Items.Remove(inquiry);
                }
            }
        }
        private bool CanDelete(Inquiry item)
        {
            if (item == null)
                return false;

            if (item.Status == "Order")
                return false;

            if (item.Status == "Submitted")
                return false;

            if (item.Status == "Quoting")
                return false;

            if (!UserData.ModifyInquiries)
                return false;

            return true;
        }

        private void AllInquiries()
        {
            Status = null;
        }
        private void NewInquiries()
        {
            Status = "New";
        }
        private void Quotations()
        {
            Status = "Quoting";
        }
        private void Submitted()
        {
            Status = "Submitted";
        }
        private void JobOrders()
        {
            Status = "Order";
        }

        private void UserInquiries()
        {
            if (SalesmanCode != null)
            {
                SalesmanCode = null;
            }
            else
            {
                SalesmanCode = UserData.Code;
            }
        }

        private bool CanAccessUserInquiries()
        {
            return UserData.IsSalesman;
        }

        private void Communications(Inquiry inquiry)
        {
            Navigation.To(new CommunicationsView(inquiry.Id), ViewData);
        }

        private bool CanAccessCommunications(Inquiry inquiry)
        {
            if (inquiry == null)
                return false;

            if (UserData.ManageQuotationsUpdates)
                return true;

            if (UserData.EmployeeId == inquiry.SalesmanID)
                return true;

            return false;
        }
    }
}