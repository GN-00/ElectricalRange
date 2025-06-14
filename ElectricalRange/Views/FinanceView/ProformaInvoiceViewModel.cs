using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Services;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

using JPanel = ProjectsNow.Data.JobOrders.JPanel;

namespace ProjectsNow.Views.FinanceView
{
    public class ProformaInvoiceViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ProformaInvoicePanel _SelectedItem;
        private InvoiceType _SelectedType;
        private ObservableCollection<ProformaInvoicePanel> _Items;
        private ObservableCollection<InvoiceType> _Types;
        private ICollectionView _ItemsCollection;

        public ProformaInvoiceViewModel(ProformaInvoice invoice, ObservableCollection<ProformaInvoice> invoices, IView view)
        {
            ViewData = view;
            InvoiceData = invoice;
            InvoicesData = invoices;
            NewData.Update(InvoiceData);

            GetData();

            AddItemCommand = new RelayCommand(AddItem, CanAddItem);
            DeleteItemCommand = new RelayCommand<ProformaInvoicePanel>(DeleteItem, CanDeleteItem);

            CreateCommand = new RelayCommand(Create, CanAccessCreate);
            PrintCommand = new RelayCommand(Print, CanAccessPrint);
        }

        public CustomerAccount CustomerData { get; }
        public ProformaInvoice InvoiceData { get; }
        public ProformaInvoice NewData { get; } = new ProformaInvoice();
        private ObservableCollection<ProformaInvoice> InvoicesData { get; }
        public ObservableCollection<JPanel> PanelsData { get; set; }
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
        public ProformaInvoicePanel SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<ProformaInvoicePanel> Items
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

        public InvoiceType SelectedType
        {
            get => _SelectedType;
            set
            {
                if (SetValue(ref _SelectedType, value))
                {
                    UpdateTypeInfo();
                }
            }
        }
        public ObservableCollection<InvoiceType> Types
        {
            get => _Types;
            set => SetValue(ref _Types, value);
        }

        public bool IsNew => NewData.Code == "-New Invoice-";
        public bool IsEditing => IsNew;
        public bool AmountEditing
        {
            get
            {
                if (!IsEditing)
                    return false;

                if (NewData.Type != "Payment")
                    return false;

                return true;
            }
        }

        public RelayCommand AddItemCommand { get; }
        public RelayCommand<ProformaInvoicePanel> EditItemCommand { get; }
        public RelayCommand<ProformaInvoicePanel> DeleteItemCommand { get; }
        public RelayCommand CreateCommand { get; }
        public RelayCommand PrintCommand { get; }

        public class InvoiceType
        {
            public string Name { get; set; }
        }
        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            PanelsData = JPanelController.GetJobOrderPanels(connection, NewData.JobOrderId);

            if (NewData.Code == "-New Invoice-")
            {
                Items = new ObservableCollection<ProformaInvoicePanel>();
            }
            else
            {
                string query = $"Select * From [Finance].[ProformaInvoicesPanels] " +
                               $"Where InvoiceId  = {NewData.Id} ";
                Items = new ObservableCollection<ProformaInvoicePanel>(connection.Query<ProformaInvoicePanel>(query));
            }

            Types = new ObservableCollection<InvoiceType>()
                {
                    new() { Name = "Invoice" },
                    new() { Name = "Payment" },
                    new() { Name = "Down Payment" },
                    new() { Name = "Balance Payment" },
                };
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


