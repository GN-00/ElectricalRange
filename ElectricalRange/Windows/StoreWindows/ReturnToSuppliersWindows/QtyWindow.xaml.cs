using ProjectsNow.Data.Store;

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.Windows.StoreWindows.ReturnToSuppliersWindows
{
    public partial class QtyWindow : Window
    {
        public ItemTransaction ItemData { get; set; }
        public ObservableCollection<ItemTransaction> ItemsData { get; set; }

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
                ItemData.ReturnedQty += qty;
                ItemTransaction returnedData = new()
                {
                    JobOrderID = ItemData.JobOrderID,
                    InvoiceID = ItemData.InvoiceID,
                    PanelID = ItemData.PanelID,
                    PanelTransactionID = ItemData.PanelTransactionID,
                    Code = ItemData.Code,
                    Description = ItemData.Description,
                    Reference = ItemData.ID,
                    Source = ItemData.InvoiceID.ToString(),
                    Type = Enums.TransactionType.Returned.ToString(),
                    Unit = ItemData.Unit,
                    Qty = qty,
                    Cost = ItemData.Cost,
                    Date = DateTime.Now,
                    VAT = ItemData.VAT,
                    OriginalInvoiceID = ItemData.OriginalInvoiceID,
                };

                ItemsData.Add(returnedData);
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
