using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows.Data;

namespace ProjectsNow.Views.JobOrdersViews
{
    public class PurchaseOrderViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private JPanelDetails _SelectedItem;
        private ObservableCollection<JPanelDetails> _Items;

        private ICollectionView _ItemsCollection;

        public PurchaseOrderViewModel(JobOrder order, IView view)
        {
            JobOrderData = order;
            ViewData = view;
            GetData();

            SaveCommand = new RelayCommand(Save, CanAccessSave);

            AttachCommand = new RelayCommand<PurchaseOrder>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<PurchaseOrder>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<PurchaseOrder>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<PurchaseOrder>(ReadAttachment, CanAccessReadAttachment);
        }

        private User UserData => Navigation.UserData;
        public JobOrder JobOrderData { get; }
        public PurchaseOrder NewData { get; private set; }

        public string Indicator
        {
            get => _Indicator;
            set => SetValue(ref _Indicator, value);
        }
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                if (SetValue(ref _SelectedIndex, value))
                {
                    UpdateIndicator();
                }
            }
        }
        public JPanelDetails SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<JPanelDetails> Items
        {
            get => _Items;
            private set
            {
                if (SetValue(ref _Items, value))
                {
                    CreateCollectionView();
                }
            }
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public bool IsNew => NewData.ID == 0;
        public bool IsEditable
        {
            get
            {
                if (!UserData.ModifyJobOrders)
                    return false;

                return true;
            }
        }

        public RelayCommand SaveCommand { get; }

        public RelayCommand<PurchaseOrder> AttachCommand { get; }
        public RelayCommand<PurchaseOrder> DeleteAttachmentCommand { get; }
        public RelayCommand<PurchaseOrder> DownloadAttachmentCommand { get; }
        public RelayCommand<PurchaseOrder> ReadAttachmentCommand { get; }

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [JobOrder].[PurchaseOrders(View)]" +
                    $"Where JobOrderID = {JobOrderData.Id} ";
            NewData = connection.QueryFirstOrDefault<PurchaseOrder>(query);

            NewData.QuotationID = JobOrderData.QuotationID;
            NewData.JobOrderID = JobOrderData.ID;
            NewData.JobOrderCode = JobOrderData.Code;
            NewData.Customer = JobOrderData.CustomerName;

            query = $"Select * From [JobOrder].[PanelsDetails] " +
                    $"Where JobOrderID = {JobOrderData.Id} " +
                    $"Order By PanelSN ";
            Items = new ObservableCollection<JPanelDetails>(connection.Query<JPanelDetails>(query));

            UpdateNetPrice();
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
            UpdateNetPrice();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }
        public void UpdateNetPrice()
        {
            NewData.NetPrice = Items.Sum(i => i.PanelsEstimatedPrice);
            NewData.VAT = JobOrderData.VAT;
            NewData.VATPercentage = JobOrderData.VAT * 100;
            NewData.VATValue = NewData.NetPrice * NewData.VAT;
            NewData.GrossPrice = NewData.NetPrice * (1 + NewData.VAT);
        }

        private void Save()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(NewData);
        }
        private bool CanAccessSave()
        {
            if (!UserData.ModifyJobOrders)
                return false;

            if (string.IsNullOrWhiteSpace(NewData.Number))
                return false;

            return true;
        }

        private void Attach(PurchaseOrder order)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (order.AttachmentId == null)
            {
                PurchaseOrderAttachment attachment = new()
                {
                    PurchaseOrderId = order.ID,
                };

                Attachment.SaveFile<PurchaseOrderAttachment>(attachment);

                order.AttachmentId = attachment.Id;
            }
            else
            {
                PurchaseOrderAttachment attachment = new()
                {
                    Id = order.AttachmentId.GetValueOrDefault(),
                };

                Attachment.UpdateFile<PurchaseOrderAttachment>(attachment);
            }

            IsLoading = false;
        }

        private bool CanAccessAttach(PurchaseOrder order)
        {
            if (order == null)
                return false;

            if (order.ID == 0)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeleteAttachment(PurchaseOrder order)
        {
            PurchaseOrderAttachment attachment = new()
            {
                Id = order.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<PurchaseOrderAttachment>(attachment);

            order.AttachmentId = null;
        }

        private bool CanAccessDeleteAttachment(PurchaseOrder order)
        {
            if (order == null)
                return false;

            if (order.AttachmentId == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DownloadAttachment(PurchaseOrder order)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            PurchaseOrderAttachment attachment = new()
            {
                Id = order.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DownloadFile<PurchaseOrderAttachment>(attachment);

            IsLoading = false;
        }

        private bool CanAccessDownloadAttachment(PurchaseOrder order)
        {
            if (order == null)
                return false;

            if (order.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(PurchaseOrder order)
        {
            PurchaseOrderAttachment attachment = new()
            {
                Id = order.AttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<PurchaseOrderAttachment>(attachment);
        }

        private bool CanAccessReadAttachment(PurchaseOrder order)
        {
            if (order == null)
                return false;

            if (order.AttachmentId == null)
                return false;

            return true;
        }
    }
}