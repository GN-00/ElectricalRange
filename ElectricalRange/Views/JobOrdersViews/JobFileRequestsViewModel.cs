using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Panel = ProjectsNow.Data.JobOrders.TransactionPanel;

namespace ProjectsNow.Views.JobOrdersViews
{
    public class JobFileRequestsViewModel : ViewModelBase
    {
        private string _RequestsIndicator = "-";
        private string _PanelsIndicator = "-";

        private int _SelectedRequestIndex;
        private int _SelectedPanelIndex;

        private JobFileRequest _SelectedRequest;
        private Panel _SelectedPanel;

        private ObservableCollection<JobFileRequest> _Requests;
        private ObservableCollection<Panel> _Panels;

        private ICollectionView _RequestsCollection;
        private ICollectionView _PanelsCollection;
        private int _ProductionPanels;
        private int _ProjectPanels;

        public JobFileRequestsViewModel(JobOrder order, ObservableCollection<JPanel> orderPanels, IView view)
        {
            ViewData = view;
            OrderData = order;
            OrderPanels = orderPanels;
            UserData = Navigation.UserData;

            GetData();
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            SaveCommand = new RelayCommand<JobFileRequest>(Save, CanAccessSave);
            AddPanelsCommand = new RelayCommand<JobFileRequest>(AddPanels, CanAccessAddPanels);
            DeletePanelCommand = new RelayCommand<Panel>(DeletePanel, CanAccessDeletePanel);
            PrintCommand = new RelayCommand<JobFileRequest>(Print, CanAccessPrint);

            AttachCommand = new RelayCommand<JobFileRequest>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<JobFileRequest>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<JobFileRequest>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<JobFileRequest>(ReadAttachment, CanAccessReadAttachment);
        }

        public User UserData { get; }
        public JobOrder OrderData { get; }
        public ObservableCollection<JPanel> OrderPanels { get; private set; }
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
        public JobFileRequest SelectedRequest
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
        public ObservableCollection<JobFileRequest> Requests
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
        public RelayCommand<JobFileRequest> SaveCommand { get; }
        public RelayCommand<JobFileRequest> AddPanelsCommand { get; }
        public RelayCommand<Panel> DeletePanelCommand { get; }
        public RelayCommand<JobFileRequest> PrintCommand { get; }

        public RelayCommand<JobFileRequest> AttachCommand { get; }
        public RelayCommand<JobFileRequest> DeleteAttachmentCommand { get; }
        public RelayCommand<JobFileRequest> DownloadAttachmentCommand { get; }
        public RelayCommand<JobFileRequest> ReadAttachmentCommand { get; }

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
                query = $"Select * From [JobOrder].[JobFileRequests(View)] " +
                        $"Where JobOrderID = {OrderData.Id} " +
                        $"Order By Number";
                Requests = new ObservableCollection<JobFileRequest>(connection.Query<JobFileRequest>(query));

                query = $"Select * From [JobOrder].[Panels(Production)] " +
                        $"Where JobOrderID  = {OrderData.ID} " +
                        $"Order By PanelSN";
                Panels = new ObservableCollection<Panel>(connection.Query<Panel>(query));

                if (OrderPanels == null)
                {
                    OrderPanels = JPanelController.GetJobOrderPanels(connection, OrderData.ID);
                }
            }

            ProductionPanels = Panels.Sum(x => x.Qty);
            ProjectPanels = OrderPanels.Sum(x => x.PanelQty);

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
            ProductionPanels = Panels.Sum(x => x.Qty);
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }

        private void Add()
        {
            JobFileRequest delivery = new()
            {
                Date = DateTime.Now,
                Number = "-New Job File-",
            };

            Requests.Add(delivery);
        }
        private bool CanAccessAdd()
        {
            if (!UserData.ModifyJobOrders)
                return false;

            if (Requests.Any(x => x.Number == "-New Job File-"))
                return false;

            return true;
        }

        private void Save(JobFileRequest request)
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
                        $"From [JobOrder].[ProductionNumber] " +
                        $"Where Year = {DateTime.Now.Year}";

                requestNumber = connection.QueryFirstOrDefault<int>(query) + 1;
                requestCode = $"JF{requestNumber:0000}-{DateTime.Now.Month:00}-{DateTime.Now.Year.ToString().Substring(2, 2)}";

                List<Panel> newPanels = new();
                foreach (Panel panel in Panels.Where(i => i.Reference == request.Number))
                {
                    panel.Reference = requestCode;
                    newPanels.Add(panel);

                    JPanel panelData = OrderPanels.FirstOrDefault(i => i.PanelID == panel.PanelID);
                    panelData.ProductionQty += panel.Qty;

                    if (panelData.PanelQty == panelData.ProductionQty)
                    {
                        panelData.Status = Statuses.Production.ToString();
                        _ = connection.Execute($"Update [JobOrder].[Panels]" +
                                               $" Set " +
                                               $"Status ='{Statuses.Production}' " +
                                               $"Where PanelID = {panelData.PanelID}");
                    }
                }

                _ = connection.Insert(newPanels);

                request.Number = requestCode;
            }

            Navigation.CloseLoading();
        }
        private bool CanAccessSave(JobFileRequest request)
        {
            if (request == null)
                return false;

            if (request.Number != "-New Job File-")
                return false;

            if (!Panels.Any(x => x.Reference == "-New Job File-"))
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void AddPanels(JobFileRequest request)
        {
            Navigation.OpenPopup(new JobFileRequestView(request, Panels, OrderPanels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanels(JobFileRequest delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number != "-New Job File-")
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

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

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void Print(JobFileRequest request)
        {
            JobOrderServices.PrintJobFileRequest(OrderData.ID, request, ViewData);
        }
        private bool CanAccessPrint(JobFileRequest request)
        {
            if (request == null)
                return false;

            if (request.Number == "-New Job File-")
                return false;

            return true;
        }


        private void Attach(JobFileRequest request)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (request.AttachmentId == null)
            {
                ProductionAttachment attachment = new()
                {
                    ProductionNumber = request.Number,
                };

                Attachment.SaveFile<ProductionAttachment>(attachment);

                request.AttachmentId = attachment.Id;
            }
            else
            {
                ProductionAttachment attachment = new()
                {
                    Id = request.AttachmentId.GetValueOrDefault(),
                };

                Attachment.UpdateFile<ProductionAttachment>(attachment);
            }

            IsLoading = false;
        }

        private bool CanAccessAttach(JobFileRequest request)
        {
            if (request == null)
                return false;

            if (request.Number == "-New Job File-")
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeleteAttachment(JobFileRequest request)
        {
            ProductionAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<ProductionAttachment>(attachment);

            request.AttachmentId = null;
        }

        private bool CanAccessDeleteAttachment(JobFileRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DownloadAttachment(JobFileRequest request)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            ProductionAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DownloadFile<ProductionAttachment>(attachment);

            IsLoading = false;
        }

        private bool CanAccessDownloadAttachment(JobFileRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(JobFileRequest request)
        {
            ProductionAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<ProductionAttachment>(attachment);
        }

        private bool CanAccessReadAttachment(JobFileRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            return true;
        }
    }
}