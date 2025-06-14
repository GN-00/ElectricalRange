using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Printing.Finance;

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

namespace ProjectsNow.Views.FinanceView
{

    public class NotificationForReceiptViewModel : ViewModelBase
    {
        private string _NotificationsIndicator = "-";
        private int _SelectedNotificationIndex;
        private Notification _SelectedNotification;
        private ObservableCollection<Notification> _Notifications;

        private string _PanelsIndicator = "-";
        private int _SelectedPanelIndex;
        private ReceiptPanel _SelectedPanel;
        private ObservableCollection<ReceiptPanel> _Panels;

        private ICollectionView _NotificationsCollection;
        private ICollectionView _PanelsCollection;

        public NotificationForReceiptViewModel(JobOrder order, IView view)
        {
            ViewData = ViewData;
            JobOrderData = order;

            GetData();

            AddFormCommand = new RelayCommand(AddForm, CanAccessAddForm);
            InformationCommand = new RelayCommand<Notification>(Information, CanAccessInformation);
            PrintCommand = new RelayCommand<Notification>(Print, CanAccessPrint);
            AddPanelsCommand = new RelayCommand<Notification>(AddPanels, CanAccessAddPanels);
        }

        public JobOrder JobOrderData { get; }
        public User UserData => Navigation.UserData;
        public string NotificationsIndicator
        {
            get => _NotificationsIndicator;
            set => SetValue(ref _NotificationsIndicator, value);
        }
        public int SelectedNotificationIndex
        {
            get => _SelectedNotificationIndex;
            set
            {
                if (SetValue(ref _SelectedNotificationIndex, value))
                {
                    UpdateNotificationsIndicator();
                }
            }
        }
        public Notification SelectedNotification
        {
            get => _SelectedNotification;
            set
            {
                if (SetValue(ref _SelectedNotification, value))
                {
                    if (PanelsCollection != null)
                        PanelsCollection.Refresh();
                }
            }
        }
        public ObservableCollection<Notification> Notifications
        {
            get => _Notifications;
            private set
            {
                if (SetValue(ref _Notifications, value))
                {
                    CreateNotificationsCollectionView();
                }
            }
        }


        public ICollectionView NotificationsCollection
        {
            get => _NotificationsCollection;
            set => SetValue(ref _NotificationsCollection, value);
        }

