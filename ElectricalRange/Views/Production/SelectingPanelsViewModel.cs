using Dapper;
using Dapper.Contrib.Extensions;

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
    internal class SelectingPanelsViewModel : ViewModelBase
    {
        private IView checkPoint;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ProductionPanel _SelectedItem;
        private ObservableCollection<ProductionPanel> _Items;
        private ICollectionView _ItemsCollection;

        public SelectingPanelsViewModel(Order order, IView view)
        {
            checkPoint = view;
            OrderData = order;
            GetData();
            AddCommand = new RelayCommand<ProductionPanel>(Add, CanAdd);
            AddAllCommand = new RelayCommand(AddAll, CanAddAll);
            CloseCommand = new RelayCommand(Close, CanClose);
        }

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
        public ProductionPanel SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<ProductionPanel> Items
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
        public Order OrderData { get; }
        public ObservableCollection<ProductionPanel> PanelsData { get; }

        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        private void GetData()
        {
            string query = $"SELECT * FROM [Production].[Panels(View)] WHERE JobOrderId = {OrderData.JobOrderId}";
            using SqlConnection connection = new(Database.ConnectionString);
            Items = new ObservableCollection<ProductionPanel>(connection.Query<ProductionPanel>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.SortDescriptions.Add(new SortDescription("SN", ListSortDirection.Ascending));
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

        public RelayCommand<ProductionPanel> AddCommand { get; }
        public RelayCommand AddAllCommand { get; }
        public RelayCommand CloseCommand { get; }

        private void Add(ProductionPanel panel)
        {
            panel.SummaryQty = panel.Qty;
        }
        private bool CanAdd(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void AddAll()
        {
            var items = ItemsCollection.Cast<ProductionPanel>();
            foreach (ProductionPanel panel in items)
                panel.SummaryQty = panel.Qty;

            //Navigation.ClosePopup();
        }
        private bool CanAddAll()
        {
            if (Items == null)
                return false;

            if (Items.Count == 0)
                return false;

            return true;
        }

        private void Close()
        {
            Navigation.ClosePopup();
        }
        private bool CanClose()
        {
            return true;
        }
    }
}