        class OrderData 
        { 
            public double DownPayment { get; set; } 
            public double Percentage { get; set; }
        }
        private void UpdateTypeInfo()
        {
            if (NewData != null)
            {
                NewData.Type = SelectedType.Name;

                if (NewData.Type == "Invoice")
                {
                    if (Items.Count != 0)
                        NewData.Amount = Items.Sum(x => x.GrossPrice);
                    else
                        NewData.Amount = 0;

                    NewData.Description = $"-";
                    NewData.DescriptionArabic = $"-";
                }
                else if (NewData.Type == "Payment")
                {
                    NewData.Percentage = null;
                    NewData.Description = $"Payment";
                    NewData.DescriptionArabic = $"دفعة";
                }
                else if (NewData.Type == "Down Payment")
                {
                    string query = $"Select * From [Finance].[DownPayments(View)] " +
                                   $"Where JobOrderId = {NewData.JobOrderId}";
                    using SqlConnection connection = new(Database.ConnectionString);
                    OrderData data = connection.QueryFirstOrDefault<OrderData>(query);
                    NewData.Amount = data.DownPayment;
                    NewData.Percentage = data.Percentage;
                    NewData.Description = $"Down Payment {data.Percentage}% ";
                    NewData.DescriptionArabic = $"دفعة أولى {NewData.PercentageArabic}%";
                }
                else if (NewData.Type == "Balance Payment")
                {
                    string query = $"Select * From [Finance].[JobOrders(View)] " +
                                   $"Where ID = {NewData.JobOrderId}";
                    using SqlConnection connection = new(Database.ConnectionString);
                    JobOrder data = connection.QueryFirstOrDefault<JobOrder>(query);
                    NewData.Amount = (double)data.Balance;
                    NewData.Percentage = null;
                    NewData.Description = $"Balance Payment";
                    NewData.DescriptionArabic = $"دفعة متبقية";
                }

                OnPropertyChanged(nameof(AmountEditing));
            }
        }
        private void AddItem()
        {
            Navigation.OpenPopup(new SelectProformaInvoicePanelsView(Items, PanelsData), PlacementMode.Center, true);
        }
        private bool CanAddItem()
        {
            return IsEditing;
        }

        private void DeleteItem(ProformaInvoicePanel item)
        {
            MessageBoxResult result =
                MessageView.Show($"Delete",
                                 $"Are you sure want to Delete\n{item.Description}?",
                                 MessageViewButton.YesNo,
                                 MessageViewImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Items.Remove(item);
            }
        }
        private bool CanDeleteItem(ProformaInvoicePanel item)
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
                string query;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    query = $"Select IsNUll(MAX(Number),0) As Number " +
                            $"From [Finance].[ProformaInvoices] " +
                            $"Where Year = {System.DateTime.Now.Year} ";
                    NewData.Number = connection.QueryFirstOrDefault<int>(query) + 1;
                    NewData.Code = $"ER-{NewData.Date.Year}{NewData.Date.Month:00}{NewData.Number:000}P";
                    NewData.Year = NewData.Date.Year;
                    query = $"Select Address " +
                            $"From [Customer].[Customers] " +
                            $"Where CustomerID = {NewData.CustomerId} ";
                    NewData.Address = connection.QueryFirstOrDefault<string>(query);

                    query = $"Select VATNumber " +
                            $"From [Customer].[Customers] " +
                            $"Where CustomerID = {NewData.CustomerId} ";
                    NewData.CustomerVATNumber = connection.QueryFirstOrDefault<string>(query);

                    query = $"Select Number " +
                            $"From [JobOrder].[PurchaseOrders] " +
                            $"Where JobOrderID = {NewData.JobOrderId} ";
                    NewData.PurchaseOrder = connection.QueryFirstOrDefault<string>(query);

                    NewData.Panels = Items.Count;

                    _ = connection.Insert(NewData);

                    foreach (ProformaInvoicePanel item in Items)
                    {
                        item.InvoiceId = NewData.Id;
                        item.Code = NewData.JobOrderCode + "-" + item.SN;
                        item.VAT = NewData.VAT;
                    }

                    _ = connection.Insert(Items);

                }

                InvoiceData.Update(NewData);

                if (InvoicesData != null)
                {
                    InvoicesData.Add(InvoiceData);
                }

                OnPropertyChanged(nameof(IsNew));
                OnPropertyChanged(nameof(IsEditing));
                OnPropertyChanged(nameof(AmountEditing));
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

            if (NewData.Type == "Invoice")
                NewData.Amount = NewData.GrossPrice;
        }

        private void Print()
        {
            JobOrdersInvoicesServices.PrintProformaInvoice(NewData.Id, ViewData);
        }
        private bool CanAccessPrint()
        {
            return !IsNew;
        }
    }
}
