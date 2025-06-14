using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.JobOrders;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    internal class JobOrderInvoiceByPriceViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private InvoiceItem _SelectedItem;
        private ObservableCollection<InvoiceItem> _Items;
        private ICollectionView _ItemsCollection;

        public JobOrderInvoiceByPriceViewModel(Invoice invoice, ObservableCollection<Invoice> invoices, IView view)
        {
            InvoiceData = invoice;
            InvoicesData = invoices;
            NewData.Update(InvoiceData);

            ViewData = view;

            GetData();

            AddItemCommand = new RelayCommand(AddItem, CanAddItem);
            DeleteItemCommand = new RelayCommand<InvoiceItem>(DeleteItem, CanDeleteItem);

            CreateCommand = new RelayCommand(Create, CanAccessCreate);
            PrintCommand = new RelayCommand(Print, CanAccessPrint);
        }

        public Invoice InvoiceData { get; }
        public ObservableCollection<Invoice> InvoicesData { get; }
        public Invoice NewData { get; } = new Invoice();
        public ObservableCollection<PanelPrices> PanelsData { get; set; }

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

        public bool IsNew => NewData.Code == "-New Invoice-";
        public bool IsEditing => IsNew;

        public RelayCommand AddItemCommand { get; }
        public RelayCommand<InvoiceItem> EditItemCommand { get; }
        public RelayCommand<InvoiceItem> DeleteItemCommand { get; }

        public RelayCommand CreateCommand { get; }
        public RelayCommand PrintCommand { get; }

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Finance].[PanelsPrices(View)] " +
                    $"Where JobOrderId = {NewData.JobOrderId} " +
                    $"Order By SN";
            PanelsData = new ObservableCollection<PanelPrices>(connection.Query<PanelPrices>(query));

            if (NewData.Code == "-New Invoice-")
            {
                Items = new ObservableCollection<InvoiceItem>();
            }
            else
            {
                query = $"Select * From [Finance].[CustomersInvoicesItems] " +
                        $"Where InvoiceId  = {NewData.Id} ";

                Items = new ObservableCollection<InvoiceItem>(connection.Query<InvoiceItem>(query));
            }
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.SortDescriptions.Add(new SortDescription("SN", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
            UpdatePrices();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }


        private void AddItem()
        {
            Navigation.OpenPopup(new JobOrderPanelsByPriceView(Items, PanelsData), PlacementMode.Center, true);
        }
        private bool CanAddItem()
        {
            return IsEditing;
        }

        private void DeleteItem(InvoiceItem item)
        {
            MessageBoxResult result =
                MessageView.Show($"Delete",
                                 $"Are you sure want to Delete\n{item.Description}?",
                                 MessageViewButton.YesNo,
                                 MessageViewImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Items.Remove(item);

                PanelPrices panelData = PanelsData.FirstOrDefault(i => i.PanelId == item.PanelId);
                panelData.Invoiced -= item.GrossPrice;
            }
        }
        private bool CanDeleteItem(InvoiceItem item)
        {
            if (item == null)
                return false;

            return IsEditing;
        }

        private void Create()
        {
            bool isReady = true;
            string message = "Please Add:";
            if (Items.Count == 0) { message += $" Panels."; isReady = false; }

            if (isReady)
            {
                int invoiceNumber;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Select IsNUll(MAX(Number),0) As Number " +
                                   $"From [Finance].[CustomersInvoices] " +
                                   $"Where Year = {DateTime.Now.Year} " +
                                   $"And Type = 'Panels'";

                    invoiceNumber = connection.QueryFirstOrDefault<int>(query) + 1;
                }

                NewData.Number = invoiceNumber;
                NewData.Month = DateTime.Now.Month;
                NewData.Year = DateTime.Now.Year;
                NewData.Code = $"{NewData.Year}{NewData.Month:00}{NewData.Number:000}";
                NewData.Date = DateTime.Now;
                NewData.Type = "Panels";
                NewData.IsPercentageInvoice = true;
                NewData.Items = (int)Items.Sum(x => x.Qty);

                ObservableCollection<TransactionPanel> panels = new();
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(NewData);

                    foreach (InvoiceItem item in Items)
                    {
                        item.InvoiceId = NewData.Id;
                        item.Code = NewData.JobOrderCode + "-" + item.SN;
                        item.VAT = NewData.VAT;

                        string query = $"Select * From [JobOrder].[PanelsTransactions] " +
                                       $"Where PanelID = {item.PanelId}";
                        var checkPanel = connection.QueryFirstOrDefault<TransactionPanel>(query);

                        if (checkPanel == null)
                        {
                            TransactionPanel transaction = new()
                            {
                                JobOrderID = NewData.JobOrderId.Value,
                                PanelID = item.PanelId.Value,
                                Action = "Invoiced",
                                Date = NewData.Date,
                                Reference = NewData.Code,
                                Qty = (int)item.Qty,
                            };

                            panels.Add(transaction);
                        }
                    }

                    _ = connection.Insert(Items);
                    _ = connection.Insert(panels);
                }

                InvoiceData.Update(NewData);

                if (InvoicesData != null)
                {
                    InvoicesData.Add(InvoiceData);
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


        public void UpdatePrices()
        {
            NewData.NetPrice = Items.Sum(i => i.NetPrice);
            NewData.VATValue = Items.Sum(i => i.VATValue);
            NewData.GrossPrice = Items.Sum(i => i.GrossPrice);
        }

        private void Print()
        {
            Services.JobOrdersInvoicesServices.PrintInvoice(NewData.Code, ViewData);
        }
        private bool CanAccessPrint()
        {
            return !IsNew;
        }
    }
}