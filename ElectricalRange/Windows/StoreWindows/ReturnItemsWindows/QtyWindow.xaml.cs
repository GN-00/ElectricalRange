using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Store;

using System;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.Windows.StoreWindows.ReturnItemsWindows
{
    public partial class QtyWindow : Window
    {
        public JobOrder JobOrderData { get; set; }
        public ItemTransaction ItemData { get; set; }

        public QtyWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ItemsToPostInput.Text = ItemData.FinalQty.ToString();
            _ = ItemsToPostInput.Focus();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Posting_Click(object sender, RoutedEventArgs e)
        {
            JobOrder reciverJobOrderData = Database.Store;
            string query;
            SupplierInvoice senderInvoice;
            SupplierInvoice receiverInvoice;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Store].[SuppliersInvoices] Where ID = {ItemData.InvoiceID}";
            senderInvoice = connection.QueryFirstOrDefault<SupplierInvoice>(query);

            query = $"Select * From [Store].[SuppliersInvoices] Where JobOrderID = {reciverJobOrderData.ID} And Number = '{senderInvoice.Number}'";
            receiverInvoice = connection.QueryFirstOrDefault<SupplierInvoice>(query);

            if (receiverInvoice == null)
            {
                receiverInvoice = new SupplierInvoice();
                receiverInvoice.Update(senderInvoice);
                receiverInvoice.JobOrderID = reciverJobOrderData.ID;
                _ = connection.Insert(receiverInvoice);
            }

            decimal qty = decimal.Parse(ItemsToPostInput.Text);
            if (qty > 0)
            {
                ItemData.TransferredQty += qty;
                ItemTransaction transferData = new()
                {
                    JobOrderID = JobOrderData.ID,
                    InvoiceID = senderInvoice.ID,
                    PanelID = null,
                    PanelTransactionID = null,
                    Code = ItemData.Code,
                    Description = ItemData.Description,
                    Reference = ItemData.ID,
                    Source = ItemData.Source,
                    Type = Enums.TransactionType.Transfer.ToString(),
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
                    JobOrderID = reciverJobOrderData.ID,
                    InvoiceID = receiverInvoice.ID,
                    PanelID = null,
                    PanelTransactionID = null,
                    Code = ItemData.Code,
                    Description = ItemData.Description,
                    Source = JobOrderData.Code,
                    Type = Enums.TransactionType.Stock.ToString(),
                    Unit = ItemData.Unit,
                    Qty = qty,
                    Cost = ItemData.Cost,
                    Date = DateTime.Now,
                    VAT = ItemData.VAT,
                    TransferInvoiceID = transferData.ID,
                    OriginalInvoiceID = ItemData.OriginalInvoiceID,
                };
                _ = connection.Insert(newItem);

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
                ItemsToPostInput.Text = ItemData.FinalQty.ToString();
            }
            else
            {
                decimal qty = decimal.Parse(ItemsToPostInput.Text);
                if (qty > ItemData.FinalQty)
                {
                    ItemsToPostInput.Text = ItemData.FinalQty.ToString();
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
