using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Data.Users;
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

        private JobFile _SelectedRequest;
        private ProductionPanel _SelectedPanel;

        private ObservableCollection<JobFile> _Requests;
        private ObservableCollection<ProductionPanel> _Panels;

        private ICollectionView _RequestsCollection;
        private ICollectionView _PanelsCollection;
        private int _ProductionPanels;
        private int _ProjectPanels;

        public NewOrderPanelsViewModel(Order order, ObservableCollection<ProductionPanel> orderPanels, IView view)
        {
            ViewData = view;
            OrderData = order;
            OrderPanels = orderPanels;
            UserData = Navigation.UserData;

            GetData();
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            SaveCommand = new RelayCommand<JobFile>(Save, CanAccessSave);
            AddPanelsCommand = new RelayCommand<JobFile>(AddPanels, CanAccessAddPanels);
            DeletePanelCommand = new RelayCommand<ProductionPanel>(DeletePanel, CanAccessDeletePanel);
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
        public JobFile SelectedRequest
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
        public ObservableCollection<JobFile> Requests
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
        public RelayCommand<JobFile> SaveCommand { get; }
        public RelayCommand<JobFile> AddPanelsCommand { get; }
        public RelayCommand<ProductionPanel> DeletePanelCommand { get; }

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

            int value = (int)item.GetType().GetProperty(columnName).GetValue(item);
            int checkValue = SelectedRequest.Number;

            if (value == checkValue)
                result = true;

            return result;
        }

        #endregion

        private void GetData()
        {
            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Production].[OrdersJobFiles(View)] " +
                        $"Where JobOrderId = {OrderData.JobOrderId} " +
                        $"Order By Number";
                Requests = new ObservableCollection<JobFile>(connection.Query<JobFile>(query));

                query = $"Select * From [Production].[Panels(View)] " +
                        $"Where JobOrderId  = {OrderData.JobOrderId} " +
                        $"Order By SN";
                Panels = new ObservableCollection<ProductionPanel>(connection.Query<ProductionPanel>(query));

                if (OrderPanels == null)
                {
                    query = $"Select * From [Production].[Orders(AllPanels)] " +
                            $"Where JobOrderId = {OrderData.JobOrderId} " +
                            $"Order By SN";
                    OrderPanels = new ObservableCollection<ProductionPanel>(connection.Query<ProductionPanel>(query));
                }
            }

            ProductionPanels = Panels.Sum(x => x.Qty);
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
            ProductionPanels = Panels.Sum(x => x.Qty);
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }

        private void Add()
        {
            JobFile delivery = new()
            {
                Date = DateTime.Now,
                Number = 0,
            };

            Requests.Add(delivery);
        }
        private bool CanAccessAdd()
        {
            if (Requests.Any(x => x.Number == 0))
                return false;

            return true;
        }

        private void Save(JobFile request)
        {
            int requestNumber;
            LoadingText = "Working...";
            LoadingIcon = Visibility.Visible;
            Navigation.OpenLoading(LoadingIcon, LoadingText);

            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * " +
                        $"From [Production].[Orders(View)] " +
                        $"Where JobOrderId = {OrderData.JobOrderId}";
                Order orderData = connection.QueryFirstOrDefault<Order>(query);

                if (orderData == null)
                {
                    orderData = new Order
                    {
                        JobOrderId = OrderData.JobOrderId,
                        Code = OrderData.Code,
                        CodeNumber = OrderData.CodeNumber,
                        CodeMonth = OrderData.CodeMonth,
                        CodeYear = OrderData.CodeYear,
                        Date = DateTime.Now,//
                        Project = OrderData.Project,
                        CustomerId = OrderData.CustomerId,
                        Customer = OrderData.Customer,
                        Quotation = OrderData.Quotation//
                    };
                    _ = connection.Insert(orderData);
                }

                query = $"Select IsNull(Reference,0) " +
                        $"From [Production].[JobFile(NewNumber)] " +
                        $"Where Year = {DateTime.Now.Year}";
                requestNumber = connection.QueryFirstOrDefault<int>(query) + 1;

                List<AddPanel> newPanels = new();
                AddPanel newPanel;
                DateTime requestDate = DateTime.Now;
                foreach (ProductionPanel panel in Panels.Where(i => i.Reference == request.Number))
                {
                    newPanel = new AddPanel
                    {
                        OrderId = orderData.Id,
                        PanelId = panel.PanelId,
                        InProduction = true,
                        Reference = requestNumber,
                        Date = requestDate
                    };
                    newPanels.Add(newPanel);
                }

                _ = connection.Insert(newPanels);
            }

            request.Number = requestNumber;
            Navigation.CloseLoading();
        }
        private bool CanAccessSave(JobFile request)
        {
            if (request == null)
                return false;

            if (request.Number != 0)
                return false;

            if (!Panels.Any(x => x.Reference == 0))
                return false;

            return true;
        }

        private void AddPanels(JobFile request)
        {
            Navigation.OpenPopup(new NewOrderPanelsPostingView(request, Panels, OrderPanels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanels(JobFile delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number != 0)
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

            return true;
        }
    }
}