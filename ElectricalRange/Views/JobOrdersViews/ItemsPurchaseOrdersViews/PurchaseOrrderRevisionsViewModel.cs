using Dapper;

using Microsoft.Data.SqlClient;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.PurchaseOrder;
using ProjectsNow.Services;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews
{
    internal class PurchaseOrrderRevisionsViewModel : ViewModelBase
    {

        private string _Indicator = "-";
        private int _SelectedIndex;
        private Revision _SelectedItem;
        private ObservableCollection<Revision> _Items;
        private ICollectionView _ItemsCollection;

        public PurchaseOrrderRevisionsViewModel(CompanyPO order, IView view)
        {

            OrderData = order;
            ViewData = view;
            GetData();

            InfoCommand = new RelayCommand<Revision>(Info, CanInfo);
        }




        public CompanyPO OrderData { get; }
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
        public Revision SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Revision> Items
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

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            Items = new ObservableCollection<Revision>();
            CompanyPO tempOrder = OrderData;
            while (tempOrder.OriginalOrderID != null)
            {
                query = $"Select * From [Purchase].[Orders] " +
                        $"Where ID = {tempOrder.OriginalOrderID} ";
                tempOrder = connection.QueryFirstOrDefault<CompanyPO>(query);

                if (tempOrder == null)
                    break;
                else
                {
                    Revision revision = new()
                    {
                        Id = tempOrder.ID,
                        Number = tempOrder.Revise + 1,
                        Code = tempOrder.Code,
                        Date = tempOrder.Date,
                    };

                    Items.Add(revision);
                }
            }
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Ascending));
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

        public RelayCommand<Revision> InfoCommand { get; }

        private void Info(Revision revision)
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            CompanyPO tempOrder;
            query = $"Select * From [Purchase].[Orders(Revisions)] " +
                    $"Where ID = {revision.Id} ";
            tempOrder = connection.QueryFirstOrDefault<CompanyPO>(query);

            PurchaseOrdersServices.Print(tempOrder, ViewData);

            Navigation.ClosePopup();
        }
        private bool CanInfo(Revision revision)
        {
            if (revision == null)
                return false;

            return true;
        }
    }
}