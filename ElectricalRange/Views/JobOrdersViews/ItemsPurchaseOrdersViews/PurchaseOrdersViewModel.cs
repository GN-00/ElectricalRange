using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.PurchaseOrder;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows.Data;

namespace ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews
{
    public class PurchaseOrdersViewModel : ViewModelBase, IAccess
    {
        private string _OrdersIndicator = "-";
        private string _ItemsIndicator = "-";

        private int _SelectedOrderIndex;
        private CompanyPO _SelectedOrder;
        private ObservableCollection<CompanyPO> _Orders;

        private int _SelectedItemIndex;
        private CompanyPOTransaction _SelectedItem;
        private ObservableCollection<CompanyPOTransaction> _Items;

        private ICollectionView _OrdersCollection;
        private ICollectionView _ItemsCollection;

        public PurchaseOrdersViewModel(JobOrder order, IView view)
        {
            JobOrderData = order;
            ViewData = view;

            GetData();

            AddOrderCommand = new RelayCommand(AddOrder, CanAddOrder);
            InformationCommand = new RelayCommand<CompanyPO>(Information, CanAccessInformation);
            ReviseCommand = new RelayCommand<CompanyPO>(Revise, CanAccessRevise);
            CancelCommand = new RelayCommand<CompanyPO>(Cancel, CanAccessCancel);
            PostItemsCommand = new RelayCommand<CompanyPO>(PostItems, CanPostItems);
            PurchaseOrderStatusCommand = new RelayCommand<CompanyPO>(PurchaseOrderStatus, CanAccessPurchaseOrderStatus);
            DeleteCommand = new RelayCommand<CompanyPO>(Delete, CanDelete);
            PrintCommand = new RelayCommand<CompanyPO>(Print, CanPrint);
            RevisionsCommand = new RelayCommand<CompanyPO>(Revisions, CanAccessRevisions);
            GetDataCommand = new RelayCommand(GetData);

            AttachCommand = new RelayCommand<CompanyPO>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<CompanyPO>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<CompanyPO>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<CompanyPO>(ReadAttachment, CanAccessReadAttachment);
        }

        public JobOrder JobOrderData { get; }
        private User UserData => Navigation.UserData;

        public string OrdersIndicator
        {
            get => _OrdersIndicator;
            set => SetValue(ref _OrdersIndicator, value);
        }
        public string ItemsIndicator
        {
            get => _ItemsIndicator;
            set => SetValue(ref _ItemsIndicator, value);
        }

        public int SelectedOrderIndex
        {
            get => _SelectedOrderIndex;
            set
            {
                if (SetValue(ref _SelectedOrderIndex, value))
                {
                    UpdateOrdersIndicator();
                }
            }
        }
        public int SelectedItemIndex
        {
            get => _SelectedItemIndex;
            set
            {
                if (SetValue(ref _SelectedItemIndex, value))
                {
                    UpdateItemsIndicator();
                }
            }
        }

        public CompanyPO SelectedOrder
        {
            get => _SelectedOrder;
            set
            {
                if (SetValue(ref _SelectedOrder, value))
                {
                    ItemsCollection.Refresh();
                }
            }
        }
        public CompanyPOTransaction SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }

        public ObservableCollection<CompanyPO> Orders
        {
            get => _Orders;
            private set => SetValue(ref _Orders, value);
        }
        public ObservableCollection<CompanyPOTransaction> Items
        {
            get => _Items;
            private set => SetValue(ref _Items, value);
        }

        public ICollectionView OrdersCollection
        {
            get => _OrdersCollection;
            set => SetValue(ref _OrdersCollection, value);
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand AddOrderCommand { get; }
        public RelayCommand<CompanyPO> InformationCommand { get; }
        public RelayCommand<CompanyPO> ReviseCommand { get; }
        public RelayCommand<CompanyPO> CancelCommand { get; }
        public RelayCommand<CompanyPO> PostItemsCommand { get; }
        public RelayCommand<CompanyPO> PurchaseOrderStatusCommand { get; }
        public RelayCommand<CompanyPO> DeleteCommand { get; }
        public RelayCommand<CompanyPO> PrintCommand { get; }
        public RelayCommand<CompanyPO> RevisionsCommand { get; }
        public RelayCommand GetDataCommand { get; }

        public RelayCommand<CompanyPO> AttachCommand { get; }
        public RelayCommand<CompanyPO> DeleteAttachmentCommand { get; }
        public RelayCommand<CompanyPO> DownloadAttachmentCommand { get; }
        public RelayCommand<CompanyPO> ReadAttachmentCommand { get; }

        private void GetData()
        {
            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Purchase].[OrdersView] " +
                        $"Where JobOrderID = {JobOrderData.ID}" +
                        $"Order By Number Desc";
                Orders = new ObservableCollection<CompanyPO>(connection.Query<CompanyPO>(query));

                query = $"Select * From [Purchase].[TransactionsView] " +
                        $"Where JobOrderID = {JobOrderData.ID} " +
                        $"Order By Code";
                Items = new ObservableCollection<CompanyPOTransaction>(connection.Query<CompanyPOTransaction>(query));
            }

