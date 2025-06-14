using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Store;

using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.Windows.StoreWindows.TransferWindows
{
    public partial class QtyWindow : Window
    {
        public JobOrder JobOrderData { get; set; }
        public ItemTransaction ItemData { get; set; }
        public ItemPurchased JobOrderItemData { get; set; }
        public SupplierInvoice InvoiceData { get; set; }
        public ObservableCollection<ItemTransaction> ItemsData { get; set; }

        public QtyWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (JobOrderItemData.RemainingQty <= ItemData.FinalQty)
            {
                ItemsToPostInput.Text = $"{JobOrderItemData.RemainingQty:N2}";
            }
            else
            {
                ItemsToPostInput.Text = $"{ItemData.FinalQty:N2}";
            }

            InNeed.Text = $"{JobOrderItemData.RemainingQty:N2}/{JobOrderItemData.Qty:N2}";
            DataContext = JobOrderData;
            _ = ItemsToPostInput.Focus();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Posting_Click(object sender, RoutedEventArgs e)
        {
            JobOrder receiverJobOrderData = JobOrderData;
            string query;
            SupplierInvoice senderInvoice;
            SupplierInvoice receiverInvoice;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Store].[SuppliersInvoices] Where ID = {ItemData.InvoiceID}";
            senderInvoice = connection.QueryFirstOrDefault<SupplierInvoice>(query);
            receiverInvoice = InvoiceData;

            decimal qty = decimal.Parse(ItemsToPostInput.Text);
            if (qty > 0)
            {
                JobOrderItemData.PurchasedQty += qty;
                ItemData.TransferredQty += qty;
                ItemTransaction transferData = new()
                {
                    JobOrderID = 0,
                    InvoiceID = senderInvoice.ID,
                    PanelID = null,
                    PanelTransactionID = null,
                    Code = ItemData.Code,
                    Description = ItemData.Description,
                    Reference = ItemData.ID,
                    Source = ItemData.Source,
                    Type = "Transfer",
                    Unit = ItemData.Unit,
                    Qty = qty,
                    Cost = ItemData.Cost,
                    Date = DateTime.Now,
                    VAT = ItemData.VAT,
                    OriginalInvoiceID = ItemData.OriginalInvoiceID,
                };
                _ = connection.Insert(transferData);

                ItemTransaction newItem = new()
                {
                    JobOrderID = receiverJobOrderData.ID,
                    InvoiceID = receiverInvoice.ID,
                    PanelID = null,
                    PanelTransactionID = null,
                    Code = ItemData.Code,
                    Description = ItemData.Description,
                    Source = "0",
                    Type = "Stock",
                    Unit = ItemData.Unit,
                    Qty = qty,
                    Cost = ItemData.Cost,
                    Date = DateTime.Now,
                    TransferInvoiceID = transferData.ID,
                    VAT = ItemData.VAT,
                    OriginalInvoiceID = ItemData.OriginalInvoiceID,
                };
                _ = connection.Insert(newItem);

                ItemsData.Add(newItem);
                Close();
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ItemsToPostInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ItemsToPostInput.Text))
            {
                ItemsToPostInput.Text = ItemData.FinalQty.ToString("N2");
            }
            else
            {
                decimal qty = decimal.Parse(ItemsToPostInput.Text);
                if (qty > JobOrderItemData.Qty)
                {
                    ItemsToPostInput.Text = JobOrderItemData.RemainingQty.ToString("N2");
                }
            }
        }
        private void ItemsToPostInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ItemData.Unit == "No" || ItemData.Unit == "Set")
            {
                DataInput.Input.IntOnly(e, 4);
            }
            else
            {
                DataInput.Input.DoubleOnly(e);
            }
        }
    }
}
