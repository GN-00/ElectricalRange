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
    public class DeliveryRequestsViewModel : ViewModelBase
    {
        private string _RequestsIndicator = "-";
        private string _PanelsIndicator = "-";

        private int _SelectedRequestIndex;
        private int _SelectedPanelIndex;

        private DeliveryRequest _SelectedRequest;
        private ProductionPanel _SelectedPanel;

        private ObservableCollection<DeliveryRequest> _Requests;
        private ObservableCollection<ProductionPanel> _Panels;

        private ICollectionView _RequestsCollection;
        private ICollectionView _PanelsCollection;
        private int _ClosedPanels;
        private int _ProjectPanels;

        public DeliveryRequestsViewModel(Order order, IView view)
        {
            ViewData = view;
            OrderData = order;
            UserData = Navigation.UserData;

            GetData();
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            SaveCommand = new RelayCommand<DeliveryRequest>(Save, CanAccessSave);
            AddPanelsCommand = new RelayCommand<DeliveryRequest>(AddPanels, CanAccessAddPanels);
            DeletePanelCommand = new RelayCommand<ProductionPanel>(DeletePanel, CanAccessDeletePanel);
            PrintCommand = new RelayCommand<DeliveryRequest>(Print, CanAccessPrint);

            AttachCommand = new RelayCommand<DeliveryRequest>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<DeliveryRequest>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<DeliveryRequest>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<DeliveryRequest>(ReadAttachment, CanAccessReadAttachment);
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
        public DeliveryRequest SelectedRequest
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
        public ObservableCollection<DeliveryRequest> Requests
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
        public RelayCommand<DeliveryRequest> SaveCommand { get; }
        public RelayCommand<DeliveryRequest> AddPanelsCommand { get; }
        public RelayCommand<ProductionPanel> DeletePanelCommand { get; }
        public RelayCommand<DeliveryRequest> PrintCommand { get; }


        public RelayCommand<DeliveryRequest> AttachCommand { get; }
        public RelayCommand<DeliveryRequest> DeleteAttachmentCommand { get; }
        public RelayCommand<DeliveryRequest> DownloadAttachmentCommand { get; }
        public RelayCommand<DeliveryRequest> ReadAttachmentCommand { get; }


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
                query = $"Select * From [Production].[DeliveryRequests(View)] " +
                        $"Where JobOrderID = {OrderData.JobOrderId} " +
                        $"Order By Number";
                Requests = new ObservableCollection<DeliveryRequest>(connection.Query<DeliveryRequest>(query));

                query = $"Select * From [Production].[Panels(Delivered)] " +
                        $"Where JobOrderID  = {OrderData.JobOrderId} " +
                        $"Order By SN";
                Panels = new ObservableCollection<ProductionPanel>(connection.Query<ProductionPanel>(query));

                query = $"Select * From [Production].[Panels(View)] " +
                        $"Where JobOrderID  = {OrderData.JobOrderId} " +
                        $"Order By SN";
                OrderPanels = new ObservableCollection<ProductionPanel>(connection.Query<ProductionPanel>(query));
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
            DeliveryRequest delivery = new()
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

        private void Save(DeliveryRequest request)
        {
            LoadingText = "Working...";
            LoadingIcon = Visibility.Visible;
            Navigation.OpenLoading(LoadingIcon, LoadingText);

            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                int requestNumber;
                query = $"Select IsNull(Number,0) " +
                        $"From [Production].[PanelsDelivery(NewNumber)] " +
                        $"Where JobOrderId = {OrderData.JobOrderId}";

                requestNumber = connection.QueryFirstOrDefault<int>(query) + 1;

                DeliverPanel DeliverPanel;
                DateTime requestDate = DateTime.Now;
                List<DeliverPanel> closePanels = new();
                foreach (ProductionPanel panel in Panels.Where(i => i.Reference == request.Number))
                {
                    panel.Reference = requestNumber;
                    DeliverPanel = new DeliverPanel
                    {
                        JobOrderId = OrderData.JobOrderId,
                        PanelId = panel.PanelId,
                        Number = requestNumber,
                        Qty = panel.DeliveredQty,
                        Date = requestDate,
                    };
                    closePanels.Add(DeliverPanel);
                }

                _ = connection.Insert(closePanels);

                request.Number = requestNumber;
                request.JobOrderId = OrderData.JobOrderId;
                request.JobOrderCode = OrderData.Code;
            }

            Navigation.CloseLoading();
        }
        private bool CanAccessSave(DeliveryRequest request)
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

        private void AddPanels(DeliveryRequest request)
        {
            Navigation.OpenPopup(new DeliveryRequestView(request, Panels, OrderPanels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanels(DeliveryRequest delivery)
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

        private void Print(DeliveryRequest request)
        {
            //ProductionServices.PrintDeliveryRequest(request, ViewData);
        }
        private bool CanAccessPrint(DeliveryRequest request)
        {
            //if (request == null)
            //    return false;

            //if (request.Number == 0)
            //    return false;

            return false;
        }

        private void Attach(DeliveryRequest delivery)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (delivery.AttachmentId == null)
            {
                DeliveryAttachment attachment = new()
                {
                    DeliveryNumber = delivery.DeliveryCode,
                };

                Attachment.SaveFile<DeliveryAttachment>(attachment);

                delivery.AttachmentId = attachment.Id;
            }
            else
            {
                DeliveryAttachment attachment = new()
                {
                    Id = delivery.AttachmentId.GetValueOrDefault(),
                };

                Attachment.UpdateFile<DeliveryAttachment>(attachment);
            }

            IsLoading = false;
        }

        private bool CanAccessAttach(DeliveryRequest delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number == 0)
                return false;

            return true;
        }

        private void DeleteAttachment(DeliveryRequest delivery)
        {
            DeliveryAttachment attachment = new()
            {
                Id = delivery.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<DeliveryAttachment>(attachment);

            delivery.AttachmentId = null;
        }

        private bool CanAccessDeleteAttachment(DeliveryRequest delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.AttachmentId == null)
                return false;

            return true;
        }

        private void DownloadAttachment(DeliveryRequest delivery)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            DeliveryAttachment attachment = new()
            {
                Id = delivery.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DownloadFile<DeliveryAttachment>(attachment);

            IsLoading = false;
        }

        private bool CanAccessDownloadAttachment(DeliveryRequest delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(DeliveryRequest delivery)
        {
            DeliveryAttachment attachment = new()
            {
                Id = delivery.AttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<DeliveryAttachment>(attachment);
        }

        private bool CanAccessReadAttachment(DeliveryRequest delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.AttachmentId == null)
                return false;

            return true;
        }
    }
}
