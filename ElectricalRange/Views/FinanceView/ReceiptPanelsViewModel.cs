using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    internal class ReceiptPanelsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ReceiptPanel _SelectedItem;
        private ObservableCollection<ReceiptPanel> _Items;
        private ICollectionView _ItemsCollection;

        public ReceiptPanelsViewModel(Notification notification, ObservableCollection<ReceiptPanel> panels)
        {
            UserData = Navigation.UserData;
            NotificationData = notification;
            PanelsData = panels;

            GetData();

            SaveCommand = new RelayCommand(Save, CanSave);
            AddCommand = new RelayCommand<ReceiptPanel>(Add, CanAdd);
        }

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [Accountant].[NotificationsPanels(JobOrdersView)] " +
                           $"Where JobOrderId = {NotificationData.JobOrderId} " +
                           $"And PanelId Not IN " +
                           $"(Select PanelId From [Accountant].[NotificationsPanels(View)] Where JobOrderId = {NotificationData.JobOrderId})";

            Items = new ObservableCollection<ReceiptPanel>(connection.Query<ReceiptPanel>(query));
        }

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.SortDescriptions.Add(new SortDescription("SN", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }


        public User UserData { get; }
        public Notification NotificationData { get; }
        public ObservableCollection<ReceiptPanel> PanelsData { get; }

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
        public ReceiptPanel SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<ReceiptPanel> Items
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


        public RelayCommand SaveCommand { get; }
        public RelayCommand<ReceiptPanel> AddCommand { get; }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        private void Save()
        {
            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            return true;
        }


        private void Add(ReceiptPanel panel)
        {
            using SqlConnection connection = new(Database.ConnectionString);
            panel.NotificationId = NotificationData.Id;

            _ = connection.Insert(panel);

            PanelsData.Add(panel);
            Items.Remove(panel);
        }

        private bool CanAdd(ReceiptPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }
    }
}