using Dapper;
using System;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Data;
using ProjectsNow.Data;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;
using ProjectsNow.Data.JobOrders;
using Dapper.Contrib.Extensions;
using ProjectsNow.Data.Users;

namespace ProjectsNow.Windows.JobOrderWindows.LookingForQuotations
{
    public partial class QuotationsRequestsWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }

        private CollectionViewSource RequestsView;
        private CollectionViewSource ItemsView;
        private ObservableCollection<QuotationRequest> requests;
        private ObservableCollection<QuotationRequestItem> items;
        public QuotationsRequestsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query;
                query = $"Select * From [JobOrder].[QuotationsRequests] Where JobOrderID = {JobOrderData.ID}";
                requests = new ObservableCollection<QuotationRequest>(connection.Query<QuotationRequest>(query));

                query = $"Select * From [JobOrder].[QuotationsRequestsItems] Where JobOrderID = {JobOrderData.ID}";
                items = new ObservableCollection<QuotationRequestItem>(connection.Query<QuotationRequestItem>(query));
            }

            RequestsView = new CollectionViewSource() { Source = requests };
            ItemsView = new CollectionViewSource() { Source = items };

            ItemsView.Filter += DataFilter;

            RequestsList.ItemsSource = RequestsView.View;
            ItemsList.ItemsSource = ItemsView.View;

            RequestsView.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Requests_CollectionChanged);
            ItemsView.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (requests.Count == 0)
            {
                Requests_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            if (items.Count == 0)
            {
                Items_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            DataContext = new { JobOrderData, UserData };
        }
        private void Requests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ItemsView.View.Refresh();
            int selectedIndex = RequestsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationPO.Text = $"Requests: {RequestsView.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationPO.Text = $"Request: {selectedIndex + 1} / {RequestsView.View.Cast<object>().Count()}";
            }
        }
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationItems.Text = $"Items: {ItemsView.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationItems.Text = $"Item: {selectedIndex + 1} / {ItemsView.View.Cast<object>().Count()}";
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ItemsView.View.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            }
        }
        private void RequestList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            ItemsView.View.Refresh();
            int selectedIndex = RequestsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationPO.Text = $"Requests: {RequestsView.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationPO.Text = $"Request: {selectedIndex + 1} / {RequestsView.View.Cast<object>().Count()}";
            }
        }
        private void ItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationItems.Text = $"Items: {ItemsView.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationItems.Text = $"Item: {selectedIndex + 1} / {ItemsView.View.Cast<object>().Count()}";
            }
        }

        #region Filters
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is QuotationRequestItem record)
                {
                    if (RequestsList.SelectedItem is QuotationRequest request)
                    {
                        if (record.QuotationRequestId != request.Id)
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

        private void NewRequest_Click(object sender, RoutedEventArgs e)
        {
            QuotationRequest request = new()
            {
                Date = DateTime.Now,
                JobOrderID = JobOrderData.ID,
                Number = requests.Count + 1,
            };

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Insert(request);
            }

            requests.Add(request);
        }
        private void DeleteRequest_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is QuotationRequest request)
            {
                bool canDelete = true;
                string query;
                CompanyPO checkPO;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    query = $"Select * From [Purchase].[Orders] Where QuotationRequestId = {request.Id}";
                    checkPO = connection.QueryFirstOrDefault<CompanyPO>(query);

                    if (checkPO != null)
                    {
                        canDelete = false;
                    }

                    if (canDelete)
                    {
                        query += $"Delete From [JobOrder].[QuotationsRequests] Where Id = {request.Id}; ";
                        query += $"Delete From [JobOrder].[QuotationsRequestsItems] Where QuotationRequestId = {request.Id}; ";
                        _ = connection.Execute(query);
                        _ = requests.Remove(request);
                    }

                }

                if (!canDelete)
                {
                    _ = MessageWindow.Show("Error", "You can't delete this Request!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    return;
                }
            }
        }

        private void AddItems_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is QuotationRequest request)
            {
                string query;
                bool canAdd = true;
                CompanyPO checkPO;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    query = $"Select * From [Purchase].[Orders] Where QuotationRequestId = {request.Id}";
                    checkPO = connection.QueryFirstOrDefault<CompanyPO>(query);
                }

                if (checkPO != null)
                {
                    canAdd = false;
                }

                if (canAdd)
                {
                    if (request.JobOrderID == 0) //Stock
                    {
                        ItemWindow itemWindow = new()
                        {
                            ActionData = Actions.New,
                            RequestData = request,
                            ItemData = null,
                            ItemsData = items,
                        };
                        _ = itemWindow.ShowDialog();
                    }
                    else
                    {
                        ItemsWindow itemsWindow = new()
                        {
                            RequestData = request,
                            ItemsData = items,
                        };
                        _ = itemsWindow.ShowDialog();
                    }
                }

                if (!canAdd)
                {
                    _ = MessageWindow.Show("Error", "You can't add new Item!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    return;
                }
            }
        }
        private void EditItems_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is QuotationRequest request)
            {
                if (ItemsList.SelectedItem is QuotationRequestItem transaction)
                {
                    string query;
                    bool canEdit = true;
                    CompanyPO checkPO;
                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        query = $"Select * From [Purchase].[Orders] Where QuotationRequestId = {request.Id}";
                        checkPO = connection.QueryFirstOrDefault<CompanyPO>(query);

                        if (checkPO != null)
                        {
                            canEdit = false;
                        }

                        if (canEdit)
                        {
                            if (request.JobOrderID == 0) //Stock
                            {
                                ItemWindow itemWindow = new()
                                {
                                    ActionData = Actions.Edit,
                                    RequestData = request,
                                    ItemData = transaction,
                                    ItemsData = items,
                                };
                                _ = itemWindow.ShowDialog();
                            }
                            else
                            {
                                ItemPurchased item;
                                query = $"Select * From [Store].[JobOrdersItems(PurchaseDetails)] Where JobOrderID = {request.JobOrderID} And Code = '{transaction.Code}'";
                                item = connection.QueryFirstOrDefault<ItemPurchased>(query);

                                QtyWindow qtyWindow = new()
                                {
                                    ActionData = Actions.Edit,
                                    ItemData = item,
                                    RequestData = request,
                                    QuotationRequestItemsData = null,
                                    QuotationRequestItemData = transaction,
                                };
                                _ = qtyWindow.ShowDialog();
                            }
                        }
                    }

                    if (!canEdit)
                    {
                        _ = MessageWindow.Show("Error", "You can't edit this Item!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                        return;
                    }
                }
            }
        }
        private void DeleteItems_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is QuotationRequestItem transaction)
            {
                string query;
                bool canDelete = true;
                CompanyPO checkPO;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    query = $"Select * From [Purchase].[Orders] Where QuotationRequestId = {transaction.QuotationRequestId}";
                    checkPO = connection.QueryFirstOrDefault<CompanyPO>(query);

                    if (checkPO != null)
                    {
                        canDelete = false;
                    }

                    if (canDelete)
                    {
                        _ = connection.Delete(transaction);
                        _ = items.Remove(transaction);
                    }
                }

                if (!canDelete)
                {
                    _ = MessageWindow.Show("Error", "You can't delete this Item!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    return;
                }
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is QuotationRequest request)
            {
                const double rows = 15;
                double pages = items.Count / rows;

                if (pages - Math.Truncate(pages) != 0)
                {
                    pages = Math.Truncate(pages) + 1;
                }

                ObservableCollection<QuotationRequestItem> transactions = new(items.Where(item => item.QuotationRequestId == request.Id));
                foreach (QuotationRequestItem transaction in transactions)
                {
                    transaction.SN = transactions.IndexOf(transaction) + 1;
                }
                List<FrameworkElement> elements = new();
                if (pages != 0)
                {
                    for (int i = 1; i <= pages; i++)
                    {
                        Printing.JobOrderPages.QuotationRequestPage quotationRequestPage = new()
                        {
                            Page = i,
                            Pages = pages,
                            JobOrderData = JobOrderData,
                            RequestData = request,
                            ItemsData = new ObservableCollection<QuotationRequestItem>(transactions.Where(p => p.SN > ((i - 1) * rows) && p.SN <= (i * rows)))
                        };
                        elements.Add(quotationRequestPage);
                    }

                    Printing.Print.PrintPreview(elements, $"Qoutation Request No. {request.Number}");
                }
                else
                {
                    _ = MessageWindow.Show("Statement", "There is no items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            using SqlConnection connection = new(Database.ConnectionString);
            UserData.JobOrderId = null;
            UserController.UpdateJobOrderID(connection, UserData);
        }
    }
}
