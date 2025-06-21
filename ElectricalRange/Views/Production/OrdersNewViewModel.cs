using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Data.Users;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    public class OrdersNewViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Order _SelectedItem;
        private ObservableCollection<Order> _Items;

        private int? _SelectedYear;

        private string _Code;
        private string _QuotationCode;
        private string _CustomerName;
        private string _ProjectName;
        private string _EstimationName;
        private string _SalesmanName;

        private ICollectionView _ItemsCollection;
        public OrdersNewViewModel(IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;

            GetData();

            PanelsCommand = new RelayCommand<Order>(Panels, CanAccessPanels);

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
        public Order SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Order> Items
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

        public RelayCommand<Order> PanelsCommand { get; }
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

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            Items = new ObservableCollection<Order>(
                connection.Query<Order>(
                    $"Select * From [Production].[Orders(New)] Order By CodeYear Desc, CodeNumber Desc"));
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

        private void Panels(Order order)
        {
            Navigation.To(new NewOrderPanelsView(order), ViewData);
        }
        private bool CanAccessPanels(Order order)
        {
            if (order == null)
                return false;

            return true;
        }
    }
}