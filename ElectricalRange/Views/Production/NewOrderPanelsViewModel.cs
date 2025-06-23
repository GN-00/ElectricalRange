using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    public class NewOrderPanelsViewModel : ViewModelBase
    {
        private string _RequestsIndicator = "-";
        private string _PanelsIndicator = "-";

        private int _SelectedRequestIndex;
        private int _SelectedPanelIndex;

        private OrderRequest _SelectedRequest;
        private Panel _SelectedPanel;

        private ObservableCollection<OrderRequest> _Requests;
        private ObservableCollection<Panel> _Panels;

        private ICollectionView _RequestsCollection;
        private ICollectionView _PanelsCollection;
        private int _ProductionPanels;
        private int _ProjectPanels;

        public NewOrderPanelsViewModel(Order order, ObservableCollection<Panel> orderPanels, IView view)
        {
            ViewData = view;
            OrderData = order;
            OrderPanels = orderPanels;
            UserData = Navigation.UserData;

            GetData();
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            SaveCommand = new RelayCommand<OrderRequest>(Save, CanAccessSave);
            AddPanelsCommand = new RelayCommand<OrderRequest>(AddPanels, CanAccessAddPanels);
            DeletePanelCommand = new RelayCommand<Panel>(DeletePanel, CanAccessDeletePanel);
        }

        public User UserData { get; }
        public Order OrderData { get; }
        public ObservableCollection<Panel> OrderPanels { get; private set; }
        public string RequestsIndicator
        {
            get => _RequestsIndicator;
            set => SetValue(ref _RequestsIndicator, value);
        }
        public string PanelsIndicator
        {
            get => _PanelsIndicator;
            set => SetValue(ref _PanelsIndicator, value);
        }
        public int SelectedRequestIndex
        {
            get => _SelectedRequestIndex;
            set
            {
                if (SetValue(ref _SelectedRequestIndex, value))
                {
                    UpdateRequestsIndicator();
                }
            }
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
        public OrderRequest SelectedRequest
        {
            get => _SelectedRequest;
            set
            {
                if (SetValue(ref _SelectedRequest, value))
                {
                    PanelsCollection.Refresh();
                }
            }
        }
        public Panel SelectedPanel
        {
            get => _SelectedPanel;
            set => SetValue(ref _SelectedPanel, value);
        }
        public ObservableCollection<OrderRequest> Requests
        {
            get => _Requests;
            private set => SetValue(ref _Requests, value);
        }
        public ObservableCollection<Panel> Panels
        {
            get => _Panels;
            private set => SetValue(ref _Panels, value);
        }
        public ICollectionView RequestsCollection
        {
            get => _RequestsCollection;
            set => SetValue(ref _RequestsCollection, value);
        }
        public ICollectionView PanelsCollection
        {
            get => _PanelsCollection;
            set => SetValue(ref _PanelsCollection, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand<OrderRequest> SaveCommand { get; }
        public RelayCommand<OrderRequest> AddPanelsCommand { get; }
        public RelayCommand<Panel> DeletePanelCommand { get; }

        public int ProductionPanels
        {
            get => _ProductionPanels;
            set => SetValue(ref _ProductionPanels, value);
        }

        public int ProjectPanels
        {
            get => _ProjectPanels;
            set => SetValue(ref _ProjectPanels, value);
        }

        #region Data Filter
        private bool DataFilter(object item)
        {
            if (SelectedRequest == null)
                return false;

            bool result = false;
            string columnName = "Reference";

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = SelectedRequest.Number.ToUpper();

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
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Production].[OrderRequests(View)] " +
                        $"Where ProjectId = {OrderData.ProjectId} " +
                        $"Order By Number";
                Requests = new ObservableCollection<OrderRequest>(connection.Query<OrderRequest>(query));

                query = $"Select * From [Production].[Panels(View)] " +
                        $"Where OrderId  = {OrderData.Id} " +
                        $"Order By PanelSN";
                Panels = new ObservableCollection<Panel>(connection.Query<Panel>(query));

                //if (OrderPanels == null)
                //{
                //    OrderPanels = PanelController.GetOrderPanels(connection, OrderData.ID);
                //}
            }

            //ProductionPanels = Panels.Sum(x => x.Qty);
            //ProjectPanels = OrderPanels.Sum(x => x.PanelQty);

            CreateCollectionView();
        }
        private void CreateCollectionView()
        {
            RequestsCollection = CollectionViewSource.GetDefaultView(Requests);
            RequestsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            RequestsCollection.CollectionChanged += RequestsCollectionChanged;

            PanelsCollection = CollectionViewSource.GetDefaultView(Panels);
            PanelsCollection.Filter = new Predicate<object>(DataFilter);
            PanelsCollection.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
            PanelsCollection.CollectionChanged += PanelsCollectionChanged;
        }
        private void RequestsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateRequestsIndicator();
        }
        private void PanelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePanelsIndicator();
        }
        private void UpdateRequestsIndicator()
        {
            RequestsIndicator = DataGridIndicator.Get(SelectedRequestIndex, RequestsCollection);
        }
        private void UpdatePanelsIndicator()
        {
            //ProductionPanels = Panels.Sum(x => x.Qty);
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }

        private void Add()
        {
            OrderRequest delivery = new()
            {
                Date = DateTime.Now,
                Number = "-New Job File-",
            };

            Requests.Add(delivery);
        }
        private bool CanAccessAdd()
        {
            if (Requests.Any(x => x.Number == "-New Job File-"))
                return false;

            return true;
        }

        private void Save(OrderRequest request)
        {
            LoadingText = "Working...";
            LoadingIcon = Visibility.Visible;
            Navigation.OpenLoading(LoadingIcon, LoadingText);

            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                int requestNumber;
                string requestCode;
                query = $"Select IsNull(ProductionNumber,0) " +
                        $"From [Order].[ProductionNumber] " +
                        $"Where Year = {DateTime.Now.Year}";

                requestNumber = connection.QueryFirstOrDefault<int>(query) + 1;
                requestCode = $"JF{requestNumber:0000}-{DateTime.Now.Month:00}-{DateTime.Now.Year.ToString().Substring(2, 2)}";

                List<Panel> newPanels = new();
                foreach (Panel panel in Panels.Where(i => i.Reference == request.Number))
                {
                    panel.Reference = requestCode;
                    newPanels.Add(panel);

                    //Panel panelData = OrderPanels.FirstOrDefault(i => i.PanelID == panel.PanelID);
                    //panelData.ProductionQty += panel.Qty;

                    //if (panelData.PanelQty == panelData.ProductionQty)
                    //{
                    //    panelData.Status = Statuses.Production.ToString();
                    //    _ = connection.Execute($"Update [Order].[Panels]" +
                    //                           $" Set " +
                    //                           $"Status ='{Statuses.Production}' " +
                    //                           $"Where PanelID = {panelData.PanelID}");
                    //}
                }

                _ = connection.Insert(newPanels);

                request.Number = requestCode;
            }

            Navigation.CloseLoading();
        }
        private bool CanAccessSave(OrderRequest request)
        {
            if (request == null)
                return false;

            if (request.Number != "-New Job File-")
                return false;

            if (!Panels.Any(x => x.Reference == "-New Job File-"))
                return false;

            return true;
        }

        private void AddPanels(OrderRequest request)
        {
            //Navigation.OpenPopup(new OrderRequestView(request, Panels, OrderPanels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanels(OrderRequest delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number != "-New Job File-")
                return false;

            //if (!UserData.ModifyOrders)
            //    return false;

            return true;
        }

        private void DeletePanel(Panel panel)
        {
            _ = Panels.Remove(panel);
        }
        private bool CanAccessDeletePanel(Panel panel)
        {
            if (panel == null)
                return false;

            if (panel.Reference != "-New Job File-")
                return false;

            //if (!UserData.ModifyOrders)
            //    return false;

            return true;
        }
    }
}