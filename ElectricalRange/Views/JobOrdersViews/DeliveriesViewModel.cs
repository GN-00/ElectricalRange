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
    public class DeliveriesViewModel : ViewModelBase
    {
        private string _DeliveriesIndicator = "-";
        private string _PanelsIndicator = "-";

        private int _SelectedDeliveryIndex;
        private int _SelectedPanelIndex;

        private Delivery _SelectedDelivery;
        private Panel _SelectedPanel;

        private ObservableCollection<Delivery> _Deliveries;
        private ObservableCollection<Panel> _Panels;

        private ICollectionView _DeliveriesCollection;
        private ICollectionView _PanelsCollection;
        private int _DeliveredPanels;
        private int _ProjectPanels;

        public DeliveriesViewModel(JobOrder order, ObservableCollection<JPanel> orderPanels, IView view)
        {
            ViewData = view;
            OrderData = order;
            OrderPanels = orderPanels;
            UserData = Navigation.UserData;

            GetData();
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            SaveCommand = new RelayCommand<Delivery>(Save, CanAccessSave);
            AddPanelsCommand = new RelayCommand<Delivery>(AddPanels, CanAccessAddPanels);
            DeletePanelCommand = new RelayCommand<Panel>(DeletePanel, CanAccessDeletePanel);
            PrintCommand = new RelayCommand<Delivery>(Print, CanAccessPrint);

            AttachCommand = new RelayCommand<Delivery>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<Delivery>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<Delivery>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<Delivery>(ReadAttachment, CanAccessReadAttachment);
        }

        public User UserData { get; }
        public JobOrder OrderData { get; }
        public ObservableCollection<JPanel> OrderPanels { get; private set; }
        public string DeliveriesIndicator
        {
            get => _DeliveriesIndicator;
            set => SetValue(ref _DeliveriesIndicator, value);
        }
        public string PanelsIndicator
        {
            get => _PanelsIndicator;
            set => SetValue(ref _PanelsIndicator, value);
        }
        public int SelectedDeliveryIndex
        {
            get => _SelectedDeliveryIndex;
            set
            {
                if (SetValue(ref _SelectedDeliveryIndex, value))
                {
                    UpdateDeliveriesIndicator();
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
        public Delivery SelectedDelivery
        {
            get => _SelectedDelivery;
            set
            {
                if (SetValue(ref _SelectedDelivery, value))
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
        public ObservableCollection<Delivery> Deliveries
        {
            get => _Deliveries;
            private set => SetValue(ref _Deliveries, value);
        }
        public ObservableCollection<Panel> Panels
        {
            get => _Panels;
            private set => SetValue(ref _Panels, value);
        }
        public ICollectionView DeliveriesCollection
        {
            get => _DeliveriesCollection;
            set => SetValue(ref _DeliveriesCollection, value);
        }
        public ICollectionView PanelsCollection
        {
            get => _PanelsCollection;
            set => SetValue(ref _PanelsCollection, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand<Delivery> SaveCommand { get; }
        public RelayCommand<Delivery> AddPanelsCommand { get; }
        public RelayCommand<Panel> DeletePanelCommand { get; }
        public RelayCommand<Delivery> PrintCommand { get; }

        public RelayCommand<Delivery> AttachCommand { get; }
        public RelayCommand<Delivery> DeleteAttachmentCommand { get; }
        public RelayCommand<Delivery> DownloadAttachmentCommand { get; }
        public RelayCommand<Delivery> ReadAttachmentCommand { get; }

        public int DeliveredPanels
        {
            get => _DeliveredPanels;
            set => SetValue(ref _DeliveredPanels, value);
        }

        public int ProjectPanels
        {
            get => _ProjectPanels;
            set => SetValue(ref _ProjectPanels, value);
        }

        #region Data Filter
        private bool DataFilter(object item)
        {
            if (SelectedDelivery == null)
                return false;

            bool result = false;
            string columnName = "Reference";

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = SelectedDelivery.Number.ToUpper();

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
                query = $"Select * From [JobOrder].[Deliveries(View)] " +
                        $"Where JobOrderID = {OrderData.Id} " +
                        $"Order By Number";
                Deliveries = new ObservableCollection<Delivery>(connection.Query<Delivery>(query));

                query = $"Select * From [JobOrder].[Panels(Delivered)] " +
                        $"Where JobOrderID  = {OrderData.ID} " +
                        $"Order By PanelSN";
                Panels = new ObservableCollection<Panel>(connection.Query<Panel>(query));

                if (OrderPanels == null)
                {
                    OrderPanels = JPanelController.GetJobOrderPanels(connection, OrderData.ID);
                }
            }

            DeliveredPanels = Panels.Sum(x => x.Qty);
            ProjectPanels = OrderPanels.Sum(x => x.PanelQty);

            CreateCollectionView();
        }
        private void CreateCollectionView()
        {
            DeliveriesCollection = CollectionViewSource.GetDefaultView(Deliveries);
            DeliveriesCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            DeliveriesCollection.CollectionChanged += DeliveriesCollectionChanged;

            PanelsCollection = CollectionViewSource.GetDefaultView(Panels);
            PanelsCollection.Filter = new Predicate<object>(DataFilter);
            PanelsCollection.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
            PanelsCollection.CollectionChanged += PanelsCollectionChanged;
        }
        private void DeliveriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateDeliveriesIndicator();
        }
        private void PanelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePanelsIndicator();
        }
        private void UpdateDeliveriesIndicator()
        {
            DeliveriesIndicator = DataGridIndicator.Get(SelectedDeliveryIndex, DeliveriesCollection);
        }
        private void UpdatePanelsIndicator()
        {
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }

        private void Add()
        {
            Delivery delivery = new()
            {
                Date = DateTime.Now,
                Number = "-New Delivery Note-",
            };

            Deliveries.Add(delivery);
        }
        private bool CanAccessAdd()
        {
            if (!UserData.ModifyJobOrders)
                return false;

            if (Deliveries.Any(x => x.Number == "-New Delivery Note-"))
                return false;

            return true;
        }

        private void Save(Delivery delivery)
        {
            LoadingText = "Working...";
            LoadingIcon = Visibility.Visible;
            Navigation.OpenLoading(LoadingIcon, LoadingText);

            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                int deliveryNumber;
                string deliveryCode;
                query = $"Select IsNull(DeliveryNumber,0) " +
                        $"From [JobOrder].[DeliveryNumber] " +
                        $"Where Year = {DateTime.Now.Year}";

                deliveryNumber = connection.QueryFirstOrDefault<int>(query) + 1;
                deliveryCode = $"{(DateTime.Now.Year - Database.CompanyCreationYear - 5) * 1000 + deliveryNumber}{DateTime.Now.Month:00}{DateTime.Now.Year.ToString().Substring(2, 2)}";

                List<Panel> newPanels = new();
                foreach (Panel panel in Panels.Where(i => i.Reference == delivery.Number))
                {
                    panel.Reference = deliveryCode;
                    newPanels.Add(panel);

                    JPanel panelData = OrderPanels.FirstOrDefault(i => i.PanelID == panel.PanelID);
                    panelData.DeliveredQty += panel.Qty;

                    if (panelData.PanelQty == panelData.DeliveredQty)
                    {
                        panelData.Status = Statuses.Delivered.ToString();
                        _ = connection.Execute($"Update [JobOrder].[Panels] Set Status ='{Statuses.Delivered}' Where PanelID = {panelData.PanelID}");
                    }
                }

                _ = connection.Insert(newPanels);

                delivery.Number = deliveryCode;
            }

            Navigation.CloseLoading();
        }
        private bool CanAccessSave(Delivery delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number != "-New Delivery Note-")
                return false;

            if (!Panels.Any(x => x.Reference == "-New Delivery Note-"))
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void AddPanels(Delivery delivery)
        {
            Navigation.OpenPopup(new DeliveryView(delivery, Panels, OrderPanels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanels(Delivery delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number != "-New Delivery Note-")
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

            if (panel.Reference != "-New Delivery Note-")
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void Print(Delivery delivery)
        {
            DeliveryNoteSerices.Print(OrderData.ID, delivery, ViewData);
        }
        private bool CanAccessPrint(Delivery delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number == "-New Delivery Note-")
                return false;

            return true;
        }


        private void Attach(Delivery delivery)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (delivery.AttachmentId == null)
            {
                DeliveryAttachment attachment = new()
                {
                    DeliveryNumber = delivery.Number,
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

        private bool CanAccessAttach(Delivery delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.Number == "-New Delivery Note-")
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeleteAttachment(Delivery delivery)
        {
            DeliveryAttachment attachment = new()
            {
                Id = delivery.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<DeliveryAttachment>(attachment);

            delivery.AttachmentId = null;
        }

        private bool CanAccessDeleteAttachment(Delivery delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.AttachmentId == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DownloadAttachment(Delivery delivery)
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

        private bool CanAccessDownloadAttachment(Delivery delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(Delivery delivery)
        {
            DeliveryAttachment attachment = new()
            {
                Id = delivery.AttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<DeliveryAttachment>(attachment);
        }

        private bool CanAccessReadAttachment(Delivery delivery)
        {
            if (delivery == null)
                return false;

            if (delivery.AttachmentId == null)
                return false;

            return true;
        }
    }
}