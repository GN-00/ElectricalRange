using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    internal class SupplierInvoiceViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private SupplierInvoiceItem _SelectedItem;
        private ObservableCollection<SupplierInvoiceItem> _Items;
        public SupplierInvoiceViewModel(SupplierInvoice invoice, IView view)
        {
            InvoiceData = invoice;
            ViewData = view;

            GetData();
        }

        public SupplierInvoice InvoiceData { get; set; }
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
        public SupplierInvoiceItem SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<SupplierInvoiceItem> Items
        {
            get => _Items;
            set
            {
                if (SetValue(ref _Items, value))
                {
                    CreateCollectionView();
                }
            }
        }
        public ICollectionView ItemsCollection { get; set; }

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [Finance].[SuppliersInvoicesItems(View)] " +
                           $"Where InvoiceID = {InvoiceData.ID} " +
                           $"Order By Code";

            Items = new ObservableCollection<SupplierInvoiceItem>(connection.Query<SupplierInvoiceItem>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
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
    }
}