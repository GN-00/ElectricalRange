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
    public class InspectionRequestsViewModel : ViewModelBase
    {
        private string _RequestsIndicator = "-";
        private string _PanelsIndicator = "-";

        private int _SelectedRequestIndex;
        private int _SelectedPanelIndex;

        private InspectionRequest _SelectedRequest;
        private Panel _SelectedPanel;

        private ObservableCollection<InspectionRequest> _Requests;
        private ObservableCollection<Panel> _Panels;

        private ICollectionView _RequestsCollection;
        private ICollectionView _PanelsCollection;
        private int _TestedPanels;
        private int _ProjectPanels;

        public InspectionRequestsViewModel(JobOrder order, ObservableCollection<JPanel> orderPanels, IView view)
        {
            ViewData = view;
            OrderData = order;
            OrderPanels = orderPanels;
            UserData = Navigation.UserData;

            GetData();
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            SaveCommand = new RelayCommand<InspectionRequest>(Save, CanAccessSave);
            AddPanelsCommand = new RelayCommand<InspectionRequest>(AddPanels, CanAccessAddPanels);
            DeletePanelCommand = new RelayCommand<Panel>(DeletePanel, CanAccessDeletePanel);
            PrintCommand = new RelayCommand<InspectionRequest>(Print, CanAccessPrint);

            AttachCommand = new RelayCommand<InspectionRequest>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<InspectionRequest>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<InspectionRequest>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<InspectionRequest>(ReadAttachment, CanAccessReadAttachment);
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
        public InspectionRequest SelectedRequest
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
        public ObservableCollection<InspectionRequest> Requests
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
        public RelayCommand<InspectionRequest> SaveCommand { get; }
        public RelayCommand<InspectionRequest> AddPanelsCommand { get; }
        public RelayCommand<Panel> DeletePanelCommand { get; }
        public RelayCommand<InspectionRequest> PrintCommand { get; }

        public RelayCommand<InspectionRequest> AttachCommand { get; }
        public RelayCommand<InspectionRequest> DeleteAttachmentCommand { get; }
        public RelayCommand<InspectionRequest> DownloadAttachmentCommand { get; }
        public RelayCommand<InspectionRequest> ReadAttachmentCommand { get; }

        public int TestedPanels
        {
            get => _TestedPanels;
            set => SetValue(ref _TestedPanels, value);
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
                query = $"Select * From [JobOrder].[InspectionRequests(View)] " +
                        $"Where JobOrderID = {OrderData.Id} " +
                        $"Order By Number";
                Requests = new ObservableCollection<InspectionRequest>(connection.Query<InspectionRequest>(query));

                query = $"Select * From [JobOrder].[Panels(Tested)] " +
                        $"Where JobOrderID  = {OrderData.ID} " +
                        $"Order By PanelSN";
                Panels = new ObservableCollection<Panel>(connection.Query<Panel>(query));

                if (OrderPanels == null)
                {
                    OrderPanels = JPanelController.GetJobOrderPanels(connection, OrderData.ID);
                }
            }

            TestedPanels = Panels.Sum(x => x.Qty);
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
            TestedPanels = Panels.Sum(x => x.Qty);
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }

        private void Add()
        {
            InspectionRequest delivery = new()
            {
                Date = DateTime.Now,
                Number = "-New QC Request-",
            };

            Requests.Add(delivery);
        }
        private bool CanAccessAdd()
        {
            if (!UserData.ModifyJobOrders)
                return false;

            if (Requests.Any(x => x.Number == "-New QC Request-"))
                return false;

            return true;
        }

        private void Save(InspectionRequest request)
        {
            LoadingText = "Working...";
            LoadingIcon = Visibility.Visible;
            Navigation.OpenLoading(LoadingIcon, LoadingText);

            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                int requestNumber;
                string requestCode;
                query = $"Select IsNull(InspectionNumber,0) " +
                        $"From [JobOrder].[InspectionNumber] " +
                        $"Where Year = {DateTime.Now.Year}";

                requestNumber = connection.QueryFirstOrDefault<int>(query) + 1;
                requestCode = $"IR{requestNumber:0000}-{DateTime.Now.Month:00}-{DateTime.Now.Year.ToString().Substring(2, 2)}";

                List<Panel> newPanels = new();
                foreach (Panel panel in Panels.Where(i => i.Reference == request.Number))
                {
                    panel.Reference = requestCode;
                    newPanels.Add(panel);

                    JPanel panelData = OrderPanels.FirstOrDefault(i => i.PanelID == panel.PanelID);
                    panelData.TestedQty += panel.Qty;

                    if (panelData.PanelQty == panelData.TestedQty)
                    {
                        panelData.Status = Statuses.Waiting_Approval.ToString();
                        _ = connection.Execute($"Update [JobOrder].[Panels]" +
                                               $" Set " +
                                               $"Status ='{Statuses.QC}' " +
                                               $"Where PanelID = {panelData.PanelID}");
                    }
                }

                _ = connection.Insert(newPanels);

                request.Number = requestCode;
            }

            Navigation.CloseLoading();
        }
        private bool CanAccessSave(InspectionRequest request)
        {
            if (request == null)
                return false;

            if (request.Number != "-New QC Request-")
                return false;

            if (!Panels.Any(x => x.Reference == "-New QC Request-"))
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void AddPanels(InspectionRequest request)
        {
            Navigation.OpenPopup(new InspectionRequestView(request, Panels, OrderPanels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanels(InspectionRequest delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number != "-New QC Request-")
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

            if (panel.Reference != "-New QC Request-")
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void Print(InspectionRequest request)
        {
            JobOrderServices.PrintInspectionRequest(OrderData.ID, request, ViewData);
        }
        private bool CanAccessPrint(InspectionRequest request)
        {
            if (request == null)
                return false;

            if (request.Number == "-New QC Request-")
                return false;

            return true;
        }


        private void Attach(InspectionRequest request)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (request.AttachmentId == null)
            {
                InspectionRequestAttachment attachment = new()
                {
                    InspectionNumber = request.Number,
                };

                Attachment.SaveFile<InspectionRequestAttachment>(attachment);

                request.AttachmentId = attachment.Id;
            }
            else
            {
                InspectionRequestAttachment attachment = new()
                {
                    Id = request.AttachmentId.GetValueOrDefault(),
                };

                Attachment.UpdateFile<InspectionRequestAttachment>(attachment);
            }

            IsLoading = false;
        }

        private bool CanAccessAttach(InspectionRequest request)
        {
            if (request == null)
                return false;

            if (request.Number == "-New QC Request-")
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeleteAttachment(InspectionRequest request)
        {
            InspectionRequestAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<InspectionRequestAttachment>(attachment);

            request.AttachmentId = null;
        }

        private bool CanAccessDeleteAttachment(InspectionRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DownloadAttachment(InspectionRequest request)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            InspectionRequestAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DownloadFile<InspectionRequestAttachment>(attachment);

            IsLoading = false;
        }

        private bool CanAccessDownloadAttachment(InspectionRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(InspectionRequest request)
        {
            InspectionRequestAttachment attachment = new()
            {
                Id = request.AttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<InspectionRequestAttachment>(attachment);
        }

        private bool CanAccessReadAttachment(InspectionRequest request)
        {
            if (request == null)
                return false;

            if (request.AttachmentId == null)
                return false;

            return true;
        }
    }
}
