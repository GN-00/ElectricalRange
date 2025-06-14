
using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows.Data;

namespace ProjectsNow.Views.QuotationsViews
{
    public class QuoteViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Inquiry _SelectedItem;
        private ObservableCollection<Inquiry> _Items;

        private string _RegisterCode;
        private string _CustomerName;
        private string _ProjectName;
        private string _EstimationName;
        private string _RegisterDateInfo;
        private string _DuoDateInfo;
        private string _Priority;

        private ICollectionView _ItemsCollection;

        public QuoteViewModel()
        {
            UserData = Navigation.UserData;

            GetData();

            QuoteCommand = new RelayCommand<Inquiry>(Quote, CanAccessQuote);
            QuotationsCommand = new RelayCommand(Quotations, CanAccessQuotations);
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
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand<Inquiry> QuoteCommand { get; }
        public RelayCommand QuotationsCommand { get; }
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
        public string RegisterDateInfo
        {
            get => _RegisterDateInfo;
            set
            {
                if (SetValue(ref _RegisterDateInfo, value))
                {
                    FilterProperty = nameof(RegisterDateInfo);
                    ItemsCollection.Refresh();
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
                    ItemsCollection.Refresh();
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
            query = $"Select * From [Quotation].[NewProjects] " +
                    $"Where EstimationID = {UserData.EmployeeId}" +
                    $"Order By RegisterYear Desc, RegisterNumber Desc";

            Items = new ObservableCollection<Inquiry>(connection.Query<Inquiry>(query));
        }

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("RegisterYear", ListSortDirection.Descending));
            ItemsCollection.SortDescriptions.Add(new SortDescription("RegisterNumber", ListSortDirection.Descending));
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

        private void Quote(Inquiry inquiry)
        {
            if (UserData.Access(inquiry))
            {
                Quotation quotationData = new(inquiry);
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    quotationData.QuotationStatus = Statuses.Running.ToString();
                    quotationData.QuotationNumber = QuotationController.NewQuotationNumber(connection, DateTime.Now.Year);
                    quotationData.QuotationYear = DateTime.Now.Year;
                    quotationData.QuotationMonth = DateTime.Now.Month;
                    quotationData.QuotationCode =
                        $"ER-{quotationData.QuotationNumber:000}/{UserData.Code}/{quotationData.QuotationMonth:00}/{quotationData.QuotationYear}/R00";
                    quotationData.QuotationReviseDate = DateTime.Now;

                    _ = connection.Insert(quotationData);
                    TermController.DefaultTerms(connection, quotationData.QuotationID);
                }

                _ = MessageView.Show($"Quotation",
                                     $"{quotationData.QuotationCode} has been added to your quotations!",
                                     MessageViewButton.OK,
                                     MessageViewImage.Information);

                Items.Remove(inquiry);
            }
        }
        private bool CanAccessQuote(Inquiry inquiry)
        {
            if (inquiry == null)
                return false;

            return true;
        }

        private void Quotations()
        {
            Navigation.To(new QuotationsView());
        }
        private bool CanAccessQuotations()
        {
            if (!UserData.AccessQuotations)
                return false;

            return true;
        }
    }
}