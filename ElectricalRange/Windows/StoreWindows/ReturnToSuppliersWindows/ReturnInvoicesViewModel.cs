using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Store;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows.Data;

namespace ProjectsNow.Windows.StoreWindows.ReturnToSuppliersWindows
{
    internal class ReturnInvoicesViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ReturnInvoice _SelectedItem;
        private ObservableCollection<ReturnInvoice> _Items;
        private SupplierInvoice _Invoice;
        private ICollectionView _ItemsView;

        public ReturnInvoicesViewModel(SupplierInvoice invoice)
        {
            Invoice = invoice;
            GetData();
            PrintCommand = new RelayCommand<ReturnInvoice>(Print, CanPrint);
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
        public ReturnInvoice SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<ReturnInvoice> Items
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

        public RelayCommand<ReturnInvoice> PrintCommand { get; }


        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Store].[ReturnInvoices(View)] " +
                    $"Where OriginalInvoiceID  = {Invoice.ID}";
            Items = new ObservableCollection<ReturnInvoice>(connection.Query<ReturnInvoice>(query));
        }
        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
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

        private void Print(ReturnInvoice item)
        {
            Printing.PrintingHelper.PrintReturnInvoice(item.ID);
        }

        private bool CanPrint(ReturnInvoice item)
        {
            bool result = false;

            if (item != null)
                result = true;

            return result;
        }
    }
}