            AppData.ReferencesListData = null;
            CreateCollectionView();
        }
        private void CreateCollectionView()
        {
            OrdersCollection = CollectionViewSource.GetDefaultView(Orders);
            OrdersCollection.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            OrdersCollection.CollectionChanged += OrdersCollectionChanged;

            ItemsCollection = CollectionViewSource.GetDefaultView(Items);
            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += ItemsCollectionChanged;
        }
        private void OrdersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateOrdersIndicator();
        }
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateItemsIndicator();
        }
        private void UpdateOrdersIndicator()
        {
            OrdersIndicator = DataGridIndicator.Get(SelectedOrderIndex, OrdersCollection);
        }
        private void UpdateItemsIndicator()
        {
            ItemsIndicator = DataGridIndicator.Get(SelectedItemIndex, ItemsCollection);
        }

        #region Data Filter
        private bool DataFilter(object item)
        {
            if (SelectedOrder == null)
                return false;

            bool result = true;
            string value = $"{item.GetType().GetProperty("PurchaseOrderID").GetValue(item)}".ToUpper();
            string checkValue = SelectedOrder.GetType().GetProperty("ID").GetValue(SelectedOrder).ToString();

            if (value != checkValue)
            {
                result = false;
            }

            return result;
        }
        #endregion

        private void AddOrder()
        {
            CompanyPO order = new()
            {
                Code = "-New Purchase Order-",
                Date = DateTime.Now,
                JobOrderID = JobOrderData.ID,
                VAT = AppData.VAT
            };

            Navigation.To(new PurchaseOrderView(order, Orders), ViewData);
        }
        private bool CanAddOrder()
        {
            return UserData.ModifyJobOrders;
        }

        private void Information(CompanyPO order)
        {
            Navigation.To(new PurchaseOrderView(order, Orders), ViewData);
        }
        private bool CanAccessInformation(CompanyPO order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void PostItems(CompanyPO order)
        {
            //PurchaseOrdersServices.PostItems(order, ViewData);
        }
        private bool CanPostItems(CompanyPO order)
        {
            return false;
        }

        private void PurchaseOrderStatus(CompanyPO order)
        {
            //PurchaseOrdersServices.PurchaseOrderStatus(order, ViewData);
        }
        private bool CanAccessPurchaseOrderStatus(CompanyPO order)
        {
            //if (order == null)
            //    return false;

            return false;
        }

        private void Revise(CompanyPO order)
        {
            bool canRevise = true;
            string query;
            ObservableCollection<CompanyPOTransaction> checkItems;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Purchase].[TransactionsView] Where PurchaseOrderID = {order.ID}";
                checkItems = new ObservableCollection<CompanyPOTransaction>(connection.Query<CompanyPOTransaction>(query));

                foreach (CompanyPOTransaction transaction in checkItems)
                {
                    if (transaction.ReceivedQty != 0)
                    {
                        canRevise = false;
                        break;
                    }
                }

                if (canRevise)
                {
                    CompanyPO newOrder = new();
                    newOrder.Update(order);
                    newOrder.OriginalOrderID = order.ID;
                    newOrder.Revise++;
                    newOrder.ReviseDate = newOrder.Date = DateTime.Now;
                    if (newOrder.Code.Contains("/R"))
                    {
                        newOrder.Code = newOrder.Code.Substring(0, newOrder.Code.Length - 2) + $"{newOrder.Revise:00}";
                    }
                    else
                    {
                        newOrder.Code += $"/R{newOrder.Revise:00}";
                    }


                    Orders.Remove(order);
                    Orders.Add(newOrder);

                    _ = connection.Insert(newOrder);

                    query = $"Select * From [Purchase].[TransactionsView] " +
                            $"Where PurchaseOrderID = {order.ID}";
                    var orderItems = new ObservableCollection<CompanyPOTransaction>(connection.Query<CompanyPOTransaction>(query));
                    foreach (CompanyPOTransaction transaction in orderItems)
                    {
                        transaction.PurchaseOrderID = newOrder.ID;
                        Items.Add(transaction);
                    }

                    _ = connection.Insert(orderItems);

                    order.Revised = true;
                    _ = connection.Update(order);
                }
            }

            if (!canRevise)
            {
                _ = MessageView.Show("Error", "You can't revise this PO!!", MessageViewButton.OK, MessageViewImage.Warning);
                return;
            }
        }
        private bool CanAccessRevise(CompanyPO order)
        {
            if (order == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void Cancel(CompanyPO order)
        {
            //PurchaseOrdersServices.Cancel(order);
        }
        private bool CanAccessCancel(CompanyPO order)
        {
            return false;
        }

        private void Delete(CompanyPO order)
        {
            bool canDelete = true;
            string query;
            ObservableCollection<CompanyPOTransaction> checkItems;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Purchase].[TransactionsView] Where PurchaseOrderID = {order.ID}";
                checkItems = new ObservableCollection<CompanyPOTransaction>(connection.Query<CompanyPOTransaction>(query));

                foreach (CompanyPOTransaction transaction in checkItems)
                {
                    if (transaction.ReceivedQty > 0)
                    {
                        canDelete = false;
                        break;
                    }
                }

                if (canDelete)
                {
                    query += $"Delete From [Purchase].[Orders] Where ID = {order.ID}; ";
                    query += $"Delete From [Purchase].[Transactions] Where PurchaseOrderID = {order.ID}; ";
                    _ = connection.Execute(query);

                    _ = Orders.Remove(order);
                }

            }

            if (!canDelete)
            {
                _ = MessageView.Show("Error", "You can't delete this PO!!", MessageViewButton.OK, MessageViewImage.Warning);
                return;
            }
        }
        private bool CanDelete(CompanyPO order)
        {
            if (order == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void Print(CompanyPO order)
        {
            PurchaseOrdersServices.Print(order, ViewData);
        }
        private bool CanPrint(CompanyPO order)
        {
            if (order == null)
                return false;

            return true;
        }

        private void Revisions(CompanyPO order)
        {
            PurchaseOrdersServices.Revisions(order, ViewData);
        }
        private bool CanAccessRevisions(CompanyPO order)
        {
            if (order == null)
                return false;

            if (order.Revise == 0)
                return false;

            return true;
        }


        private void Attach(CompanyPO order)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (order.AttachmentId == null)
            {
                PurchaseAttachment attachment = new()
                {
                    PurchaseOrderId = order.ID,
                };

                Attachment.SaveFile<PurchaseAttachment>(attachment);

                order.AttachmentId = attachment.Id;
            }
            else
            {
                PurchaseAttachment attachment = new()
                {
                    Id = order.AttachmentId.GetValueOrDefault(),
                };

                Attachment.UpdateFile<PurchaseAttachment>(attachment);
            }

            IsLoading = false;
        }
        private bool CanAccessAttach(CompanyPO order)
        {
            if (order == null)
                return false;

            if (order.ID == 0)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeleteAttachment(CompanyPO order)
        {
            PurchaseAttachment attachment = new()
            {
                Id = order.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<PurchaseAttachment>(attachment);

            order.AttachmentId = null;
        }
        private bool CanAccessDeleteAttachment(CompanyPO order)
        {
            if (order == null)
                return false;

            if (order.AttachmentId == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DownloadAttachment(CompanyPO order)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            PurchaseAttachment attachment = new()
            {
                Id = order.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DownloadFile<PurchaseAttachment>(attachment);

            IsLoading = false;
        }
        private bool CanAccessDownloadAttachment(CompanyPO order)
        {
            if (order == null)
                return false;

            if (order.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(CompanyPO order)
        {
            PurchaseAttachment attachment = new()
            {
                Id = order.AttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<PurchaseAttachment>(attachment);
        }
        private bool CanAccessReadAttachment(CompanyPO order)
        {
            if (order == null)
                return false;

            if (order.AttachmentId == null)
                return false;

            return true;
        }
    }
}