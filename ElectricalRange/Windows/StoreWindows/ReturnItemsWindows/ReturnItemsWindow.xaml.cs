using Dapper;

using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Store;
using ProjectsNow.Data.Users;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ProjectsNow.Windows.StoreWindows.ReturnItemsWindows
{
    public partial class ReturnItemsWindow : Window
    {
        public User UserData { get; set; }
        public ItemStock ItemData { get; set; }
        public JobOrder JobOrderData { get; set; }

        private CollectionViewSource viewData;
        private ObservableCollection<ItemTransaction> itemsData;
        public ReturnItemsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Store].[TransactionsView] Where JobOrderID = {JobOrderData.ID} And Code = '{ItemData.Code}'";
                itemsData = new ObservableCollection<ItemTransaction>(connection.Query<ItemTransaction>(query));
            }
            viewData = new CollectionViewSource() { Source = itemsData };
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
            if (ItemsList.SelectedItem is ItemTransaction item)
            {
                QtyWindow qtyWindow = new()
                {
                    ItemData = item,
                    JobOrderData = JobOrderData,
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
                if (e.Item is ItemTransaction item)
                {
                    if (item.FinalQty == 0)
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
