using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Store;
using ProjectsNow.Enums;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows.Data;

namespace ProjectsNow.Windows.StoreWindows.ReturnToSuppliersWindows
{
    internal class ReturnToSupplierViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ItemTransaction _SelectedItem;
        private ObservableCollection<ItemTransaction> _Items;
        private SupplierInvoice _Invoice;
        private ICollectionView _ItemsView;

        public ReturnToSupplierViewModel(SupplierInvoice invoice)
        {
            Invoice = invoice;
            GetData();
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            ReturnCommand = new RelayCommand<ItemTransaction>(ReturnItem, CanReturnItem);
        }

        public int ReturenInvoiceId { get; set; }

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
        public ItemTransaction SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<ItemTransaction> Items
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

        public RelayCommand<ItemTransaction> ReturnCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }


        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Data.Database.ConnectionString);
            query = $"Select * From [Store].[JobOrdersInvoicesItems] " +
                    $"Where InvoiceID  = {Invoice.ID}";
            Items = new ObservableCollection<ItemTransaction>(connection.Query<ItemTransaction>(query));
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

        private void Save()
        {
            using SqlConnection connection = new(Data.Database.ConnectionString);
            string query = $"Select IsNull(Number, 0) + 1 From [Store].[ReturnInvoices(View)] " +
                           $"Where Year = {DateTime.Today.Year} " +
                           $"And Month = {DateTime.Today.Month} ";
            int returnInvoiceNumber = connection.QueryFirstOrDefault<int>(query);
            ReturnInvoice returnInvoice = new()
            {
                OriginalInvoiceID = Invoice.ID,
                Number = returnInvoiceNumber,
                Date = DateTime.Today,
            };
            _ = connection.Insert(returnInvoice);

            ReturenInvoiceId = returnInvoice.ID;

            foreach (ItemTransaction item in Items)
            {
                if (item.ID == 0)
                {
                    item.ReturnInvoiceID = returnInvoice.ID;
                    _ = connection.Insert(item);
                }
            }

            //Data.Finance.MoneyTransaction transaction = new Data.Finance.MoneyTransaction()
            //{
            //    Description = $"{Invoice.Number} Return Items {DateTime.Today:dd/MM/yyyy}",
            //    PurchaseOrderID = Invoice.PurchaseOrderID,
            //    SupplierID = Invoice.SupplierID,
            //    SupplierInvoiceID = Invoice.ID,
            //    JobOrderID = Invoice.JobOrderID,
            //    Date = DateTime.Now,
            //    Type = MoneyTransactionTypes.PurchaseOrder.ToString(),
            //    Amount = totalRetaurn,
            //};
            //_ = connection.Insert(transaction);

            //Data.Finance.MoneySubTransaction subTransaction = new Data.Finance.MoneySubTransaction()
            //{
            //    Description = $"{Invoice.Number} Return Items {DateTime.Today:dd/MM/yyyy}",
            //    PurchaseOrderID = Invoice.PurchaseOrderID,
            //    SupplierInvoiceID= Invoice.ID,
            //    Date = DateTime.Now,
            //    Type = MoneyTransactionTypes.SupplierInvoice.ToString(),
            //    Amount = totalRetaurn,
            //};
            //_ = connection.Insert(subTransaction);


        }

        private void Cancel()
        {
            
        }

        private void ReturnItem(ItemTransaction item)
        {
            if (item.Source != "New")
            {
                MessageWindows.MessageWindow.Show($"Return",
                    $"Only ItemFrom Orginal Invoice can return!",
                    MessageWindows.MessageWindowButton.OK,
                    MessageWindows.MessageWindowImage.Information);
            }

            QtyWindow qtyWindow = new()
            {
                ItemData = item,
                ItemsData = Items,
            };
            qtyWindow.ShowDialog();

        }
        private bool CanReturnItem(ItemTransaction item)
        {
            bool result = false;
            
            if (item != null)
            {
                if (item.Type == "Stock")
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