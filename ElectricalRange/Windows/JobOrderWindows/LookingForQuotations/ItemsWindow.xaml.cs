using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Data.JobOrders;

namespace ProjectsNow.Windows.JobOrderWindows.LookingForQuotations
{
    public partial class ItemsWindow : Window
    {
        public QuotationRequest RequestData { get; set; }
        public ObservableCollection<QuotationRequestItem> ItemsData { get; set; }

        private CollectionViewSource viewData;
        private ObservableCollection<ItemPurchased> jobOrderItems;
        public ItemsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Store].[JobOrdersItems(PurchaseDetails)] Where JobOrderID = {RequestData.JobOrderID}";
                jobOrderItems = new ObservableCollection<ItemPurchased>(connection.Query<ItemPurchased>(query));
            }

            viewData = new CollectionViewSource() { Source = jobOrderItems };
            viewData.Filter += DataFilter;

            ItemsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (viewData.View.Cast<object>().Count() == 0)
            {
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                Navigation.Text = $"Items: {viewData.View.Cast<object>().Count()}";
            }
            else
            {
                Navigation.Text = $"Item: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
            }
        }
        private void ItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                Navigation.Text = $"Items: {viewData.View.Cast<object>().Count()}";
            }
            else
            {
                Navigation.Text = $"Item: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
            }
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is ItemPurchased item)
            {
                QtyWindow qtyWindow = new()
                {
                    ActionData = Enums.Actions.New,
                    ItemData = item,
                    RequestData = RequestData,
                    QuotationRequestItemsData = ItemsData,
                };
                _ = qtyWindow.ShowDialog();

                viewData.View.Refresh();
            }
        }

        #region Filters
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is ItemPurchased item)
                {
                    if (item.PurchasedQty + item.InOrderQty >= item.Qty)
                    {
                        e.Accepted = false;
                        return;
                    }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }
        #endregion
    }
}
