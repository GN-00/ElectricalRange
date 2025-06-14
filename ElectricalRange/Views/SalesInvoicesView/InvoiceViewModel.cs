using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Customers;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.SalesInvoicesView
{
    public class InvoiceViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private InvoiceItem _SelectedItem;
        private ObservableCollection<InvoiceItem> _Items;
        private ICollectionView _ItemsCollection;

        private Customer _SelectedCustomer;
        private Contact _SelectedContact;
        private ObservableCollection<Customer> _CustomersData;
        private ObservableCollection<Contact> _ContactsData;
        private ObservableCollection<string> _AddressesData;

        public InvoiceViewModel(Invoice invoice, ObservableCollection<Invoice> invoices, ObservableCollection<InvoiceItem> items, IView view)
        {
            InvoiceData = invoice;
            InvoicesData = invoices;
            Items = items;
            NewData.Update(invoice);

            GetData();

            AddItemCommand = new RelayCommand(AddItem, CanAddItem);
            EditItemCommand = new RelayCommand<InvoiceItem>(EditItem, CanEditItem);
            DeleteItemCommand = new RelayCommand<InvoiceItem>(DeleteItem, CanDeleteItem);

            CreateCommand = new RelayCommand(Create, CanAccessCreate);
            PrintCommand = new RelayCommand(Print, CanAccessPrint);

            AttachCommand = new RelayCommand<Invoice>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<Invoice>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<Invoice>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<Invoice>(ReadAttachment, CanAccessReadAttachment);
        }

        public User UserData => Navigation.UserData;
        public Invoice InvoiceData { get; }
        public Invoice NewData { get; } = new Invoice();
        private ObservableCollection<Invoice> InvoicesData { get; }

        public ObservableCollection<Customer> CustomersData
        {
            get => _CustomersData;
            set => SetValue(ref _CustomersData, value);
        }
        public ObservableCollection<Contact> ContactsData
        {
            get => _ContactsData;
            set => SetValue(ref _ContactsData, value);
        }
        public ObservableCollection<string> AddressesData
        {
            get => _AddressesData;
            set => SetValue(ref _AddressesData, value);
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

        public Customer SelectedCustomer
        {
            get => _SelectedCustomer;
            set
            {
                if (SetValue(ref _SelectedCustomer, value))
                {
                    if (SelectedCustomer != null)
                    {
                        NewData.CustomerName = SelectedCustomer.CustomerName;
                        GetContacts();
                    }
                    else
                    {
                        NewData.CustomerId = null;
                        NewData.CustomerName = null;
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
                        NewData.Contact = SelectedContact.ContactName;
                    }
                    else
                    {
                        NewData.ContactId = null;
                        NewData.Contact = null;
                    }
                }
            }
        }
        public InvoiceItem SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<InvoiceItem> Items
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

        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public bool IsNew => InvoiceData.Id == 0;
        public bool IsEditing => InvoiceData.Id == 0;

        public RelayCommand AddItemCommand { get; }
        public RelayCommand<InvoiceItem> EditItemCommand { get; }
        public RelayCommand<InvoiceItem> DeleteItemCommand { get; }

        public RelayCommand CreateCommand { get; }
        public RelayCommand PostCommand { get; }
        public RelayCommand PrintCommand { get; }


        public RelayCommand<Invoice> AttachCommand { get; }
        public RelayCommand<Invoice> DeleteAttachmentCommand { get; }
        public RelayCommand<Invoice> DownloadAttachmentCommand { get; }
        public RelayCommand<Invoice> ReadAttachmentCommand { get; }

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = "Select * From [Customer].[Customers(View)] " +
                    "Order By CustomerName";
            CustomersData = new ObservableCollection<Customer>(connection.Query<Customer>(query));
        }
        private void GetContacts()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Customer].[Contacts] " +
                    $"Where CustomerID = {SelectedCustomer.CustomerID} " +
                    $"Order By ContactName";
            ContactsData = new ObservableCollection<Contact>(connection.Query<Contact>(query));

            query = "Select Address From [Finance].[CustomersInvoices(View)] " +
                    $"Where CustomerId = {SelectedCustomer.CustomerID} " +
                    $"Order By Address ";
            AddressesData = new ObservableCollection<string>(connection.Query<string>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);
            ItemsCollection.Filter = new Predicate<object>(DataFilter);
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
            InvoiceItem item = new()
            {
                InvoiceId = InvoiceData.Id,
                VAT = InvoiceData.VAT
            };

            Navigation.OpenPopup(new ItemView(item, Items), PlacementMode.Center, true);
        }
        private bool CanAddItem()
        {
            if (!IsNew)
                return false;

            return true;
        }

        private void EditItem(InvoiceItem item)
        {
            Navigation.OpenPopup(new ItemView(item, Items), PlacementMode.Center, true);
        }
        private bool CanEditItem(InvoiceItem item)
        {
            if (item == null)
                return false;

            if (!IsNew)
                return false;

            return true;
        }

        private void DeleteItem(InvoiceItem item)
        {
            MessageBoxResult result =
                MessageWindow.Show($"Delete",
                                   $"Are you sure want to Delete\n{item.Code}?",
                                   MessageWindowButton.YesNo,
                                   MessageWindowImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Items.Remove(item);
            }
        }
        private bool CanDeleteItem(InvoiceItem item)
        {
            return CanEditItem(item);
        }

        private void Create()
        {
            Navigation.OpenLoading(Visibility.Visible, "Saving...");

            bool isReady = true;
            string message = "Please Enter:";
            if (NewData.CustomerId == null) { message += $"\n  *Customer."; isReady = false; }
            if (NewData.ContactId == null) { message += $"\n  *Contact."; isReady = false; }
            if (Items.Where(x => x.Id == 0).Count() == 0) { message += $"\n  *Items."; isReady = false; }

            if (isReady)
            {
                SalesInvoicesServices.GetCode(NewData);

                string query;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    foreach (InvoiceItem item in Items)
                    {
                        if (item.Id != 0)
                            continue;

                        query = $"Select Qty from [Store].[JobOrderStock(AvgCost)] " +
                                $"Where JobOrderID = 0 And Code = '{item.Code}' " +
                                $"Order By Code";
                        double checkQty = connection.QueryFirstOrDefault<double>(query);

                        if (item.Qty > checkQty)
                        {
                            MessageView.Show($"Error",
                                             $"The stockis not enought to complete this task!\n{item.Code}(Need:{item.Qty}, Stock:{checkQty}.)",
                                             MessageViewButton.OK,
                                             MessageViewImage.Warning);

                            Navigation.CloseLoading();
                            return;
                        }
                    }


                    SalesInvoicesServices.GetCode(NewData);
                    _ = connection.Insert(NewData);

                    foreach (InvoiceItem item in Items)
                    {
                        if (item.Id != 0)
                            continue;

                        item.InvoiceId = NewData.Id;
                        item.SN = Items.IndexOf(item) + 1;
                        _ = connection.Insert(item);

                        IEnumerable<Data.Store.ItemTransaction> stockItems;
                        query = $"Select * From [Store].[TransactionsView] " +
                                $"Where JobOrderID = 0 " +
                                $"And Code = '{item.Code}' " +
                                $"Order By Cost ";
                        stockItems = connection.Query<Data.Store.ItemTransaction>(query);

                        decimal qtyNeeded = (decimal)item.Qty;
                        foreach (Data.Store.ItemTransaction transaction in stockItems)
                        {
                            if (qtyNeeded == 0)
                                break;

                            if (transaction.FinalQty >= qtyNeeded)
                            {
                                transaction.Reference = transaction.ID;
                                transaction.SalesItemId = NewData.Id;
                                transaction.Qty = qtyNeeded;
                                transaction.Type = "Used";

                                _ = connection.Insert(transaction);

                                qtyNeeded = 0;
                            }
                            else
                            {
                                transaction.Reference = transaction.ID;
                                transaction.Qty = transaction.FinalQty;
                                transaction.Type = "Used";
                                _ = connection.Insert(transaction);

                                qtyNeeded -= transaction.FinalQty;
                            }
                        }
                    }
                }

                InvoiceData.Update(NewData);

                if (InvoicesData != null)
                {
                    InvoicesData.Add(InvoiceData);
                }

                OnPropertyChanged(nameof(IsNew));

                Navigation.CloseLoading();
            }
            else
            {
                _ = MessageWindow.Show("Saving", message, MessageWindowButton.OK, MessageWindowImage.Information);
                Navigation.CloseLoading();
            }
        }
        private bool CanAccessCreate()
        {
            return IsNew;
        }


        public void UpdateNetPrice()
        {
            if (NewData.Id == 0)
            {
                NewData.NetPrice = Items.Where(x => x.Id == 0).Sum(i => i.NetPrice);
                NewData.VATValue = Items.Where(x => x.Id == 0).Sum(i => i.VATValue);
                NewData.GrossPrice = Items.Where(x => x.Id == 0).Sum(i => i.GrossPrice);
            }
            else
            {
                NewData.NetPrice = Items.Where(x => x.Id == NewData.Id).Sum(i => i.NetPrice);
                NewData.VATValue = Items.Where(x => x.Id == NewData.Id).Sum(i => i.VATValue);
                NewData.GrossPrice = Items.Where(x => x.Id == NewData.Id).Sum(i => i.GrossPrice);
            }
        }


        private void Print()
        {
            SalesInvoicesServices.PrintInvoice(InvoiceData.Code, ViewData);
        }
        private bool CanAccessPrint()
        {
            if (IsNew)
                return false;

            return true;
        }

        #region Data Filter
        private bool DataFilter(object item)
        {
            if (InvoiceData == null)
                return false;

            bool result = true;
            string value = $"{item.GetType().GetProperty("InvoiceId").GetValue(item)}".ToUpper();
            string checkValue = InvoiceData.GetType().GetProperty("Id").GetValue(InvoiceData).ToString();

            if (!value.Contains(checkValue))
            {
                result = false;
            }

            return result;
        }
        #endregion

        private void Attach(Invoice invoice)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (invoice.AttachmentId == null)
            {
                InvoiceAttachment attachment = new()
                {
                    InvoiceId = invoice.Id,
                };

                Attachment.SaveFile<InvoiceAttachment>(attachment);

                InvoiceData.AttachmentId = invoice.AttachmentId = attachment.Id;
            }
            else
            {
                InvoiceAttachment attachment = new()
                {
                    Id = invoice.AttachmentId.GetValueOrDefault(),
                };

                Attachment.UpdateFile<InvoiceAttachment>(attachment);
            }

            IsLoading = false;
        }
        private bool CanAccessAttach(Invoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.Id == 0)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeleteAttachment(Invoice invoice)
        {
            InvoiceAttachment attachment = new()
            {
                Id = invoice.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<InvoiceAttachment>(attachment);

            InvoiceData.AttachmentId = invoice.AttachmentId = null;
        }
        private bool CanAccessDeleteAttachment(Invoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.AttachmentId == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DownloadAttachment(Invoice invoice)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            InvoiceAttachment attachment = new()
            {
                Id = invoice.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DownloadFile<InvoiceAttachment>(attachment);

            IsLoading = false;
        }
        private bool CanAccessDownloadAttachment(Invoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(Invoice invoice)
        {
            InvoiceAttachment attachment = new()
            {
                Id = invoice.AttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<InvoiceAttachment>(attachment);
        }
        private bool CanAccessReadAttachment(Invoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.AttachmentId == null)
                return false;

            return true;
        }
    }
}
