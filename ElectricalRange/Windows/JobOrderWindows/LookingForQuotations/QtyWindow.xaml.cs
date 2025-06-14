using Dapper.Contrib.Extensions;

using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Enums;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.Windows.JobOrderWindows.LookingForQuotations
{
    public partial class QtyWindow : Window
    {
        public Actions ActionData { get; set; }
        public ItemPurchased ItemData { get; set; }
        public QuotationRequest RequestData { get; set; }
        public QuotationRequestItem QuotationRequestItemData { get; set; }
        public ObservableCollection<QuotationRequestItem> QuotationRequestItemsData { get; set; }

        public QtyWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ActionData == Actions.New)
            {
                ItemsToPostInput.Text = ItemData.RemainingQty.ToString();
            }
            else
            {
                ItemsToPostInput.Text = (ItemData.RemainingQty + ItemData.InOrderQty).ToString();
            }

            DataContext = ItemData;
            _ = ItemsToPostInput.Focus();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Posting_Click(object sender, RoutedEventArgs e)
        {
            if (ActionData == Actions.Edit)
            {
                decimal qty = decimal.Parse(ItemsToPostInput.Text);
                if (qty > 0)
                {
                    ItemData.InOrderQty += qty;

                    QuotationRequestItemData.Qty = qty;

                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        _ = connection.Update(QuotationRequestItemData);
                    }

                    Close();
                }
            }
            else
            {
                decimal qty = decimal.Parse(ItemsToPostInput.Text);
                if (qty > 0)
                {
                    ItemData.InOrderQty += qty;

                    QuotationRequestItem newItem = new()
                    {
                        JobOrderID = RequestData.JobOrderID,
                        QuotationRequestId = RequestData.Id,
                        Code = ItemData.Code,
                        Description = ItemData.Description,
                        Unit = ItemData.Unit,
                        Qty = qty,
                    };

                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        _ = connection.Insert(newItem);
                    }

                    if (QuotationRequestItemsData != null)
                    {
                        QuotationRequestItemsData.Add(newItem);
                    }

                    Close();
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ItemsToPostInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ActionData == Actions.New)
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
            else
            {
                if (string.IsNullOrWhiteSpace(ItemsToPostInput.Text))
                {
                    ItemsToPostInput.Text = (ItemData.RemainingQty + ItemData.InOrderQty).ToString();
                }
                else
                {
                    decimal qty = decimal.Parse(ItemsToPostInput.Text);
                    if (qty > (ItemData.RemainingQty + ItemData.InOrderQty))
                    {
                        ItemsToPostInput.Text = (ItemData.RemainingQty + ItemData.InOrderQty).ToString();
                    }
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
