using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;

using System.Collections.Specialized;

using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows.Data;

namespace ProjectsNow.Views.StoreViews
{

    public class SelectJobOrderViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private JobOrder _SelectedItem;
        private ObservableCollection<JobOrder> _Items;
        private ICollectionView _ItemsCollection;
        public SelectJobOrderViewModel(StockViewModel viewModel)
        {
            ViewModel = viewModel;
            GetData();
            NewOrderCommand = new RelayCommand<JobOrder>(NewOrder, CanNewOrder);
        }

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = "Select * From [Store].[JobOrders(HaveStock)]";
            Items = new ObservableCollection<JobOrder>(connection.Query<JobOrder>(query));
            Items.Insert(0, Database.Store);
        }

        public StockViewModel ViewModel { get; private set; }

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
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand<JobOrder> NewOrderCommand { get; }


        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

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

        private void NewOrder(JobOrder order)
        {
            ViewModel.JobOrderData = order;
            Navigation.ClosePopup();
        }
        private bool CanNewOrder(JobOrder order)
        {
            return true;
        }
    }
}