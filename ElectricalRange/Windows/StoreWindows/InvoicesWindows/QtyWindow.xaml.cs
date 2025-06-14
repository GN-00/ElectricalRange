using Dapper.Contrib.Extensions;

using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Store;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows
{
    public partial class QtyWindow : Window
    {
        public ItemPurchased ItemData { get; set; }
        public SupplierInvoice InvoiceData { get; set; }
        public ObservableCollection<ItemTransaction> ItemsData { get; set; }
        public QtyWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ItemsToPostInput.Text = ItemData.RemainingQty.ToString();
            VATInput.Text = AppData.VAT.ToString();
            DataContext = ItemData;
            _ = ItemsToPostInput.Focus();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Posting_Click(object sender, RoutedEventArgs e)
        {
            decimal qty = decimal.Parse(ItemsToPostInput.Text);
            decimal cost = decimal.Parse(CostInput.Text);
            if (qty > 0)
            {
                ItemData.PurchasedQty += qty;

                ItemTransaction newItem = new()
                {
                    JobOrderID = InvoiceData.JobOrderID,
                    InvoiceID = InvoiceData.ID,
                    PanelID = null,
                    PanelTransactionID = null,
                    Code = ItemData.Code,
                    Description = ItemData.Description,
                    Source = "New",
                    Type = "Stock",
                    Unit = ItemData.Unit,
                    Qty = qty,
                    Cost = cost,
                    VAT = decimal.Parse(VATInput.Text),
                    Date = InvoiceData.Date,
                    OriginalInvoiceID = InvoiceData.ID,
                };

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(newItem);
                }
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
                ItemsToPostInput.Text = ItemData.RemainingQty.ToString();
            }
            else
            {
                decimal qty = decimal.Parse(ItemsToPostInput.Text);
                if (qty > ItemData.RemainingQty)
                {
                    ItemsToPostInput.Text = ItemData.RemainingQty.ToString();
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
        private void CostInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void CostInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CostInput.Text))
            {
                CostInput.Text = "0";
            }
        }

        private void VATInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void VATInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(VATInput.Text) || VATInput.Text == "0")
            {
                VATInput.Text = AppData.VAT.ToString();
            }
        }
    }
}
