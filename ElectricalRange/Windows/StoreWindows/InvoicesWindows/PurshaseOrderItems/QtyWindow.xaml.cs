using Dapper.Contrib.Extensions;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Store;

using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows.PurshaseOrderItems
{
    public partial class QtyWindow : Window
    {
        public SupplierInvoice Invoice { get; set; }
        public CompanyPOTransaction ItemData { get; set; }
        public ObservableCollection<CompanyPOTransaction> ItemsData { get; set; }

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
            decimal qty = decimal.Parse(ItemsToPostInput.Text);
            if (qty > 0)
            {
                CompanyPOTransaction newCompanyPOTransaction = new();
                newCompanyPOTransaction.Update(ItemData);
                newCompanyPOTransaction.Qty = qty;
                newCompanyPOTransaction.ReceivedQty = 0;
                newCompanyPOTransaction.Type = "Received Invoice";

                ItemData.ReceivedQty += qty;
                ItemTransaction receivedData = new()
                {
                    JobOrderID = Invoice.JobOrderID,
                    InvoiceID = Invoice.ID,
                    
                    Code = ItemData.Code,
                    Description = ItemData.Description,
                    Reference = ItemData.ID,
                    Source = "New",
                    Type = Enums.TransactionType.Stock.ToString(),
                    Unit = ItemData.Unit,
                    Qty = qty,
                    Cost = ItemData.Cost,
                    Date = DateTime.Now,
                    VAT = ItemData.VAT,
                    OriginalInvoiceID = null,
                };

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(receivedData);
                }

                ItemsData.Add(newCompanyPOTransaction);
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
