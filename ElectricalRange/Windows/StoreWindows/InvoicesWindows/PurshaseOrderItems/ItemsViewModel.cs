using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Store;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows.Data;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows.PurshaseOrderItems
{
    internal class ItemsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private CompanyPOTransaction _SelectedItem;
        private ObservableCollection<CompanyPOTransaction> _Items;
        private SupplierInvoice _Invoice;
        private ICollectionView _ItemsView;

        public ItemsViewModel(SupplierInvoice invoice)
        {
            Invoice = invoice;
            GetData();
            ReceiveItemsCommand = new RelayCommand<CompanyPOTransaction>(ReceiveItems, CanReceiveItems);
        }

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
        public CompanyPOTransaction SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<CompanyPOTransaction> Items
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
        public SupplierInvoice Invoice
        {
            get => _Invoice;
            set => SetValue(ref _Invoice, value);
        }
        public ICollectionView ItemsView
        {
            get => _ItemsView;
            set => SetValue(ref _ItemsView, value);
        }

        public RelayCommand<CompanyPOTransaction> ReceiveItemsCommand { get; }
        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Data.Database.ConnectionString);
            query = $"Select * From [Purchase].[TransactionsView] " +
                    $"Where PurchaseOrderID  = {Invoice.PurchaseOrderID}";
            Items = new ObservableCollection<CompanyPOTransaction>(connection.Query<CompanyPOTransaction>(query));
        }
        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            ItemsView.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsView);
        }

        private void ReceiveItems(CompanyPOTransaction item)
        {
            QtyWindow qtyWindow = new()
            {
                ItemData = item,
                ItemsData = Items,
                Invoice = Invoice,
            };
            _ = qtyWindow.ShowDialog();
        }
        private bool CanReceiveItems(CompanyPOTransaction item)
        {
            bool result = false;

            if (item != null)
            {
                if (!item.Received)
                {
                    if (item.FinalQty > 0)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }
    }
}