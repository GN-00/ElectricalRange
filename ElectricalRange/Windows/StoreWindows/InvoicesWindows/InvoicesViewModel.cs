using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Store;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Windows.MessageWindows;
using ProjectsNow.Windows.StoreWindows.ReturnToSuppliersWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows
{
    internal class InvoicesViewModel : ViewModelBase
    {
        private Visibility _IsUpdating = Visibility.Collapsed;

        private decimal _NetPrice;
        private decimal _GrossPrice;
        private decimal _VATValue;
        private decimal _VATPercentage;

        private string _InvoicesIndicator = "-";
        private string _ItemsIndicator = "-";

        private int _SelectedInvoiceIndex;
        private int _SelectedItemIndex;

        private JobOrder _JobOrderData;
        private User _UserData;
        private SupplierInvoice _SelectedInvoice;
        private ItemTransaction _SelectedItem;

        private ObservableCollection<ItemTransaction> _Items;
        private ObservableCollection<SupplierInvoice> _Invoices;

        private ICollectionView _ItemsView;
        private ICollectionView _InvoicesView;

        public InvoicesViewModel(JobOrder jobOrder, User user)
        {
            JobOrderData = jobOrder;
            UserData = user;

            GetData();

            NewInvoicesCommand = new RelayCommand(NewInvoice);
            InternalInvoiceCommand = new RelayCommand(InternalInvoice);
            DeleteInternalInvoiceCommand = new RelayCommand<SupplierInvoice>(DeleteInternalInvoice, CanDeleteInternalInvoice);
            EditCommand = new RelayCommand<SupplierInvoice>(Edit, CanEdit);

            PurchaseOrdersCommand = new RelayCommand<SupplierInvoice>(PurchaseOrders, CanAccessPurchaseOrders);
            ReturnItemsCommand = new RelayCommand<SupplierInvoice>(ReturnItems, CanAccessReturnItems);
            ReturnInvoicesCommand = new RelayCommand<SupplierInvoice>(ReturnInvoices, CanAccessReturnInvoices);
            PrintReturnCommand = new RelayCommand<ItemTransaction>(PrintReturn, CanAccessPrintReturn);
            PrintCommand = new RelayCommand<SupplierInvoice>(Print, CanAccessPrint);
            AttachCommand = new RelayCommand<SupplierInvoice>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<SupplierInvoice>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<SupplierInvoice>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<SupplierInvoice>(ReadAttachment, CanAccessReadAttachment);

            NewItemCommand = new RelayCommand(NewItem, CanAccessNewItem);
            EditItemCommand = new RelayCommand<ItemTransaction>(EditItem, CanAccessEditItem);
            DeleteItemCommand = new RelayCommand<ItemTransaction>(DeleteItem, CanAccessDeleteItem);
        }

        public Visibility IsUpdating
        {
            get => _IsUpdating;
            set => SetValue(ref _IsUpdating, value);
        }
        public decimal NetPrice
        {
            get => _NetPrice;
            set => SetValue(ref _NetPrice, value);
        }
        public decimal GrossPrice
        {
            get => _GrossPrice;
            set => SetValue(ref _GrossPrice, value);
        }
        public decimal VATValue
        {
            get => _VATValue;
            set => SetValue(ref _VATValue, value);
        }
        public decimal VATPercentage
        {
            get => _VATPercentage;
            set => SetValue(ref _VATPercentage, value);
        }

        public string InvoicesIndicator
        {
            get => _InvoicesIndicator;
            set => SetValue(ref _InvoicesIndicator, value);
        }
        public string ItemsIndicator
        {
            get => _ItemsIndicator;
            set => SetValue(ref _ItemsIndicator, value);
        }
        public int SelectedInvoiceIndex
        {
            get => _SelectedInvoiceIndex;
            set
            {
                if (SetValue(ref _SelectedInvoiceIndex, value))
                {
                    UpdateInvoicesIndicator();
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
        public JobOrder JobOrderData
        {
            get => _JobOrderData;
            set => SetValue(ref _JobOrderData, value);
        }
        public User UserData
        {
            get => _UserData;
            set => SetValue(ref _UserData, value);
        }
        public SupplierInvoice SelectedInvoice
        {
            get => _SelectedInvoice;
            set
            {
                if (SetValue(ref _SelectedInvoice, value))
                {
                    ItemsView.Refresh();

                    if (_SelectedInvoice != null)
                    {
                        UpdatePrices();
                    }
                }
            }
        }
        public ItemTransaction SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<SupplierInvoice> Invoices
        {
            get => _Invoices;
            private set => SetValue(ref _Invoices, value);
        }
        public ObservableCollection<ItemTransaction> Items
        {
            get => _Items;
            private set => SetValue(ref _Items, value);
        }

        public ICollectionView InvoicesView
        {
            get => _InvoicesView;
            set => SetValue(ref _InvoicesView, value);
        }
        public ICollectionView ItemsView
        {
            get => _ItemsView;
            set => SetValue(ref _ItemsView, value);
        }

        public RelayCommand NewInvoicesCommand { get; }
        public RelayCommand InternalInvoiceCommand { get; }
        public RelayCommand<SupplierInvoice> DeleteInternalInvoiceCommand { get; }
        public RelayCommand<SupplierInvoice> EditCommand { get; }
        public RelayCommand<SupplierInvoice> PurchaseOrdersCommand { get; }
        public RelayCommand<SupplierInvoice> ReturnItemsCommand { get; }
        public RelayCommand<SupplierInvoice> ReturnInvoicesCommand { get; }
        public RelayCommand<SupplierInvoice> PrintCommand { get; }
        public RelayCommand<ItemTransaction> PrintReturnCommand { get; }
        public RelayCommand<SupplierInvoice> AttachCommand { get; }
        public RelayCommand<SupplierInvoice> DeleteAttachmentCommand { get; }
        public RelayCommand<SupplierInvoice> DownloadAttachmentCommand { get; }
        public RelayCommand<SupplierInvoice> ReadAttachmentCommand { get; }
        public RelayCommand NewItemCommand { get; }
        public RelayCommand<ItemTransaction> EditItemCommand { get; }
        public RelayCommand<ItemTransaction> DeleteItemCommand { get; }


        #region Data Filter
        private bool DataFilter(object item)
        {
            if (SelectedInvoice == null)
                return false;

            bool result = false;
            string columnName = "InvoiceID";

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = SelectedInvoice.ID.ToString();

            if (value == checkValue)
            {
                result = true;
            }

            return result;
        }

        #endregion

        private void GetData()
        {
            string query;
            using (SqlConnection connection = new(Data.Database.ConnectionString))
            {
                query = $"Select * From [Store].[SuppliersInvoices] " +
                        $"Where JobOrderID  = {JobOrderData.ID}" +
                        $"Order By Date Desc";
                Invoices = new ObservableCollection<SupplierInvoice>(connection.Query<SupplierInvoice>(query));

                query = $"Select * From [Store].[JobOrdersInvoicesItems] " +
                        $"Where JobOrderID  = {JobOrderData.ID} " +
                        $"Order By Code";
                Items = new ObservableCollection<ItemTransaction>(connection.Query<ItemTransaction>(query));
            }

            CreateCollectionView();
        }
        private void CreateCollectionView()
        {
            InvoicesView = CollectionViewSource.GetDefaultView(Invoices);
            InvoicesView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            InvoicesView.GroupDescriptions.Add(new PropertyGroupDescription("PurchaseOrder"));
            InvoicesView.CollectionChanged += InvoicesCollectionChanged;

            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = new Predicate<object>(DataFilter);
            ItemsView.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            ItemsView.CollectionChanged += ItemsCollectionChanged;
        }

        private void InvoicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateInvoicesIndicator();
        }
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateItemsIndicator();
        }

        private void UpdateInvoicesIndicator()
        {
            InvoicesIndicator = DataGridIndicator.Get(SelectedInvoiceIndex, InvoicesView);
        }
        private void UpdateItemsIndicator()
        {
            ItemsIndicator = DataGridIndicator.Get(SelectedItemIndex, ItemsView);
        }

        private void NewInvoice()
        {
            SupplierInvoice supplierInvoice = new()
            {
                JobOrderID = JobOrderData.ID,
                Date = DateTime.Now,
            };
            InvoiceWindow invoicesWindow = new()
            {
                ActionData = Actions.New,
                SupplierInvoiceData = supplierInvoice,
                SupplierInvoicesData = Invoices,
            };
            _ = invoicesWindow.ShowDialog();
        }

        private void Edit(SupplierInvoice invoice)
        {
            InvoiceWindow invoicesWindow = new()
            {
                ActionData = Actions.Edit,
                SupplierInvoiceData = invoice
            };
            _ = invoicesWindow.ShowDialog();
        }
        private bool CanEdit(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.InternalTransferNumber != null)
                return false;

            if (invoice.Date.Date != DateTime.Today.Date)
                return false;

            return true;
        }

        private void PurchaseOrders(SupplierInvoice invoice)
        {
            if (invoice.PurchaseOrderID == 0)
            {
                PurchaseOrdersItemsWindow purchaseOrdersItemsWindow = new()
                {
                    JobOrderData = JobOrderData,
                    InvoiceData = invoice,
                    ItemsData = Items,
                };
                _ = purchaseOrdersItemsWindow.ShowDialog();
            }
            else
            {
                PurshaseOrderItems.ItemsWindow itemsWindow =
                         new(invoice);
                itemsWindow.ShowDialog();
            }

            UpdateInvoiceItems(invoice);
            UpdatePrices();
        }
        private bool CanAccessPurchaseOrders(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.InternalTransferNumber != null)
                return false;

            if (Items.Where(i => i.InvoiceID == invoice.ID).Count() != 0)
                return false;

            return true;
        }

        private void ReturnItems(SupplierInvoice invoice)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select ReturnPeriod From [Store].[SuppliersInvoices] " +
                               $"Where ID = {invoice.ID} ";
                invoice.ReturnPeriod = connection.QueryFirstOrDefault<int>(query);
            }

            if (invoice.CanReturn)
            {
                ReturnToSuppliersWindows.ReturnToSupplierWindow returnToSupplierWindow =
                                    new(invoice);

                returnToSupplierWindow.ShowDialog();

                UpdateInvoiceItems(invoice);
                UpdatePrices();
            }
            else
            {
                MessageWindow.Show($"Return",
                                   $"Can't return!\nReturn period {invoice.ReturnPeriod} form {invoice.Date:dd/MM/yyyy}",
                                   MessageWindowButton.OK,
                                   MessageWindowImage.Information);
            }
        }
        private bool CanAccessReturnItems(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.InternalTransferNumber != null)
                return false;

            return true;
        }

        private void ReturnInvoices(SupplierInvoice invoice)
        {
            ReturnInvoicesWindow returnInvoicesWindow =
                new(invoice);

            returnInvoicesWindow.ShowDialog();
        }
        private bool CanAccessReturnInvoices(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            if (Items.Any(i => i.ReturnInvoiceID != null && i.InvoiceID == invoice.ID))
                return true;

            return false;
        }


        private void Print(SupplierInvoice item)
        {
            //Printing.PrintingHelper.PrintReturnInvoice(item.ReturnInvoiceID.Value);
        }
        private bool CanAccessPrint(SupplierInvoice item)
        {
            if (item == null)
                return false;

            //if (item.ReturnInvoiceID == null)
            //    return false;

            return true;
        }


        private void PrintReturn(ItemTransaction item)
        {
            Printing.PrintingHelper.PrintReturnInvoice(item.ReturnInvoiceID.Value);
        }
        private bool CanAccessPrintReturn(ItemTransaction item)
        {
            if (item == null)
                return false;

            if (item.ReturnInvoiceID == null)
                return false;

            return true;
        }

        private void UpdatePrices()
        {
            var list = Items.Where(i => i.InvoiceID == SelectedInvoice.ID && i.Type == "Stock");
            if (list.Count() > 0)
            {
                NetPrice = list.Sum(i => i.TotalCost);
                GrossPrice = list.Sum(i => i.TotalCost * (1 + i.VAT / 100));
                VATPercentage = list.Max(i => i.VAT);
                VATValue = GrossPrice - NetPrice;
            }
            else
            {
                NetPrice = 0;
                GrossPrice = 0;
                VATPercentage = 0;
                VATValue = 0;
            }
        }
        private void UpdateInvoiceItems(SupplierInvoice invoice)
        {
            ObservableCollection<ItemTransaction> oldItems = new(Items.Where(i => i.InvoiceID == invoice.ID));
            foreach (ItemTransaction item in oldItems)
            {
                Items.Remove(item);
            }

            IEnumerable<ItemTransaction> items;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Store].[JobOrdersInvoicesItems] " +
                               $"Where InvoiceID = {invoice.ID} " +
                               $"Order By Code";

                items = connection.Query<ItemTransaction>(query);
            }

            foreach (ItemTransaction item in items)
            {
                Items.Add(item);
            }
        }

        private void Attach(SupplierInvoice invoice)
        {
            IsUpdating = Visibility.Visible;
            Events.ShowEvent.Do();

            if (invoice.AttachmentID == null)
            {
                InvoiceAttachment attachment = new()
                {
                    InvoiceId = invoice.ID,
                };

                Attachment.SaveFile<InvoiceAttachment>(attachment);

                invoice.AttachmentID = attachment.Id;
                using SqlConnection connection = new(Database.ConnectionString);
                _ = connection.Update(invoice);
            }
            else
            {
                InvoiceAttachment attachment = new()
                {
                    Id = invoice.AttachmentID.GetValueOrDefault(),
                    InvoiceId = invoice.ID,
                };

                Attachment.UpdateFile<InvoiceAttachment>(attachment);
            }

            IsUpdating = Visibility.Collapsed;
        }

        private bool CanAccessAttach(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            return true;
        }

        private void DeleteAttachment(SupplierInvoice invoice)
        {
            InvoiceAttachment attachment = new()
            {
                Id = invoice.AttachmentID.GetValueOrDefault(),
                InvoiceId = invoice.ID,
            };

            Attachment.DeleteFile<InvoiceAttachment>(attachment);

            invoice.AttachmentID = null;
            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(invoice);
        }

        private bool CanAccessDeleteAttachment(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.AttachmentID == null)
                return false;

            return true;
        }

        private void DownloadAttachment(SupplierInvoice invoice)
        {
            IsUpdating = Visibility.Visible;
            Events.ShowEvent.Do();

            InvoiceAttachment attachment = new()
            {
                Id = invoice.AttachmentID.GetValueOrDefault(),
                InvoiceId = invoice.ID,
            };

            Attachment.DownloadFile<InvoiceAttachment>(attachment);

            IsUpdating = Visibility.Collapsed;
        }

        private bool CanAccessDownloadAttachment(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.AttachmentID == null)
                return false;

            return true;
        }

        private void ReadAttachment(SupplierInvoice invoice)
        {
            InvoiceAttachment attachment = new()
            {
                Id = invoice.AttachmentID.GetValueOrDefault(),
                InvoiceId = invoice.ID,
            };

            Attachment.OpenFile<InvoiceAttachment>(attachment);
        }

        private bool CanAccessReadAttachment(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.AttachmentID == null)
                return false;

            return true;
        }



        private void InternalInvoice()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select Number From [Store].[TransferInvoiceNumber] Where Year  = {DateTime.Now.Year} ";
            int invoiceNumber = connection.QueryFirstOrDefault<int>(query) + 1;
            SupplierInvoice supplierInvoice = new()
            {
                JobOrderID = JobOrderData.ID,
                Date = DateTime.Now,
                SupplierID = 0,
                SupplierCode = null,
                InternalTransferNumber = invoiceNumber,
                Number = $"ER-I{DateTime.Now.Year.ToString().Substring(2, 2)}/{invoiceNumber:0000}"
            };

            _ = connection.Insert(supplierInvoice);

            Invoices.Add(supplierInvoice);
        }

        private void DeleteInternalInvoice(SupplierInvoice invoice)
        {
            MessageBoxResult result = MessageWindow.Show("Delete",
                                                         $"Are you sure want to delete\n{invoice.Number}?",
                                                         MessageWindowButton.YesNo,
                                                         MessageWindowImage.Question);

            if (result == MessageBoxResult.No)
                return;

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Delete(invoice);
            Invoices.Remove(invoice);
        }
        private bool CanDeleteInternalInvoice(SupplierInvoice invoice)
        {
            if (invoice == null)
                return false;

            if (invoice.InternalTransferNumber == null)
                return false;

            if (Items.Any(x => x.InvoiceID == invoice.ID))
                return false;

            return true;
        }

        private void NewItem()
        {
            TransferWindows.TransferWindow transferWindow = new()
            {
                UserData = UserData,
                JobOrderData = JobOrderData,
                InvoiceData = SelectedInvoice,
                ItemsData = Items,
            };

            _ = transferWindow.ShowDialog();
        }

        private bool CanAccessNewItem()
        {
            if (SelectedInvoice == null)
                return false;

            if (SelectedInvoice.InternalTransferNumber == null)
                return false;

            return true;
        }

        private void EditItem(ItemTransaction item)
        {

        }

        private bool CanAccessEditItem(ItemTransaction item)
        {
            if (item == null)
                return false;

            if (SelectedInvoice == null)
                return false;

            if (SelectedInvoice.InternalTransferNumber == null)
                return false;

            return false;
        }

        private void DeleteItem(ItemTransaction item)
        {
            ItemTransaction checkItemUsage;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select Reference From [Store].[Transactions] Where Reference = {item.ID}";
                checkItemUsage = connection.QueryFirstOrDefault<ItemTransaction>(query);

                if (checkItemUsage == null)
                {
                    query = $"Delete From [Store].[Transactions] Where ID = {item.ID}; ";

                    if (item.TransferInvoiceID != null)
                    {
                        query += $"Delete From [Store].[Transactions] Where ID = {item.TransferInvoiceID}; ";
                    }

                    _ = connection.Execute(query);

                    _ = Items.Remove(item);
                }
            }

            if (checkItemUsage != null)
            {
                _ = MessageWindow.Show("Items Usage", "Can't delete this item!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }

        private bool CanAccessDeleteItem(ItemTransaction item)
        {
            if (item == null)
                return false;

            if (SelectedInvoice == null)
                return false;

            if (SelectedInvoice.InternalTransferNumber == null)
                return false;

            return true;
        }


        ///        //private void Print()
        //{
        //    if (InvoicesList.SelectedItem is SupplierInvoice invoiceData)
        //    {
        //        if (invoiceData.SupplierID != 0)
        //        {
        //            _ = MessageWindow.Show("Invoice", "Only internal invoice can be print it!! ", MessageWindowButton.OK, MessageWindowImage.Warning);
        //            return;
        //        }

        //        InvoiceInformation invoiceInformation;
        //        List<Printing.Store.Item> items;
        //        List<IPanel> panels;
        //        List<string> POs;
        //        string IDs = "";
        //        Printing.Store.InternalInvoice invoiceForm;

        //        foreach (ItemTransaction item in viewDataItems.View)
        //        {
        //            IDs += $"{item.ID}, ";
        //        }

        //        IDs = IDs.Substring(0, IDs.Length - 2);


        //        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
        //        {
        //            string query;
        //            query = $"Select * From [Store].[InvoicesInformations] Where ID  = {invoiceData.ID}";
        //            invoiceInformation = connection.QueryFirstOrDefault<InvoiceInformation>(query);

        //            query = $"Select PurchaseOrdersNumber From [JobOrder].[Panels] Where JobOrderID = {invoiceData.JobOrderID}";
        //            panels = connection.Query<IPanel>(query).ToList();

        //            query = $"Select * From [Store].[InvoicesItemsInformations] Where InvoiceID = {invoiceData.ID} And " +
        //                    $"ID in ({IDs})";
        //            items = connection.Query<Printing.Store.Item>(query).ToList();
        //        }
        //        POs = panels.GroupBy(p => p.PurchaseOrdersNumber).Select(p => p.Key).ToList();


        //        for (int i = 1; i <= items.Count; i++)
        //        {
        //            items[i - 1].SN = i;
        //        }

        //        foreach (string po in POs)
        //        {
        //            invoiceInformation.POs += $"{po}, ";
        //        }

        //        invoiceInformation.POs = invoiceInformation.POs.Substring(0, invoiceInformation.POs.Length - 2);

        //        double pagesNumber = items.Count / 8d;
        //        if (pagesNumber - Math.Truncate(pagesNumber) != 0)
        //        {
        //            pagesNumber = Math.Truncate(pagesNumber) + 1;
        //        }

        //        if (pagesNumber != 0)
        //        {
        //            List<FrameworkElement> elements = new List<FrameworkElement>();
        //            for (int i = 1; i <= pagesNumber; i++)
        //            {
        //                if (i == pagesNumber)
        //                {
        //                    invoiceForm = new Printing.Store.InternalInvoice()
        //                    {
        //                        VATPercentage = items.Max(item => item.VAT),
        //                        TotalCost = items.Sum(item => item.TotalCost),
        //                        TotalVAT = items.Sum(item => item.VAT / 100m * item.TotalCost),
        //                        TotalPrice = items.Sum(item => (1 + item.VAT / 100m) * item.TotalCost),
        //                        Page = i,
        //                        Pages = Convert.ToInt32(pagesNumber),
        //                        InvoiceInformationData = invoiceInformation,
        //                        ItemsData = items.Where(item => item.SN > ((i - 1) * 8) && item.SN <= (i * 8)).ToList()
        //                    };
        //                }
        //                else
        //                {
        //                    invoiceForm = new Printing.Store.InternalInvoice()
        //                    {
        //                        Page = i,
        //                        Pages = Convert.ToInt32(pagesNumber),
        //                        InvoiceInformationData = invoiceInformation,
        //                        ItemsData = items.Where(item => item.SN > ((i - 1) * 8) && item.SN <= (i * 8)).ToList()
        //                    };
        //                }

        //                elements.Add(invoiceForm);
        //            }

        //            Printing.Print.PrintPreview(elements, $"Invoice-{invoiceData.Number}");
        //        }
        //        else
        //        {
        //            _ = MessageWindow.Show("Items", "There is no items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
        //        }
        //    }
        //}
    }
}