        public string PanelsIndicator
        {
            get => _PanelsIndicator;
            set => SetValue(ref _PanelsIndicator, value);
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
        public ReceiptPanel SelectedPanel
        {
            get => _SelectedPanel;
            set => SetValue(ref _SelectedPanel, value);
        }
        public ObservableCollection<ReceiptPanel> Panels
        {
            get => _Panels;
            private set
            {
                if (SetValue(ref _Panels, value))
                {
                    CreatePanelsCollectionView();
                }
            }
        }
        public ICollectionView PanelsCollection
        {
            get => _PanelsCollection;
            set => SetValue(ref _PanelsCollection, value);
        }

        public RelayCommand AddFormCommand { get; }
        public RelayCommand<Notification> InformationCommand { get; }
        public RelayCommand<Notification> PrintCommand { get; }
        public RelayCommand<Notification> AddPanelsCommand { get; }

        #region Data Filter
        private bool DataFilter(object item)
        {
            if (SelectedNotification == null)
                return false;

            int value = ((ReceiptPanel)item).NotificationId;
            int checkValue = SelectedNotification.Id;

            if (value != checkValue)
            {
                return false;
            }

            return true;
        }

        #endregion

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * from [Accountant].[Notifications] " +
                    $"Where JobOrderId = {JobOrderData.ID} ";

            Notifications = new ObservableCollection<Notification>(connection.Query<Notification>(query));


            query = $"Select * from [Accountant].[NotificationsPanels(View)] " +
                    $"Where NotificationId In " +
                    $"(Select Id from [Accountant].[Notifications] " +
                    $"Where JobOrderId = {JobOrderData.ID})";

            Panels = new ObservableCollection<ReceiptPanel>(connection.Query<ReceiptPanel>(query));
        }
        private void CreateNotificationsCollectionView()
        {
            NotificationsCollection = CollectionViewSource.GetDefaultView(Notifications);

            NotificationsCollection.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            NotificationsCollection.CollectionChanged += CollectionNotificationsChanged;
        }
        private void CreatePanelsCollectionView()
        {
            PanelsCollection = CollectionViewSource.GetDefaultView(Panels);

            PanelsCollection.Filter = new Predicate<object>(DataFilter);
            PanelsCollection.SortDescriptions.Add(new SortDescription("SN", ListSortDirection.Ascending));
            PanelsCollection.CollectionChanged += CollectionPanelsChanged;
        }
        private void CollectionNotificationsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateNotificationsIndicator();
        }
        private void CollectionPanelsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePanelsIndicator();
        }
        private void UpdateNotificationsIndicator()
        {
            NotificationsIndicator = DataGridIndicator.Get(SelectedNotificationIndex, NotificationsCollection);
        }
        private void UpdatePanelsIndicator()
        {
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }


        private void AddForm()
        {
            string query;
            Notification notification = new();
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select IsNull(Number, 0) from [Accountant].[Notifications(Number)] " +
                        $"Where Year = {DateTime.Now.Year} ";

                int number = connection.QueryFirstOrDefault<int>(query) + 1;

                notification.Number = number;
                notification.Code = $"NR-{number:000}/{DateTime.Now.Year}";
                notification.JobOrderId = JobOrderData.ID;

                query = $"Select IsNull(Paid, 0) from [Finance].[JobOrders(View)] " +
                        $"Where ID = {JobOrderData.ID}";

                notification.Paid = connection.QueryFirstOrDefault<double>(query);

                query = $"Select QuotationFinalPrice As FinalPrice from [Finance].[JobOrders(View)] " +
                        $"Where ID = {JobOrderData.ID}";

                notification.FinalPrice = connection.QueryFirstOrDefault<double>(query);
            }

            Navigation.OpenPopup(new NotificationView(notification, Notifications), PlacementMode.Center, true);
        }
        private bool CanAccessAddForm()
        {
            if (!UserData.ModifyNotifyReceipts)
                return false;

            return true;
        }

        private void Information(Notification notification)
        {
            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select IsNull(Paid, 0) from [Finance].[JobOrders(View)] " +
                        $"Where ID = {JobOrderData.ID}";
                notification.Paid = connection.QueryFirstOrDefault<double>(query);

                query = $"Select QuotationFinalPrice As FinalPrice from [Finance].[JobOrders(View)] " +
                        $"Where ID = {JobOrderData.ID}";

                notification.FinalPrice = connection.QueryFirstOrDefault<double>(query);
            }

            Navigation.OpenPopup(new NotificationView(notification, Notifications), PlacementMode.Center, true);
        }
        private bool CanAccessInformation(Notification form)
        {
            if (form == null)
                return false;

            return true;
        }

        private void Print(Notification form)
        {
            Notification notificationData;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Accountant].[Notifications(View)] Where Id = {form.Id}";
                notificationData = connection.QueryFirstOrDefault<Notification>(query);
            }

            ObservableCollection<ReceiptPanel> panels =
                new(PanelsCollection.Cast<ReceiptPanel>());

            double pages = panels.Count / 10d;
            if (pages - Math.Truncate(pages) != 0)
            {
                pages = Math.Truncate(pages) + 1;
            }

            if (pages != 0)
            {
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pages; i++)
                {
                    NotificationForReceiptForm notificationForReceiptForm =
                        new(notificationData,
                        new ObservableCollection<ReceiptPanel>(panels.Where(x => panels.IndexOf(x) >= (i - 1) * 10 && panels.IndexOf(x) < i * 10)));

                    elements.Add(notificationForReceiptForm);
                }

                Printing.Print.PrintPreview(elements, $"Notification For Receipt {form.Code}", ViewData);
            }
            else
            {
                _ = MessageView.Show("Items", "There are no panels!!", MessageViewButton.OK, MessageViewImage.Warning);
            }
        }
        private bool CanAccessPrint(Notification form)
        {
            if (form == null)
                return false;

            return true;
        }

        private void AddPanels(Notification form)
        {
            Navigation.OpenPopup(new ReceiptPanelsView(form, Panels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanels(Notification form)
        {
            if (form == null)
                return false;

            if (!UserData.ModifyNotifyReceipts)
                return false;

            return true;
        }
    }
}