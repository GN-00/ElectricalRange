using Dapper;

using Microsoft.Data.SqlClient;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    internal class ReceiveItemsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private OrderItem _SelectedItem;
        private ObservableCollection<OrderItem> _Items;
        private ObservableCollection<OrderItem> _ItemsToAdd;
        private ICollectionView _ItemsCollection;

        public ReceiveItemsViewModel(Order order, IView view)
        {
            OrderData = order;

            GetData();

            AddCommand = new RelayCommand<OrderItem>(Add, CanAdd);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }


        public Order OrderData { get; private set; }

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
        public OrderItem SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<OrderItem> Items
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
        public ObservableCollection<OrderItem> ItemsToAdd
        {
            get => _ItemsToAdd;
            private set
            {
                if (SetValue(ref _ItemsToAdd, value))
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

        public RelayCommand<OrderItem> AddCommand { get; }
        public RelayCommand CancelCommand { get; }


        private void GetData()
        {
            string query = $"SELECT * FROM [Production].[] WHERE JobOrderId = {OrderData.JobOrderId}";
            using SqlConnection connection = new(Database.ConnectionString);
            Items = new ObservableCollection<OrderItem>(connection.Query<OrderItem>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += CollectionChanged;
        }
        private bool DataFilter(object obj)
        {
            if (!(obj is OrderItem item))
                return false;

            double value = item.Stock;
            double checkValue = item.Qty;

            if (value >= checkValue)
                return false;

            if (ItemsToAdd.Any(x => x.Code == item.Code))
                return false;

            return true;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }

        private void Add(OrderItem item)
        {
            //Navigation.OpenPopup(new ItemQtyView(item, Items, PurchaseItemsData), PlacementMode.Center, true);
            //Navigation.ClosePopupEvent += UpdateList;
        }
        private void UpdateList()
        {
            ItemsCollection.Refresh();
        }
        private bool CanAdd(OrderItem item)
        {
            //if (item == null)
            //    return false;

            //if (item.RemainingQty <= 0)
            //    return false;

            return true;
        }

        private void Cancel()
        {
            Navigation.Back();
        }
        private bool CanCancel()
        {
            return true;
        }
    }
}