using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Suppliers;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews
{
    public class PurchaseOrderViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private CompanyPOTransaction _SelectedItem;
        private ObservableCollection<CompanyPOTransaction> _Items;
        private ObservableCollection<ItemPurchased> _JobOrderItems;

        private Supplier _SelectedSupplier;
        private ObservableCollection<Supplier> _SuppliersData;

        private Contact _SelectedContact;
        private ObservableCollection<Contact> _ContactsData;

        private ICollectionView _ItemsCollection;
        private ObservableCollection<string> _PaymentData;
        private ObservableCollection<string> _DeliveryPlacesData;
        private ObservableCollection<string> _DeliveryAddressesData;
        private ObservableCollection<string> _DeliveryPersonsData;

        public PurchaseOrderViewModel(CompanyPO order, ObservableCollection<CompanyPO> orders, IView view)
        {
            OrderData = order;
            OrdersData = orders;
            ViewData = view;
            NewData.Update(order);

            GetData();

            PrintCommand = new RelayCommand(Print, CanPrint);
            AddItemCommand = new RelayCommand(AddItem, CanAddItem);
            DeleteItemCommand = new RelayCommand<CompanyPOTransaction>(DeleteItem, CanDeleteItem);

            SaveCommand = new RelayCommand(Save, CanAccessSave);
            CreateCommand = new RelayCommand(Create, CanAccessCreate);
            StatusCommand = new RelayCommand(Status, CanAccessStatus);
            PrintCommand = new RelayCommand(Print, CanPrint);
        }

        public SupplierInvoice InvoiceData { get; set; }

        public CompanyPO OrderData { get; }
        public CompanyPO NewData { get; } = new CompanyPO();
        private ObservableCollection<CompanyPO> OrdersData { get; }
        private User UserData => Navigation.UserData;
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
        public Supplier SelectedSupplier
        {
            get => _SelectedSupplier;
            set
            {
                if (SetValue(ref _SelectedSupplier, value))
                {
                    if (SelectedSupplier != null)
                    {
                        NewData.SupplierName = SelectedSupplier.Name;
                        GetContacts();
                    }
                    else
                    {
                        NewData.SupplierID = null;
                        NewData.SupplierName = null;
                    }
                }
            }
        }
        public Contact SelectedContact
        {
            get => _SelectedContact;
            set
            {
                if (SetValue(ref _SelectedContact, value))
                {
                    if (SelectedContact != null)
                    {
                        NewData.SupplierAttention = SelectedContact.Name;
                    }
                    else
                    {
                        NewData.SupplierAttentionID = null;
                        NewData.SupplierAttention = null;
                    }
                }
            }
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
        public ObservableCollection<ItemPurchased> JobOrderItems
        {
            get => _JobOrderItems;
            private set => SetValue(ref _JobOrderItems, value);
        }
        public ObservableCollection<Supplier> SuppliersData
        {
            get => _SuppliersData;
            set => SetValue(ref _SuppliersData, value);
        }
        public ObservableCollection<Contact> ContactsData
        {
            get => _ContactsData;
            set => SetValue(ref _ContactsData, value);
        }
        public ObservableCollection<string> PaymentData
        {
            get => _PaymentData;
            set => SetValue(ref _PaymentData, value);
        }
        public ObservableCollection<string> DeliveryPlacesData
        {
            get => _DeliveryPlacesData;
            set => SetValue(ref _DeliveryPlacesData, value);
        }
        public ObservableCollection<string> DeliveryAddressesData
        {
            get => _DeliveryAddressesData;
            set => SetValue(ref _DeliveryAddressesData, value);
        }
        public ObservableCollection<string> DeliveryPersonsData
        {
            get => _DeliveryPersonsData;
            set => SetValue(ref _DeliveryPersonsData, value);
        }

        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }
        public bool IsEditing => CanAccessEdit();
        public bool HaveInvoices { get; private set; }
        private bool CanAccessEdit()
        {
            if (NewData == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            if (HaveInvoices)
                return false;

            return true;
        }

        public bool IsNew => NewData.ID == 0;

        public RelayCommand AddItemCommand { get; }
        public RelayCommand<CompanyPOTransaction> DeleteItemCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CreateCommand { get; }
        public RelayCommand StatusCommand { get; }
        public RelayCommand PrintCommand { get; }


        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Purchase].[TransactionsView] " +
                    $"Where PurchaseOrderID = {NewData.ID} " +
                    $"Order By Code";
            Items = new ObservableCollection<CompanyPOTransaction>(connection.Query<CompanyPOTransaction>(query));

            query = "Select Payment From [Purchase].[Orders] Where Payment Is Not Null Group By Payment";
            PaymentData = new ObservableCollection<string>(connection.Query<string>(query));

            query = $"Select * From [Store].[JobOrdersItems(PurchaseDetails)] " +
                    $"Where JobOrderID = {NewData.JobOrderID}" +
                    $"Order By Code";
            JobOrderItems = new ObservableCollection<ItemPurchased>(connection.Query<ItemPurchased>(query));

            query = "Select * From [Supplier].[Suppliers] Order By Name";
            SuppliersData = new ObservableCollection<Supplier>(connection.Query<Supplier>(query));

            if (NewData.ID != 0)
            {
                query = $"Select * From [Store].[Invoices] Where PurchaseOrderID = {NewData.ID}";
                InvoiceData = connection.QueryFirstOrDefault<SupplierInvoice>(query);
            }

            HaveInvoices = InvoiceData != null;
        }
        private void GetContacts()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Supplier].[Contacts] " +
                    $"Where SupplierID = {SelectedSupplier.ID} Order By Name ";
            ContactsData = new ObservableCollection<Contact>(connection.Query<Contact>(query));

            query = $"Select DeliverToPlace From [Purchase].[Orders] " +
                    $"Where SupplierID = {SelectedSupplier.ID} " +
                    $"And DeliverToPlace Is Not Null " +
                    $"Group By DeliverToPlace";
            DeliveryPlacesData = new ObservableCollection<string>(connection.Query<string>(query));

            query = $"Select DeliveryAddress From [Purchase].[Orders] " +
                    $"Where SupplierID = {SelectedSupplier.ID} And " +
                    $"DeliveryAddress Is Not Null " +
                    $"Group By DeliveryAddress";
            DeliveryAddressesData = new ObservableCollection<string>(connection.Query<string>(query));

            query = $"Select DeliverToPerson From [Purchase].[Orders] " +
                    $"Where SupplierID = {SelectedSupplier.ID} " +
                    $"And DeliverToPerson Is Not Null " +
                    $"Group By DeliverToPerson";
            DeliveryPersonsData = new ObservableCollection<string>(connection.Query<string>(query));
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
            UpdateNetPrice();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }

        private void AddItem()
        {
            if (NewData.JobOrderID == 0)//Stock
            {
                CompanyPOTransaction item = new()
                {
                    JobOrderID = NewData.JobOrderID,
                    VAT = NewData.VAT,
                };
                Navigation.OpenPopup(new ItemView(item, Items), System.Windows.Controls.Primitives.PlacementMode.Center, true);
            }
            else
            {
                Navigation.To(new JobOrderItemsView(NewData, Items, JobOrderItems), ViewData);
            }
        }
        private bool CanAddItem()
        {
            return CanAccessEdit();
        }

        private void DeleteItem(CompanyPOTransaction item)
        {
            MessageBoxResult result =
                MessageView.Show($"Delete",
                                 $"Are you sure want to Delete\n{item.Code}:\n{item.Description}?",
                                 MessageViewButton.YesNo,
                                 MessageViewImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var CheckSalesItem = JobOrderItems.FirstOrDefault(i => i.Code == item.Code);

                if (CheckSalesItem != null)
                {
                    CheckSalesItem.InOrderQty -= item.Qty;
                }

                Items.Remove(item);
            }
        }
        private bool CanDeleteItem(CompanyPOTransaction item)
        {
            if (item == null)
                return false;

            return CanAccessEdit();
        }

        private void Create()
        {
            bool isReady = true;
            string message = "Please Enter:";
            if (NewData.SupplierID == null) { message += $"\n  Supplier."; isReady = false; }
            if (NewData.SupplierAttentionID == null) { message += $"\n  Contact."; isReady = false; }
            if (NewData.QuotationCode == null) { message += $"\n  Quotation Code."; isReady = false; }
            if (NewData.Payment == null) { message += $"\n  Payment."; isReady = false; }
            if (Items.Count == 0) { message += $"\n  Items."; isReady = false; }

            if (isReady)
            {
                int numberPO;
                string query;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    query = $"Select * From [Purchase].[OrderNumber] " +
                            $"Where Year = {NewData.Date.Year}";
                    numberPO = connection.QueryFirstOrDefault<int>(query);
                    NewData.Number = ++numberPO;

                    if (NewData.Number == 1)
                    {
                        query = $"Select * From [Purchase].[OrderNumber] " +
                                $"Where Year = {NewData.Date.Year - 1}";
                        numberPO = connection.QueryFirstOrDefault<int>(query);

                        numberPO = numberPO / 1000;

                        NewData.Number = ((numberPO + 1) * 1000) + 1;
                    }

                    PurchaseOrdersServices.GetOrderCode(NewData);

                    _ = connection.Insert(NewData);

                    foreach (CompanyPOTransaction item in Items)
                    {
                        item.PurchaseOrderID = NewData.ID;
                    }

                    _ = connection.Insert(Items);
                }

                OrderData.Update(NewData);

                if (OrdersData != null)
                {
                    OrdersData.Add(OrderData);
                }

                OnPropertyChanged(nameof(IsNew));
                OnPropertyChanged(nameof(IsEditing));
            }
            else
            {
                _ = MessageView.Show("Saving", message, MessageViewButton.OK, MessageViewImage.Information);
            }
        }
        private bool CanAccessCreate()
        {
            return IsNew;
        }

        private void Save()
        {
            bool isReady = true;
            string message = "Please Enter:";
            if (NewData.SupplierID == null) { message += $"\n  Supplier."; isReady = false; }
            if (NewData.SupplierAttentionID == null) { message += $"\n  Contact."; isReady = false; }
            if (NewData.QuotationCode == null) { message += $"\n  Quotation Code."; isReady = false; }
            if (NewData.Payment == null) { message += $"\n  Payment."; isReady = false; }
            if (Items.Count == 0) { message += $"\n  Items."; isReady = false; }

            if (isReady)
            {
                string query;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Update(NewData);

                    query = $"Delete From [Purchase].[Transactions] " +
                            $"Where PurchaseOrderID = {NewData.ID}";
                    _ = connection.Execute(query);

                    foreach (var item in Items)
                    {
                        item.PurchaseOrderID = NewData.ID;
                    }

                    _ = connection.Insert(Items);
                }

                OrderData.Update(NewData);

                if (OrdersData != null)
                {
                    OrdersData.Add(OrderData);
                }
            }
            else
            {
                _ = MessageView.Show("Saving", message, MessageViewButton.OK, MessageViewImage.Information);
            }
        }
        private bool CanAccessSave()
        {
            if (NewData.ID == 0)
                return false;

            return CanAccessEdit();
        }

        private void Status()
        {
            //PurchaseOrdersServices.PurchaseOrderStatus(OrderData, ViewData);
        }

        private bool CanAccessStatus()
        {
            return false;
        }
        private void Print()
        {
            PurchaseOrdersServices.Print(NewData, ViewData);
        }
        private bool CanPrint()
        {
            if (NewData.ID == 0)
                return false;

            return true;
        }

        public void UpdateNetPrice()
        {
            NewData.NetPrice = Items.Sum(i => i.TotalCost);
            NewData.VATValue = NewData.NetPrice * NewData.VAT / 100;
            NewData.GrossPrice = NewData.NetPrice * (1 + NewData.VAT / 100);
        }
    }
}
