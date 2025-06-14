using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows.Data;

namespace ProjectsNow.Views.SalesInvoicesView
{
    public class InvoicesViewModel : ViewModelBase
    {
        private string _InvoicesIndicator = "-";
        private string _ItemsIndicator = "-";

        private int _SelectedInvoiceIndex;
        private Invoice _SelectedInvoice;
        private ObservableCollection<Invoice> _Invoices;

        private int _SelectedItemIndex;
        private InvoiceItem _SelectedItem;
        private ObservableCollection<InvoiceItem> _Items;

        private ICollectionView _InvoicesCollection;
        private ICollectionView _ItemsCollection;

        public InvoicesViewModel(IView view)
        {
            ViewData = view;

            GetData();

            AddInvoiceCommand = new RelayCommand(AddInvoice, CanAddInvoice);
            InformationCommand = new RelayCommand<Invoice>(Information, CanAccessInformation);
            PrintCommand = new RelayCommand<Invoice>(Print, CanAccessPrint);

            AttachCommand = new RelayCommand<Invoice>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<Invoice>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<Invoice>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<Invoice>(ReadAttachment, CanAccessReadAttachment);
        }

        public User UserData => Navigation.UserData;
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

        public Invoice SelectedInvoice
        {
            get => _SelectedInvoice;
            set
            {
                if (SetValue(ref _SelectedInvoice, value))
                {
                    ItemsCollection.Refresh();
                }
            }
        }
        public InvoiceItem SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }

        public ObservableCollection<Invoice> Invoices
        {
            get => _Invoices;
            private set
            {
                if (SetValue(ref _Invoices, value))
                {
                    CreateInvoicesCollectionView();
                }
            }
        }

        public ObservableCollection<InvoiceItem> Items
        {
            get => _Items;
            private set
            {
                if (SetValue(ref _Items, value))
                {
                    CreateItemsCollectionView();
                }
            }
        }

        public ICollectionView InvoicesCollection
        {
            get => _InvoicesCollection;
            set => SetValue(ref _InvoicesCollection, value);
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand AddInvoiceCommand { get; }
        public RelayCommand<Invoice> InformationCommand { get; }
        public RelayCommand<Invoice> PrintCommand { get; }

        public RelayCommand<Invoice> AttachCommand { get; }
        public RelayCommand<Invoice> DeleteAttachmentCommand { get; }
        public RelayCommand<Invoice> DownloadAttachmentCommand { get; }
        public RelayCommand<Invoice> ReadAttachmentCommand { get; }

        private void GetData(int? year = null)
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Finance].[CustomersInvoices(View)] " +
                    $"Where Type = 'Items' " +
                    $"Order By Date Desc";
            Invoices = new ObservableCollection<Invoice>(connection.Query<Invoice>(query));

            query = $"Select * From [Finance].[CustomersInvoicesItems] " +
                    $"Where InvoiceId In " +
                    $"(Select Id From [Finance].[CustomersInvoices(View)] Where Type = 'Items') " +
                    $"Order By Code ";
            Items = new ObservableCollection<InvoiceItem>(connection.Query<InvoiceItem>(query));
        }
        private void CreateInvoicesCollectionView()
        {
            InvoicesCollection = CollectionViewSource.GetDefaultView(Invoices);

            InvoicesCollection.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            InvoicesCollection.GroupDescriptions.Add(new PropertyGroupDescription("Year"));
            InvoicesCollection.CollectionChanged += InvoicesCollectionChanged;
        }

        private void CreateItemsCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);
            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += ItemsCollectionChanged;
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
            InvoicesIndicator = DataGridIndicator.Get(SelectedInvoiceIndex, InvoicesCollection);
        }
        private void UpdateItemsIndicator()
        {
            ItemsIndicator = DataGridIndicator.Get(SelectedItemIndex, ItemsCollection);
        }

        #region Data Filter
        private bool DataFilter(object item)
        {
            if (SelectedInvoice == null)
                return false;

            bool result = true;
            string value = $"{item.GetType().GetProperty("InvoiceId").GetValue(item)}".ToUpper();
            string checkValue = SelectedInvoice.GetType().GetProperty("Id").GetValue(SelectedInvoice).ToString();

            if (!value.Contains(checkValue))
            {
                result = false;
            }

            return result;
        }
        #endregion

        private void AddInvoice()
        {
            Invoice invoice = new()
            {
                Code = "-New Invoice-",
                VAT = (double)Data.Application.AppData.VAT / 100,
                VATPercentage = (double)Data.Application.AppData.VAT,
                Type = "Items",
            };

            Navigation.To(new InvoiceView(invoice, Invoices, Items), ViewData);
        }
        private bool CanAddInvoice()
        {
            if (!UserData.ModifySalesInvoices)
                return false;

            return true;
        }

        private void Information(Invoice invoice)
        {
            Navigation.To(new InvoiceView(invoice, Invoices, Items), ViewData);
        }
        private bool CanAccessInformation(Invoice invoice)
        {
            if (invoice == null)
                return false;

            return true;
        }

        private void Print(Invoice invoice)
        {
            SalesInvoicesServices.PrintInvoice(invoice.Code, ViewData);
        }
        private bool CanAccessPrint(Invoice invoice)
        {
            if (invoice == null)
                return false;

            return true;
        }

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

                invoice.AttachmentId = attachment.Id;
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

            invoice.AttachmentId = null;
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