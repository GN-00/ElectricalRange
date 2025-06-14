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
    public class ApprovalsViewModel : ViewModelBase
    {
        private string _RequestsIndicator = "-";
        private string _PanelsIndicator = "-";

        private int _SelectedRequestIndex;
        private int _SelectedPanelIndex;

        private ApprovalRequest _SelectedRequest;
        private Panel _SelectedPanel;

        private ObservableCollection<ApprovalRequest> _Requests;
        private ObservableCollection<Panel> _Panels;

        private ICollectionView _RequestsCollection;
        private ICollectionView _PanelsCollection;
        private int _ApprovedPanels;
        private int _ProjectPanels;

        public ApprovalsViewModel(JobOrder order, ObservableCollection<JPanel> orderPanels, IView view)
        {
            ViewData = view;
            OrderData = order;
            OrderPanels = orderPanels;
            UserData = Navigation.UserData;

            GetData();
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            SaveCommand = new RelayCommand<ApprovalRequest>(Save, CanAccessSave);
            AddPanelsCommand = new RelayCommand<ApprovalRequest>(AddPanels, CanAccessAddPanels);
            DeletePanelCommand = new RelayCommand<Panel>(DeletePanel, CanAccessDeletePanel);
            PrintCommand = new RelayCommand<ApprovalRequest>(Print, CanAccessPrint);

            AttachCommand = new RelayCommand<ApprovalRequest>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<ApprovalRequest>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<ApprovalRequest>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<ApprovalRequest>(ReadAttachment, CanAccessReadAttachment);
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
        public ApprovalRequest SelectedRequest
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
        public ObservableCollection<ApprovalRequest> Requests
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
        public RelayCommand<ApprovalRequest> SaveCommand { get; }
        public RelayCommand<ApprovalRequest> AddPanelsCommand { get; }
        public RelayCommand<Panel> DeletePanelCommand { get; }
        public RelayCommand<ApprovalRequest> PrintCommand { get; }

        public RelayCommand<ApprovalRequest> AttachCommand { get; }
        public RelayCommand<ApprovalRequest> DeleteAttachmentCommand { get; }
        public RelayCommand<ApprovalRequest> DownloadAttachmentCommand { get; }
        public RelayCommand<ApprovalRequest> ReadAttachmentCommand { get; }

        public int ApprovedPanels
        {
            get => _ApprovedPanels;
            set => SetValue(ref _ApprovedPanels, value);
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
                query = $"Select * From [JobOrder].[ApprovalsRequests(View)] " +
                        $"Where JobOrderID = {OrderData.Id} " +
                        $"Order By Number";
                Requests = new ObservableCollection<ApprovalRequest>(connection.Query<ApprovalRequest>(query));

                query = $"Select * From [JobOrder].[Panels(Waiting_Approval)] " +
                        $"Where JobOrderID  = {OrderData.ID} " +
                        $"Order By PanelSN";
                Panels = new ObservableCollection<Panel>(connection.Query<Panel>(query));

                if (OrderPanels == null)
                {
                    OrderPanels = JPanelController.GetJobOrderPanels(connection, OrderData.ID);
                }
            }

            ApprovedPanels = Panels.Sum(x => x.Qty);
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
            ApprovedPanels = Panels.Sum(x => x.Qty);
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }

        private void Add()
        {
            ApprovalRequest delivery = new()
            {
                Date = DateTime.Now,
                Number = "-New Approval Request-",
            };

            Requests.Add(delivery);
        }
        private bool CanAccessAdd()
        {
            if (!UserData.ModifyJobOrders)
                return false;

            if (Requests.Any(x => x.Number == "-New Approval Request-"))
                return false;

            return true;
        }

        private void Save(ApprovalRequest request)
        {
            LoadingText = "Working...";
            LoadingIcon = Visibility.Visible;
            Navigation.OpenLoading(LoadingIcon, LoadingText);

            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                int requestNumber;
                string requestCode;
                query = $"Select IsNull(ApprovalNumber,0) " +
                        $"From [JobOrder].[ApprovalNumber] " +
                        $"Where Year = {DateTime.Now.Year}";

                requestNumber = connection.QueryFirstOrDefault<int>(query) + 1;
                requestCode = $"AR{requestNumber:0000}-{DateTime.Now.Month:00}-{DateTime.Now.Year.ToString().Substring(2, 2)}";

                List<Panel> newPanels = new();
                foreach (Panel panel in Panels.Where(i => i.Reference == request.Number))
                {
                    panel.Reference = requestCode;
                    newPanels.Add(panel);

                    JPanel panelData = OrderPanels.FirstOrDefault(i => i.PanelID == panel.PanelID);
                    panelData.ApprovedQty += panel.Qty;

                    if (panelData.PanelQty == panelData.ApprovedQty)
                    {
                        panelData.Status = Statuses.Waiting_Approval.ToString();
                        _ = connection.Execute($"Update [JobOrder].[Panels]" +
                                               $" Set " +
                                               $"Status ='{Statuses.Waiting_Approval}' " +
                                               $"Where PanelID = {panelData.PanelID}");
                    }
                }

                _ = connection.Insert(newPanels);

                request.Number = requestCode;
            }

            Navigation.CloseLoading();
        }
        private bool CanAccessSave(ApprovalRequest request)
        {
            if (request == null)
                return false;

            if (request.Number != "-New Approval Request-")
                return false;

            if (!Panels.Any(x => x.Reference == "-New Approval Request-"))
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void AddPanels(ApprovalRequest delivery)
        {
            Navigation.OpenPopup(new ApprovalView(delivery, Panels, OrderPanels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanels(ApprovalRequest delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number != "-New Approval Request-")
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

            if (panel.Reference != "-New Approval Request-")
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void Print(ApprovalRequest request)
        {
            JobOrderServices.PrintApprovals(OrderData.ID, request, ViewData);
        }
        private bool CanAccessPrint(ApprovalRequest request)
        {
            if (request == null)
                return false;

            if (request.Number == "-New Approval Request-")
                return false;

            return true;
        }


        private void Attach(ApprovalRequest request)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (request.AttachmentId == null)
            {
                ApprovalRequestAttachment attachment = new()
                {
                    ApprovalNumber = request.Number,
                };

                Attachment.SaveFile<ApprovalRequestAttachment>(attachment);

                request.AttachmentId = attachment.Id;
            }
            else
            {
                ApprovalRequestAttachment attachment = new()
                {
                    Id = request.AttachmentId.GetValueOrDefault(),
                };

                Attachment.UpdateFile<ApprovalRequestAttachment>(attachment);
            }

            IsLoading = false;
        }

        private bool CanAccessAttach(ApprovalRequest request)
        {
            if (request == null)
                return false;

            if (request.Number == "-New Approval Request-")
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeleteAttachment(ApprovalRequest request)
        {
            ApprovalRequestAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<ApprovalRequestAttachment>(attachment);

            request.AttachmentId = null;
        }

        private bool CanAccessDeleteAttachment(ApprovalRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DownloadAttachment(ApprovalRequest request)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            ApprovalRequestAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DownloadFile<ApprovalRequestAttachment>(attachment);

            IsLoading = false;
        }

        private bool CanAccessDownloadAttachment(ApprovalRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(ApprovalRequest request)
        {
            ApprovalRequestAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<ApprovalRequestAttachment>(attachment);
        }

        private bool CanAccessReadAttachment(ApprovalRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            return true;
        }
    }
}