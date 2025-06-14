using System;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Input;
using ProjectsNow.Data;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;
using ProjectsNow.Data.JobOrders;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class Items_New_Window : Window
    {
        public int PanelID { get; set; }
        public ModificationItem ItemData { get; set; }
        public Actions ActionData { get; set; }
        public Modification ModificationData { get; set; }
        public List<ModificationItem> JobOrderItemsData { get; set; }
        public ObservableCollection<ModificationItem> ItemsData { get; set; }

        private ModificationItem newItemData;
        public Items_New_Window()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ActionData == Actions.Edit)
            {
                newItemData = new ModificationItem();
                newItemData.Update(ItemData);

                PartNumbersList.Text = newItemData.Code;
                Description.Text = newItemData.Description;
                Brand.Text = newItemData.Brand;
                Table.Text = newItemData.ItemTable;
                Discount.Text = newItemData.ItemDiscount.ToString("N2");
                Cost.Text = newItemData.ItemCost.ToString("N2");
                Qty.Text = newItemData.ItemQty.ToString("N2");
                Unit.Text = newItemData.Unit;
            }
            else
            {
                newItemData = new ModificationItem();
            }

            if (ActionData == Actions.New) { Image.Source = new BitmapImage(new Uri("/Images/Icons/Add.png", UriKind.Relative)); }
            else if (ActionData == Actions.Edit) { Image.Source = new BitmapImage(new Uri("/Images/Icons/Edit.png", UriKind.Relative)); }

            CostCalculator();
            _ = PartNumbersList.Focus();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ModificationItem updateItem;
            bool isReady = true;
            string message = "Please select correct reference!";

            newItemData.Category = PartNumbersList.Text;
            newItemData.Code = Description.Text;
            newItemData.Description = Description.Text;
            newItemData.Brand = Brand.Text;
            newItemData.ItemTable = Table.Text;
            newItemData.ItemQty = decimal.Parse(Qty.Text) * (ActionData == Actions.Remove ? -1 : 1);
            newItemData.ItemDiscount = decimal.Parse(Discount.Text);
            newItemData.ItemCost = decimal.Parse(Cost.Text);
            newItemData.ItemType = ItemTypes.NewItem.ToString();
            newItemData.Unit = Unit.Text;
            newItemData.Date = ModificationData.Date;
            newItemData.ModificationID = ModificationData.ID;
            if (ActionData == Actions.New)
            {
                newItemData.Source = "Additional";
            }

            if (newItemData.Code == "") { isReady = false; message += $"\n*Part Number."; }
            if (newItemData.Description == "") { isReady = false; message += $"\n *Deacription."; }
            if (newItemData.Unit == "No" || newItemData.Unit == "Set" || newItemData.Unit == "Roll")
            {
                bool isInt = (int)newItemData.ItemQty == newItemData.ItemQty;
                if (!isInt) { isReady = false; message += $"\n *Qty must be Integer."; }
            }

            if (isReady)
            {
                if (ActionData == Actions.Edit)
                {
                    updateItem = JobOrderItemsData.FirstOrDefault(i => i.Code == newItemData.Code);
                    if (updateItem != null)
                    {
                        updateItem.ItemQty += ItemData.ItemQty * -1;
                    }

                    ItemData.Update(newItemData);
                }
                else
                {
                    int index = 0;
                    if (JobOrderItemsData.Count != 0)
                    {
                        index = JobOrderItemsData.Max(i => i.ItemSort) + 1;
                    }

                    newItemData.ItemSort = index;
                    newItemData.PanelID = PanelID;

                    ItemsData.Add(newItemData);
                }

                ModificationItem newJobOrderItem = new();
                newJobOrderItem.Update(newItemData);
                updateItem = JobOrderItemsData.FirstOrDefault(i => i.Code == newItemData.Code);
                if (updateItem != null)
                {
                    updateItem.ItemQty += newItemData.ItemQty;
                }
                else
                {
                    JobOrderItemsData.Add(newJobOrderItem);
                }

                Close();
            }
            else
            {
                _ = MessageWindow.Show("Error", message, MessageWindowButton.OK, MessageWindowImage.Warning);
            }

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow_Click(sender, e);
        }
        private void Table_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.ArrowsOnly(e);
        }
        private void Qty_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Unit.Text == "No" || Unit.Text == "Set")
            {
                DataInput.Input.IntOnly(e, 4);
            }
            else
            {
                DataInput.Input.DoubleOnly(e);
            }
        }
        private void Qty_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Qty.Text) || Qty.Text == "0")
            {
                Qty.Text = "1";
            }

            CostCalculator();
        }
        private void Discount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void Discount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Discount.Text))
            {
                Discount.Text = "0";
            }

            if (decimal.Parse(Discount.Text) > 100)
            {
                Discount.Text = "0";
            }

            CostCalculator();
        }
        private void Cost_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void Cost_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Cost.Text))
            {
                Cost.Text = "0";
            }

            CostCalculator();
        }
        private void CostCalculator()
        {
            decimal qty, discount, cost;
            if (string.IsNullOrWhiteSpace(Qty.Text))
            {
                qty = 0;
            }
            else
            {
                qty = decimal.Parse(Qty.Text);
            }

            if (string.IsNullOrWhiteSpace(Discount.Text))
            {
                discount = 1;
            }
            else
            {
                discount = 1m - decimal.Parse(Discount.Text) / 100m;
            }

            if (string.IsNullOrWhiteSpace(Cost.Text))
            {
                cost = 0;
            }
            else
            {
                cost = decimal.Parse(Cost.Text);
            }

            TotalCost.Text = (cost * qty).ToString("N2");
            TotalPrice.Text = (cost * qty * discount).ToString("N2");
        }
    }
}
