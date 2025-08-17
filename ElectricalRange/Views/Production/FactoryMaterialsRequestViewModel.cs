using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Data.Users;
using ProjectsNow.Printing;
using ProjectsNow.Printing.ProductionPages;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
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
        private Request _SelectedRequest;

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
            EditItemCommand = new RelayCommand<Item>(EditItem, CanAccessEditItem);
            DeleteItemCommand = new RelayCommand<Item>(DeleteItem, CanAccessDeleteItem);

            NextRequestCommand = new RelayCommand(NextRequest, CanAccessNextRequest);
            PreviousRequestCommand = new RelayCommand(PreviousRequest, CanAccessPreviousRequest);
            PrintCommand = new RelayCommand(PrintRequest, CanAccessPrint);

            AddItemToGroupCommand = new RelayCommand<CollectionViewGroup>(AddItem, CanAccessAddItem);
            CopyGroupCommand = new RelayCommand<CollectionViewGroup>(CopyGroup, CanAccessCopyGroup);
            DeleteGroupCommand = new RelayCommand<CollectionViewGroup>(DeleteGroup, CanAccessDeleteGroup);
            PrintGroupCommand = new RelayCommand<CollectionViewGroup>(PrintRequest, CanAccessPrint);

            PasteCommand = new RelayCommand<CollectionViewGroup>(Paste, CanAccessPaste);
        }

        public User UserData { get; }
        public Order OrderData { get; }
        public List<Item> Clipboard { get; set; }
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

        public class Request
        {
            public int RequestId { get; set; }
        }
        public Request SelectedRequest
        {
            get => _SelectedRequest;
            set => SetValue(ref _SelectedRequest, value);
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
        public ObservableCollection<Request> Requests { get; set; }
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
        public RelayCommand<Item> EditItemCommand { get; }
        public RelayCommand<Item> DeleteItemCommand { get; }
        public RelayCommand NextRequestCommand { get; }
        public RelayCommand PreviousRequestCommand { get; }
        public RelayCommand PrintCommand { get; }
        public RelayCommand<CollectionViewGroup> AddItemToGroupCommand { get; }
        public RelayCommand<CollectionViewGroup> CopyGroupCommand { get; }
        public RelayCommand<CollectionViewGroup> DeleteGroupCommand { get; }
        public RelayCommand<CollectionViewGroup> PrintGroupCommand { get; }
        public RelayCommand<CollectionViewGroup> PasteCommand { get; }


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

            query = $"Select * From [Production].[FactoryMaterialsRequestItems] " +
                    $"Where JobOrderID  = {OrderData.JobOrderId} " +
                    $"And Type = 'FMR'" +
                    $"Order By RequestId, Code";
            Items = new ObservableCollection<Item>(connection.Query<Item>(query));

            query = $"Select * From [Production].[FactoryMaterialsRequests] " +
                    $"Where JobOrderID  = {OrderData.JobOrderId} " +
                    $"Order By RequestId Desc";
            Requests = new ObservableCollection<Request>(connection.Query<Request>(query));

            SelectedRequest = Requests.FirstOrDefault();

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
            Item item = new();
            item.PanelQty = panel.Qty;
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select RequestId From [Production].[FactoryMaterialsRequestNumber] " +
                    $"Where JobOrderID = {OrderData.JobOrderId} ";
            item.RequestId = connection.QueryFirstOrDefault<int>(query) + 1;

            Request request = new() { RequestId = item.RequestId.Value };
            Requests.Add(request);
            SelectedRequest = request;

            item.Id = -1;
            item.Code = "New FMR";
            item.Description = "New Factory Materials Request";
            item.Type = "FMR";
            item.Unit = "pcs";
            item.Qty = 0;
            item.PanelId = panel.PanelId;
            item.JobOrderId = OrderData.JobOrderId;

            Items.Add(item);
        }
        private bool CanAccessAddRequest(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            if (Items.Any(x => x.Id == -1))
                return false;

            return true;
        }

        private void AddItem(CollectionViewGroup group)
        {
            if (group == null)
                return;

            Item groupItem = group.Items.OfType<Item>().FirstOrDefault();
            if (groupItem == null)
                return;

            Item item = new()
            {
                PanelQty = SelectedPanel.Qty,
                Type = "FMR",
                RequestId = groupItem.RequestId,
                PanelId = groupItem.PanelId,
                JobOrderId = groupItem.JobOrderId
            };

            Navigation.OpenPopup(new ItemView(item, Items), PlacementMode.Center, true);
        }
        private bool CanAccessAddItem(CollectionViewGroup group)
        {
            if (group == null)
                return false;

            return true;
        }

        private void CopyGroup(CollectionViewGroup group)
        {
            if (group == null)
                return;

            Item groupItem = group.Items.OfType<Item>().FirstOrDefault();
            if (groupItem == null)
                return;

            Clipboard = [];
            foreach (Item item in group.Items.OfType<Item>())
            {
                if (item.Id == -1)
                    continue;
                Item newItem = new()
                {
                    PanelQty = SelectedPanel.Qty,   
                    Code = item.Code,
                    Description = item.Description,
                    Type = item.Type,
                    Unit = item.Unit,
                    Qty = item.Qty,
                    PanelId = item.PanelId,
                    JobOrderId = item.JobOrderId,
                    RequestId = groupItem.RequestId
                };
                Clipboard.Add(newItem);
            }
        }
        private bool CanAccessCopyGroup(CollectionViewGroup group)
        {
            if (group == null)
                return false;

            return true;
        }

        private void Paste(CollectionViewGroup group)
        {
            if (group == null)
                return;

            Item groupItem = group.Items.OfType<Item>().FirstOrDefault();
            if (groupItem == null)
                return;

            using SqlConnection connection = new(Database.ConnectionString);

            foreach (Item item in Clipboard)
            {
                if (item.Id == -1)
                    continue;
                Item newItem = new()
                {
                    PanelQty = SelectedPanel.Qty,
                    Code = item.Code,
                    Description = item.Description,
                    Type = item.Type,
                    Unit = item.Unit,
                    Qty = item.Qty,
                    PanelId = groupItem.PanelId,
                    JobOrderId = item.JobOrderId,
                    RequestId = groupItem.RequestId
                };
                Items.Add(newItem);
                _ = connection.Insert(newItem);
            }

            MaterialsRequest request = new()
            {
                RequestId = groupItem.RequestId.Value,
                Date = DateTime.Now,
                JobOrderId = groupItem.JobOrderId,
                PanelId = groupItem.PanelId
            };

            string query = $"SELECT * FROM [Production].[MaterialsRequests] " +
                           $"WHERE RequestId = {request.Id} " +
                           $"And JobOrderId = {request.JobOrderId}";
            MaterialsRequest existingRequest = connection.QueryFirstOrDefault<MaterialsRequest>(query);
            if (existingRequest == null)
                _ = connection.Insert(request);

            Item checkItem = Items.FirstOrDefault(i => i.Id == -1);
            if (checkItem != null)
                Items.Remove(checkItem);

            Clipboard = null;
        }
        private bool CanAccessPaste(CollectionViewGroup group)
        {
            if (Clipboard == null)
                return false;

            if (group == null)
                return false;

            Item clipboardItem = Clipboard.FirstOrDefault();
            if (group.Items.OfType<Item>().Any(i => i.PanelId == clipboardItem.PanelId))
                return false;

            return true;
        }
        private void DeleteGroup(CollectionViewGroup group)
        {
            if (group == null)
                return;

            List<Item> groupItems = [.. group.Items.OfType<Item>()];

            if (groupItems == null)
                return;

            if (groupItems.Count == 0)
                return;

            Item groupItem = groupItems.FirstOrDefault();
            foreach (Item item in groupItems)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                _ = connection.Delete(item);
                _ = Items.Remove(item);
            }

            Requests.Remove(Requests.FirstOrDefault(r => r.RequestId == groupItem.RequestId.Value));
        }
        private bool CanAccessDeleteGroup(CollectionViewGroup group)
        {
            if (group == null)
                return false;

            return true;
        }

        private void PrintRequest(CollectionViewGroup group)
        {
            if (group == null)
                return;

            Item groupItem = group.Items.OfType<Item>().FirstOrDefault();
            if (groupItem == null)
                return;

            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [Production].[FactoryMaterialsRequests] " +
                           $"Where JobOrderId = {groupItem.JobOrderId} " +
                           $"And RequestId = {groupItem.RequestId.Value}";
            MaterialsRequest request = connection.QueryFirstOrDefault<MaterialsRequest>(query);

            request.PanelName = Panels
                .Where(p => p.PanelId == groupItem.PanelId)
                .FirstOrDefault().Name;

            request.JobOrderCode = OrderData.Code;
            request.Customer = OrderData.Customer;

            request.Items = [.. Items
                .Where(i => i.RequestId == groupItem.RequestId.Value && i.PanelId == groupItem.PanelId)];

            double pagesNumber = request.Items.Count / 13d;
            if (pagesNumber - Math.Truncate(pagesNumber) != 0)
                pagesNumber = Math.Truncate(pagesNumber) + 1;

            foreach (var item in request.Items)
                item.SN = request.Items.IndexOf(item) + 1;

            if (pagesNumber != 0)
            {
                request.Pages = (int)pagesNumber;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    MaterialsRequest pageRequest = new();
                    pageRequest.Update(request);

                    pageRequest.Page = i;
                    pageRequest.Items = [.. request.Items.Where(p => p.SN > ((i - 1) * 13) && p.SN <= (i * 13))];
                    FactoryMaterialsRequestPage requestForm = new(pageRequest);

                    elements.Add(requestForm);
                }

                Print.PrintPreview(elements, $"{OrderData.Code.Substring(0, OrderData.Code.IndexOf("/"))}-{request.RequestCode}", ViewData);
            }
            else
            {
                _ = MessageView.Show("Items",
                                     "There is no panels!!",
                                     MessageViewButton.OK,
                                     MessageViewImage.Warning);
            }
        }
        private bool CanAccessPrint(CollectionViewGroup group)
        {
            if (group == null)
                return false;

            return true;
        }

        private void EditItem(Item item)
        {
            Navigation.OpenPopup(new ItemView(item, Items), PlacementMode.Center, true);
        }
        private bool CanAccessEditItem(Item item)
        {
            if (item == null)
                return false;

            return true;
        }

        private void DeleteItem(Item item)
        {
            int requestId = item.RequestId.Value;
            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Delete(item);
            _ = Items.Remove(item);

            var checkItems = Items.Where(i => i.RequestId == requestId).ToList();
            if (checkItems.Count == 0)
                Requests.Remove(Requests.FirstOrDefault(r => r.RequestId == requestId));
        }
        private bool CanAccessDeleteItem(Item item)
        {
            if (item == null)
                return false;

            return true;
        }

        private void PrintRequest()
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


        private void NextRequest()
        {
            int index = Requests.IndexOf(SelectedRequest);
            if (index > 0)
                SelectedRequest = Requests[index - 1];
        }
        private bool CanAccessNextRequest()
        {
            return true;
        }

        private void PreviousRequest()
        {
            int index = Requests.IndexOf(SelectedRequest);
            if (index < Requests.Count - 1)
                SelectedRequest = Requests[index + 1];
        }
        private bool CanAccessPreviousRequest()
        {
            return true;
        }
    }
}