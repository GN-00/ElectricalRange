using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    public class ClosingRequestsViewModel : ViewModelBase
    {
        private string _RequestsIndicator = "-";
        private string _PanelsIndicator = "-";

        private int _SelectedRequestIndex;
        private int _SelectedPanelIndex;

        private CloseRequest _SelectedRequest;
        private ProductionPanel _SelectedPanel;

        private ObservableCollection<CloseRequest> _Requests;
        private ObservableCollection<ProductionPanel> _Panels;

        private ICollectionView _RequestsCollection;
        private ICollectionView _PanelsCollection;
        private int _ClosedPanels;
        private int _ProjectPanels;

        public ClosingRequestsViewModel(Order order, IView view)
        {
            ViewData = view;
            OrderData = order;
            UserData = Navigation.UserData;

            GetData();
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            SaveCommand = new RelayCommand<CloseRequest>(Save, CanAccessSave);
            AddPanelsCommand = new RelayCommand<CloseRequest>(AddPanels, CanAccessAddPanels);
            DeletePanelCommand = new RelayCommand<ProductionPanel>(DeletePanel, CanAccessDeletePanel);
            PrintCommand = new RelayCommand<CloseRequest>(Print, CanAccessPrint);
        }

        public User UserData { get; }
        public Order OrderData { get; }
        public ObservableCollection<ProductionPanel> OrderPanels { get; private set; }
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
        public CloseRequest SelectedRequest
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
        public ProductionPanel SelectedPanel
        {
            get => _SelectedPanel;
            set => SetValue(ref _SelectedPanel, value);
        }
        public ObservableCollection<CloseRequest> Requests
        {
            get => _Requests;
            private set => SetValue(ref _Requests, value);
        }
        public ObservableCollection<ProductionPanel> Panels
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
        public RelayCommand<CloseRequest> SaveCommand { get; }
        public RelayCommand<CloseRequest> AddPanelsCommand { get; }
        public RelayCommand<ProductionPanel> DeletePanelCommand { get; }
        public RelayCommand<CloseRequest> PrintCommand { get; }


        public int ClosedPanels
        {
            get => _ClosedPanels;
            set => SetValue(ref _ClosedPanels, value);
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

            int value = (int)item.GetType().GetProperty(columnName).GetValue(item);
            int checkValue = SelectedRequest.Number;

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
                query = $"Select * From [Production].[ClosingRequests(View)] " +
                        $"Where JobOrderID = {OrderData.JobOrderId} " +
                        $"Order By Number";
                Requests = new ObservableCollection<CloseRequest>(connection.Query<CloseRequest>(query));

                query = $"Select * From [Production].[Panels(Closed)] " +
                        $"Where JobOrderID  = {OrderData.JobOrderId} " +
                        $"Order By SN";
                Panels = new ObservableCollection<ProductionPanel>(connection.Query<ProductionPanel>(query));

                if (OrderPanels == null)
                {
                    query = $"Select * From [Production].[Panels(View)] " +
                            $"Where JobOrderID  = {OrderData.JobOrderId} " +
                            $"Order By SN";
                    OrderPanels = new ObservableCollection<ProductionPanel>(connection.Query<ProductionPanel>(query));
                }
            }

            ClosedPanels = Panels.Sum(x => x.Qty);
            ProjectPanels = OrderPanels.Sum(x => x.Qty);

            CreateCollectionView();
        }
        private void CreateCollectionView()
        {
            RequestsCollection = CollectionViewSource.GetDefaultView(Requests);
            RequestsCollection.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Ascending));
            RequestsCollection.CollectionChanged += RequestsCollectionChanged;

            PanelsCollection = CollectionViewSource.GetDefaultView(Panels);
            PanelsCollection.Filter = new Predicate<object>(DataFilter);
            PanelsCollection.SortDescriptions.Add(new SortDescription("SN", ListSortDirection.Ascending));
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
            ClosedPanels = Panels.Sum(x => x.Qty);
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }

        private void Add()
        {
            CloseRequest delivery = new()
            {
                Date = DateTime.Now,
                Number = 0,
            };

            Requests.Add(delivery);
        }
        private bool CanAccessAdd()
        {
            if (!UserData.ModifyJobOrders)
                return false;

            if (Requests.Any(x => x.Number == 0))
                return false;

            return true;
        }

        private void Save(CloseRequest request)
        {
            LoadingText = "Working...";
            LoadingIcon = Visibility.Visible;
            Navigation.OpenLoading(LoadingIcon, LoadingText);

            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                int requestNumber;
                query = $"Select IsNull(Number,0) " +
                        $"From [Production].[PanelsClosing(NewNumber)] " +
                        $"Where JobOrderId = {OrderData.JobOrderId}";

                requestNumber = connection.QueryFirstOrDefault<int>(query) + 1;

                ClosePanel closePanel;
                DateTime requestDate = DateTime.Now;
                List<ClosePanel> closePanels = new();
                foreach (ProductionPanel panel in Panels.Where(i => i.Reference == request.Number))
                {
                    closePanel = new ClosePanel
                    {
                        JobOrderId = OrderData.JobOrderId,
                        PanelId = panel.PanelId,
                        Number = requestNumber,
                        Qty = panel.ClosedQty,
                        Date = requestDate,
                    };
                    closePanels.Add(closePanel);
                }

                _ = connection.Insert(closePanels);

                request.Number = requestNumber;
            }

            Navigation.CloseLoading();
        }
        private bool CanAccessSave(CloseRequest request)
        {
            if (request == null)
                return false;

            if (request.Number != 0)
                return false;

            if (!Panels.Any(x => x.Reference == 0))
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void AddPanels(CloseRequest request)
        {
            Navigation.OpenPopup(new ClosingRequestView(request, Panels, OrderPanels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanels(CloseRequest delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number != 0)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeletePanel(ProductionPanel panel)
        {
            _ = Panels.Remove(panel);
        }
        private bool CanAccessDeletePanel(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            if (panel.Reference != 0)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void Print(CloseRequest request)
        {
            ProductionServices.PrintCloseRequest(request, ViewData);
        }
        private bool CanAccessPrint(CloseRequest request)
        {
            if (request == null)
                return false;

            if (request.Number == 0)
                return false;

            return true;
        }
    }
}