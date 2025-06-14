using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Store;
using ProjectsNow.Windows.MessageWindows;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows
{
    public partial class PurchaseOrdersItemsWindow : Window
    {
        public int JobOrderID { get; set; }
        public JobOrder JobOrderData { get; set; }
        public SupplierInvoice InvoiceData { get; set; }
        public ObservableCollection<ItemTransaction> ItemsData { get; set; }

        private CollectionViewSource viewDataPO;
        private CollectionViewSource viewDataItems;
        private ObservableCollection<CompanyPO> orders;
        private ObservableCollection<CompanyPOTransaction> items;
        public PurchaseOrdersItemsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query;
                if (JobOrderData == null)
                {
                    JobOrderData = JobOrderController.JobOrder(connection, JobOrderID);
                }


                query = $"Select * From [Purchase].[OrdersView] " +
                        $"Where JobOrderID = {JobOrderData.ID}";
                orders = new ObservableCollection<CompanyPO>(connection.Query<CompanyPO>(query));

                query = $"Select * From [Purchase].[TransactionsView] " +
                        $"Where JobOrderID = {JobOrderData.ID}";
                items = new ObservableCollection<CompanyPOTransaction>(connection.Query<CompanyPOTransaction>(query));
            }

            viewDataPO = new CollectionViewSource() { Source = orders };
            viewDataItems = new CollectionViewSource() { Source = items };

            viewDataItems.Filter += DataFilter;

            POList.ItemsSource = viewDataPO.View;
            ItemsList.ItemsSource = viewDataItems.View;

            viewDataPO.View.CollectionChanged += new NotifyCollectionChangedEventHandler(PO_CollectionChanged);
            viewDataItems.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (orders.Count == 0)
            {
                PO_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            if (items.Count == 0)
            {
                Items_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        private void PO_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            int selectedIndex = POList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationPO.Text = $"Purchase Orders: {viewDataPO.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationPO.Text = $"Purchase Order: {selectedIndex + 1} / {viewDataPO.View.Cast<object>().Count()}";
                if (POList.SelectedItem is CompanyPO companyPO)
                {
                    SupplierName.Text = companyPO.SupplierName;
                }
            }
        }
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationItems.Text = $"Items: {viewDataItems.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationItems.Text = $"Item: {selectedIndex + 1} / {viewDataItems.View.Cast<object>().Count()}";
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewDataItems.View.SortDescriptions.Add(new SortDescription("PartNumber", ListSortDirection.Ascending));
            }
        }
        private void POList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            int selectedIndex = POList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationPO.Text = $"Purchase Orders: {viewDataPO.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationPO.Text = $"Purchase Order: {selectedIndex + 1} / {viewDataPO.View.Cast<object>().Count()}";
                if (POList.SelectedItem is CompanyPO companyPO)
                {
                    SupplierName.Text = companyPO.SupplierName;
                }
            }
        }
        private void ItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationItems.Text = $"Items: {viewDataItems.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationItems.Text = $"Item: {selectedIndex + 1} / {viewDataItems.View.Cast<object>().Count()}";
            }
        }

        #region Filters
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is CompanyPOTransaction record)
                {
                    if (POList.SelectedItem is CompanyPO order)
                    {
                        if (record.PurchaseOrderID != order.ID)
                        {
                            e.Accepted = false;
                            return;
                        }
                    }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }

        #endregion

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Post_Click(object sender, RoutedEventArgs e)
        {
            if (POList.SelectedItem is CompanyPO companyPO)
            {
                bool isReady = items.Any(i => i.PurchaseOrderID == companyPO.ID && i.FinalQty != 0);
                
                if (isReady)
                {
                    var result = MessageWindow.Show($"Posting",
                                                    $"Are you sure you want to post\n" +
                                                    $"invoice: {InvoiceData.Number} in\n" +
                                                    $"Purchase Order {companyPO.Code}?",
                                                    MessageWindowButton.YesNo, MessageWindowImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        InvoiceData.SupplierID = companyPO.SupplierID.Value;
                        InvoiceData.SupplierCode = companyPO.SupplierCode;
                        InvoiceData.SupplierName = companyPO.SupplierName;
                        InvoiceData.PurchaseOrderID = companyPO.ID;
                        InvoiceData.PurchaseOrder = companyPO.Code;

                        using (SqlConnection connection = new(Database.ConnectionString))
                        {
                            _ = connection.Update(InvoiceData);
                        }

                        PurshaseOrderItems.ItemsWindow itemsWindow =
                                new(InvoiceData);
                        Close();
                        itemsWindow.ShowDialog();
                    }
                }
                else
                {
                    _ = MessageWindow.Show($"Posting",
                                           $"All items already received!",
                                           MessageWindowButton.OK, 
                                           MessageWindowImage.Information);
                }
            }
        }
    }
}
