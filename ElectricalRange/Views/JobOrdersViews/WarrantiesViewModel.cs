using Dapper;

using Microsoft.Data.SqlClient;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;

namespace ProjectsNow.Views.JobOrdersViews
{
    public class WarrantiesViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Warranty _SelectedItem;
        private ObservableCollection<Warranty> _Items;

        private string _Code;
        private string _Panels;
        private string _DateInfo;
        private string _DurationInfo;
        private string _StartFrom;
        private string _IssuedBy;

        private ICollectionView _ItemsCollection;

        public WarrantiesViewModel(JobOrder order, IView view)
        {
            ViewData = view;
            OrderData = order;

            GetData(order);

            AddCommand = new RelayCommand(Add, CanAdd);
            InfoCommand = new RelayCommand<Warranty>(Info, CanAccessInfo);
            PrintCommand = new RelayCommand<Warranty>(Print, CanPrint);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData => Navigation.UserData;
        public JobOrder OrderData { get; }
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
        public Warranty SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Warranty> Items
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
        public RelayCommand AddCommand { get; }
        public RelayCommand<Warranty> InfoCommand { get; }
        public RelayCommand<Warranty> PrintCommand { get; }
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
        public string DurationInfo
        {
            get => _DurationInfo;
            set
            {
                if (SetValue(ref _DurationInfo, value))
                {
                    FilterProperty = nameof(DurationInfo);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string StartFrom
        {
            get => _StartFrom;
            set
            {
                if (SetValue(ref _StartFrom, value))
                {
                    FilterProperty = nameof(StartFrom);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string IssuedBy
        {
            get => _IssuedBy;
            set
            {
                if (SetValue(ref _IssuedBy, value))
                {
                    FilterProperty = nameof(IssuedBy);
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

        private void GetData(JobOrder order)
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [JobOrder].[Warranties(View)] " +
                    $"Where JobOrderId = {order.ID} " +
                    $"Order by Date Desc; ";
            Items = new ObservableCollection<Warranty>(connection.Query<Warranty>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
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


        private void Add()
        {
            Warranty warranty = new()
            {
                JobOrderId = OrderData.ID,
                JobOrderCode = OrderData.Code,
                Code = "-New Warranty-",
                Date = DateTime.Now,
                Customer = OrderData.CustomerName,
                Project = OrderData.ProjectName,
                IssuedById = UserData.Id,
                IssuedBy = UserData.Name,
            };
            Navigation.To(new WarrantyView(warranty, Items), ViewData);
        }
        private bool CanAdd()
        {
            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void Info(Warranty warrnty)
        {
            Navigation.To(new WarrantyView(warrnty, Items), ViewData);
        }
        private bool CanAccessInfo(Warranty warrnty)
        {
            if (warrnty == null)
                return false;

            return true;
        }

        private void Print(Warranty warrnty)
        {
            WarrantyCertificateServices.PrintWarranty(warrnty.Id, ViewData);
        }
        private bool CanPrint(Warranty warrnty)
        {
            if (warrnty == null)
                return false;

            return true;
        }
    }
}