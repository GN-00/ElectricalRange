using Dapper.Contrib.Extensions;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectsNow.Views.JobOrdersViews
{
    public class NewJobOrdersViewModel : ViewModelBase
    {
        private string _Status = "Quotations";
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
        private string _SalesmanCode;
        private string _QuotationEstimatedPrice;

        private ICollectionView _ItemsView;

        public NewJobOrdersViewModel(IView view)
        {
            ViewData = view;
            AccessKeys.Add("JobOrderId");
            UserData = Navigation.UserData;

            GetYearsData();

            JobOrdersCommand = new RelayCommand(JobOrders, CanAccessJobOrders);
            StartCommand = new RelayCommand<Quotation>(Start, CanAccessStart);
            QuotationsCommand = new RelayCommand(Quotations, CanAccessQuotations);
            OrdersCommand = new RelayCommand(Order, CanAccessOrder);

            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public string Status
        {
            get => _Status;
            set
            {
                if (SetValue(ref _Status, value))
                {
                    OnPropertyChanged(nameof(StatusStart));
                    OnPropertyChanged(nameof(StatusColor));
                    OnPropertyChanged(nameof(YearInfo));
                    GetYearsData();
                }
            }
        }
        public string StatusStart => $"{Status}";
        public string YearInfo => $"{SelectedYear ?? DateTime.Now.Year}";
        public SolidColorBrush StatusColor
        {
            get
            {
                if (Status == "Quotations")
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#FF376CDC"); 
                else if (Status == "Orders")
                    return Brushes.Black;
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
                    GetData(SelectedYear.Value);
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

        public RelayCommand JobOrdersCommand { get; }
        public RelayCommand<Quotation> StartCommand { get; }
        public RelayCommand QuotationsCommand { get; }
        public RelayCommand OrdersCommand { get; }
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
        public string QuotationEstimatedPrice
        {
            get => _QuotationEstimatedPrice;
            set
            {
                if (SetValue(ref _QuotationEstimatedPrice, value))
                {
                    FilterProperty = nameof(QuotationEstimatedPrice);
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

            if (ItemsView != null)
                ItemsView.Refresh();
        }

        #endregion

        private void GetYearsData()
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (Status == "Quotations")
                    Years = JobOrderController.QuotationsWaitingPOYears(connection);
                else
                    Years = JobOrderController.GetJobOrdersQuotationsYears(connection);
            }

            if (Years.Count != 0)
            {
                SelectedYear = Years[0];
            }
        }
        private void GetData(int year)
        {
            using SqlConnection connection = new(Database.ConnectionString);
            if (Status == "Quotations")
                Items = JobOrderController.QuotationsWaitPO(connection, year);
            else
                Items = JobOrderController.GetJobOrdersQuotations(connection, year);
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

        private void JobOrders()
        {
            Navigation.To(new JobOrdersView());
        }
        private bool CanAccessJobOrders()
        {
            if (!UserData.AccessJobOrders)
                return false;

            return true;
        }

        private void Start(Quotation quotation)
        {
            Navigation.OpenPopup(new NewPurchaseOrderView(quotation, Items), PlacementMode.Center, true);
        }
        private bool CanAccessStart(Quotation quotation)
        {
            if (quotation == null)
                return false;

            if (quotation.JobOrderId != null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void Quotations()
        {
            Navigation.OpenLoading(Visibility.Visible, "Loading....");

            Status = "Quotations";

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                Years = JobOrderController.QuotationsWaitingPOYears(connection);
            }

            Navigation.CloseLoading();
        }

        private bool CanAccessQuotations()
        {
            return true;
        }

        private void Order()
        {
            Navigation.OpenLoading(Visibility.Visible, "Loading....");

            Status = "Job Orders";

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                Years = JobOrderController.JobOrdersYears(connection);

                if (Years.Count != 0)
                {
                    SelectedYear = Years[0];
                }
            }

            Navigation.CloseLoading();
        }

        private bool CanAccessOrder()
        {
            return true;
        }
    }
}