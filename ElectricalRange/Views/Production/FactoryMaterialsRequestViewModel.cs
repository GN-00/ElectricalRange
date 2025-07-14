using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Data.Users;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    public class FactoryMaterialsRequestViewModel : ViewModelBase
    {
        private string _PanelsIndicator = "-";
        private string _ItemsIndicator = "-";

        private int _SelectedPanelIndex;
        private int _SelectedItemIndex;

        private ProductionPanel _SelectedPanel;
        private Item _SelectedItem;

        private ObservableCollection<ProductionPanel> _Panels;
        private ObservableCollection<Item> _Items;

        private ICollectionView _RequestsCollection;
        private ICollectionView _ItemsCollection;

        public FactoryMaterialsRequestViewModel(Order order, IView view)
        {
            ViewData = view;
            OrderData = order;
            UserData = Navigation.UserData;

            GetData();
            AddRequestCommand = new RelayCommand<ProductionPanel>(AddRequest, CanAccessAddRequest);
            AddItemCommand = new RelayCommand<ProductionPanel>(AddItem, CanAccessAddItem);
            AddItemToGroupCommand = new RelayCommand<CollectionViewGroup>(AddItem, CanAccessAddItem);
            EditItemCommand = new RelayCommand<Item>(EditItem, CanAccessEditItem);
            DeleteItemCommand = new RelayCommand<Item>(DeleteItem, CanAccessDeleteItem);
            PrintCommand = new RelayCommand(Print, CanAccessPrint);
        }

        public User UserData { get; }
        public Order OrderData { get; }
        public string PanelsIndicator
        {
            get => _PanelsIndicator;
            set => SetValue(ref _PanelsIndicator, value);
        }
        public string ItemsIndicator
        {
            get => _ItemsIndicator;
            set => SetValue(ref _ItemsIndicator, value);
        }
        public int SelectedPanelIndex
        {
            get => _SelectedPanelIndex;
            set
            {
                if (SetValue(ref _SelectedPanelIndex, value))
                {
                    UpdatePanelsIndicator();
                }
            }
        }
        public int SelectedItemIndex
        {
            get => _SelectedItemIndex;
            set
            {
                if (SetValue(ref _SelectedItemIndex, value))
                {
                    UpdateItemsIndicator();
                }
            }
        }
        public ProductionPanel SelectedPanel
        {
            get => _SelectedPanel;
            set
            {
                if (SetValue(ref _SelectedPanel, value))
                {
                    ItemsCollection.Refresh();
                }
            }
        }
        public Item SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<ProductionPanel> Panels
        {
            get => _Panels;
            private set => SetValue(ref _Panels, value);
        }
        public ObservableCollection<Item> Items
        {
            get => _Items;
            private set => SetValue(ref _Items, value);
        }
        public ICollectionView PanelsCollection
        {
            get => _RequestsCollection;
            set => SetValue(ref _RequestsCollection, value);
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand<ProductionPanel> AddRequestCommand { get; }
        public RelayCommand<ProductionPanel> AddItemCommand { get; }
        public RelayCommand<CollectionViewGroup> AddItemToGroupCommand { get; }
        public RelayCommand<Item> EditItemCommand { get; }
        public RelayCommand<Item> DeleteItemCommand { get; }
        public RelayCommand PrintCommand { get; }


        #region Data Filter
        private bool DataFilter(object item)
        {
            if (SelectedPanel == null)
                return false;

            bool result = false;
            string columnName = "PanelId";

            int value = (int)item.GetType().GetProperty(columnName).GetValue(item);
            int checkValue = SelectedPanel.PanelId;

            if (value == checkValue)
            {
                result = true;
            }

            return result;
        }

        #endregion

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Production].[Panels(View)] " +
                    $"Where JobOrderID = {OrderData.JobOrderId} " +
                    $"Order By SN";
            Panels = new ObservableCollection<ProductionPanel>(connection.Query<ProductionPanel>(query));

            query = $"Select * From [Production].[PanelsItems] " +
                    $"Where JobOrderID  = {OrderData.JobOrderId} " +
                    $"And Type = 'FMR'" +
                    $"Order By RequestId, Code";
            Items = new ObservableCollection<Item>(connection.Query<Item>(query));

            CreateCollectionView();
        }
        private void CreateCollectionView()
        {
            PanelsCollection = CollectionViewSource.GetDefaultView(Panels);
            PanelsCollection.SortDescriptions.Add(new SortDescription("SN", ListSortDirection.Ascending));
            PanelsCollection.CollectionChanged += PanelsCollectionChanged;

            ItemsCollection = CollectionViewSource.GetDefaultView(Items);
            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("RequestId", ListSortDirection.Descending));
            ItemsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsCollection.GroupDescriptions.Add(new PropertyGroupDescription("RequestId"));
            ItemsCollection.CollectionChanged += ItemsCollectionChanged;
        }
        private void PanelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePanelsIndicator();
        }
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateItemsIndicator();
        }
        private void UpdatePanelsIndicator()
        {
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }
        private void UpdateItemsIndicator()
        {
            ItemsIndicator = DataGridIndicator.Get(SelectedItemIndex, ItemsCollection);
        }

        private void AddRequest(ProductionPanel panel)
        {
            Item item = new Item();
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select RequestId From [Production].[FactoryMaterialsRequestNumber] " +
                    $"Where JobOrderID = {OrderData.JobOrderId} ";
            item.RequestId = connection.QueryFirstOrDefault<int>(query) + 1;

            item.Code = "FMR";
            item.Description = "Factory Materials Request";
            item.Type = "FMR";
            item.Unit = "pcs";
            item.Qty = 0;
            item.PanelId = panel.PanelId;

            Items.Add(item);
            //Navigation.OpenPopup(new ItemView(item, Items), PlacementMode.Center , true);
        }
        private bool CanAccessAddRequest(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void AddItem(ProductionPanel panel)
        {
            //ProductionPanel delivery = new()
            //{
            //    Date = DateTime.Now,
            //    Number = 0,
            //};

            //Items.Add(delivery);
        }
        private bool CanAccessAddItem(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void AddItem(CollectionViewGroup panel)
        {
            //panel.ItemCount
            //CollectionViewGroup
            //ProductionPanel delivery = new()
            //{
            //    Date = DateTime.Now,
            //    Number = 0,
            //};

            //Items.Add(delivery);
        }
        private bool CanAccessAddItem(CollectionViewGroup panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void EditItem(Item item)
        {
            //_ = Items.Remove(item);
        }
        private bool CanAccessEditItem(Item item)
        {
            if (item == null)
                return false;

            return true;
        }

        private void DeleteItem(Item item)
        {
            //_ = Items.Remove(item);
        }
        private bool CanAccessDeleteItem(Item item)
        {
            if (item == null)
                return false;

            return true;
        }

        private void Print()
        {
            //ProductionServices.PrintProductionPanel(request, ViewData);
        }
        private bool CanAccessPrint()
        {
            //if (request == null)
            //    return false;

            //if (request.Number == 0)
            //    return false;

            return true;
        }
    }
}