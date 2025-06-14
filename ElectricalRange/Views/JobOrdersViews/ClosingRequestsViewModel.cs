using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

using Panel = ProjectsNow.Data.JobOrders.TransactionPanel;

namespace ProjectsNow.Views.JobOrdersViews
{
    public class ClosingRequestsViewModel : ViewModelBase
    {
        private string _RequestsIndicator = "-";
        private string _PanelsIndicator = "-";

        private int _SelectedRequestIndex;
        private int _SelectedPanelIndex;

        private ClosingRequest _SelectedRequest;
        private Panel _SelectedPanel;

        private ObservableCollection<ClosingRequest> _Requests;
        private ObservableCollection<Panel> _Panels;

        private ICollectionView _RequestsCollection;
        private ICollectionView _PanelsCollection;
        private int _ClosedPanels;
        private int _ProjectPanels;

        public ClosingRequestsViewModel(JobOrder order, ObservableCollection<JPanel> orderPanels, IView view)
        {
            ViewData = view;
            OrderData = order;
            OrderPanels = orderPanels;
            UserData = Navigation.UserData;

            GetData();
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            SaveCommand = new RelayCommand<ClosingRequest>(Save, CanAccessSave);
            AddPanelsCommand = new RelayCommand<ClosingRequest>(AddPanels, CanAccessAddPanels);
            DeletePanelCommand = new RelayCommand<Panel>(DeletePanel, CanAccessDeletePanel);
            PrintCommand = new RelayCommand<ClosingRequest>(Print, CanAccessPrint);

            AttachCommand = new RelayCommand<ClosingRequest>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<ClosingRequest>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<ClosingRequest>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<ClosingRequest>(ReadAttachment, CanAccessReadAttachment);
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
        public ClosingRequest SelectedRequest
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
        public ObservableCollection<ClosingRequest> Requests
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
        public RelayCommand<ClosingRequest> SaveCommand { get; }
        public RelayCommand<ClosingRequest> AddPanelsCommand { get; }
        public RelayCommand<Panel> DeletePanelCommand { get; }
        public RelayCommand<ClosingRequest> PrintCommand { get; }

        public RelayCommand<ClosingRequest> AttachCommand { get; }
        public RelayCommand<ClosingRequest> DeleteAttachmentCommand { get; }
        public RelayCommand<ClosingRequest> DownloadAttachmentCommand { get; }
        public RelayCommand<ClosingRequest> ReadAttachmentCommand { get; }

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
                query = $"Select * From [JobOrder].[ClosingRequests(View)] " +
                        $"Where JobOrderID = {OrderData.Id} " +
                        $"Order By Number";
                Requests = new ObservableCollection<ClosingRequest>(connection.Query<ClosingRequest>(query));

                query = $"Select * From [JobOrder].[Panels(Closed)] " +
                        $"Where JobOrderID  = {OrderData.ID} " +
                        $"Order By PanelSN";
                Panels = new ObservableCollection<Panel>(connection.Query<Panel>(query));

                if (OrderPanels == null)
                {
                    OrderPanels = JPanelController.GetJobOrderPanels(connection, OrderData.ID);
                }
            }

            ClosedPanels = Panels.Sum(x => x.Qty);
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
            ClosedPanels = Panels.Sum(x => x.Qty);
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }

        private void Add()
        {
            ClosingRequest delivery = new()
            {
                Date = DateTime.Now,
                Number = "-New Close Request-",
            };

            Requests.Add(delivery);
        }
        private bool CanAccessAdd()
        {
            if (!UserData.ModifyJobOrders)
                return false;

            if (Requests.Any(x => x.Number == "-New Close Request-"))
                return false;

            return true;
        }

        private void Save(ClosingRequest request)
        {
            LoadingText = "Working...";
            LoadingIcon = Visibility.Visible;
            Navigation.OpenLoading(LoadingIcon, LoadingText);

            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                int requestNumber;
                string requestCode;
                query = $"Select IsNull(ClosingNumber,0) " +
                        $"From [JobOrder].[ClosingNumber] " +
                        $"Where Year = {DateTime.Now.Year}";

                requestNumber = connection.QueryFirstOrDefault<int>(query) + 1;
                requestCode = $"CR{requestNumber:0000}-{DateTime.Now.Month:00}-{DateTime.Now.Year.ToString().Substring(2, 2)}";

                List<Panel> newPanels = new();
                foreach (Panel panel in Panels.Where(i => i.Reference == request.Number))
                {
                    panel.Reference = requestCode;
                    newPanels.Add(panel);

                    JPanel panelData = OrderPanels.FirstOrDefault(i => i.PanelID == panel.PanelID);
                    panelData.ClosedQty += panel.Qty;

                    if (panelData.PanelQty == panelData.ClosedQty)
                    {
                        panelData.Status = Statuses.Closed.ToString();
                        _ = connection.Execute($"Update [JobOrder].[Panels]" +
                                               $" Set " +
                                               $"Status ='{Statuses.Closed}' " +
                                               $"Where PanelID = {panelData.PanelID}");
                    }
                }

                _ = connection.Insert(newPanels);

                request.Number = requestCode;
            }

            Navigation.CloseLoading();
        }
        private bool CanAccessSave(ClosingRequest request)
        {
            if (request == null)
                return false;

            if (request.Number != "-New Close Request-")
                return false;

            if (!Panels.Any(x => x.Reference == "-New Close Request-"))
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void AddPanels(ClosingRequest request)
        {
            Navigation.OpenPopup(new ClosingRequestView(request, Panels, OrderPanels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanels(ClosingRequest delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number != "-New Close Request-")
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

            if (panel.Reference != "-New Close Request-")
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void Print(ClosingRequest request)
        {
            JobOrderServices.PrintClosingRequest(OrderData.ID, request, ViewData);
        }
        private bool CanAccessPrint(ClosingRequest request)
        {
            if (request == null)
                return false;

            if (request.Number == "-New Close Request-")
                return false;

            return true;
        }


        private void Attach(ClosingRequest request)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (request.AttachmentId == null)
            {
                ClosingRequestAttachment attachment = new()
                {
                    ClosingNumber = request.Number,
                };

                Attachment.SaveFile<ClosingRequestAttachment>(attachment);

                request.AttachmentId = attachment.Id;
            }
            else
            {
                ClosingRequestAttachment attachment = new()
                {
                    Id = request.AttachmentId.GetValueOrDefault(),
                };

                Attachment.UpdateFile<ClosingRequestAttachment>(attachment);
            }

            IsLoading = false;
        }

        private bool CanAccessAttach(ClosingRequest request)
        {
            if (request == null)
                return false;

            if (request.Number == "-New Close Request-")
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeleteAttachment(ClosingRequest request)
        {
            ClosingRequestAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<ClosingRequestAttachment>(attachment);

            request.AttachmentId = null;
        }

        private bool CanAccessDeleteAttachment(ClosingRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DownloadAttachment(ClosingRequest request)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            ClosingRequestAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DownloadFile<ClosingRequestAttachment>(attachment);

            IsLoading = false;
        }

        private bool CanAccessDownloadAttachment(ClosingRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(ClosingRequest request)
        {
            ClosingRequestAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<ClosingRequestAttachment>(attachment);
        }

        private bool CanAccessReadAttachment(ClosingRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            return true;
        }
    }
}