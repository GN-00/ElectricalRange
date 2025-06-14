using Dapper;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.References;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class PanelItemsWindow : Window
    {
        public User UserData { get; set; }
        public JPanel PanelData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public List<Reference> ReferencesListData { get; set; }

        public Tables TableData;
        public ObservableCollection<JItem> ItemsData;
        public ObservableCollection<JItem> ItemsDetails;
        public ObservableCollection<JItem> ItemsEnclosure;

        public void ListChanged()
        {
            ////Unit Cost
            ///decimal total = ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
            //                ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            //decimal detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            //decimal enclosureCost = ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));
            //decimal copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

            ////Unit Price
            //decimal detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit/100)));
            //decimal enclosureCost = ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)));
            //decimal copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100) * (1 / (1 - PanelData.PanelProfit / 100)));

            //Total Price
            PanelData.PanelDesignCost =
                ItemsDetails.Sum(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100m)) +
                ItemsEnclosure.Sum(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100m));

            decimal total = ItemsDetails.Sum(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100m) * (1m / (1m - PanelData.PanelProfit / 100m)) * PanelData.PanelQty) +
                           ItemsEnclosure.Sum(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100m) * (1m / (1m - PanelData.PanelProfit / 100m)) * PanelData.PanelQty);

            decimal detailsCost = ItemsDetails.Where(i => i.Article1 != "COPPER").Sum(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100m) * (1m / (1m - PanelData.PanelProfit / 100m)) * PanelData.PanelQty);
            decimal enclosureCost = ItemsEnclosure.Sum(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100m) * (1m / (1m - PanelData.PanelProfit / 100m)) * PanelData.PanelQty);
            decimal copperCost = ItemsDetails.Where(i => i.Article1 == "COPPER").Sum(item => item.ItemCost * item.ItemQty * (1m - item.ItemDiscount / 100m) * (1m / (1m - PanelData.PanelProfit / 100m)) * PanelData.PanelQty);

            DetailsCost.Text = $"{detailsCost:N2} ({detailsCost / total * 100:N2} %)";
            EnclosureCost.Text = $"{enclosureCost:N2} ({enclosureCost / total * 100:N2} %)";
            CopperCost.Text = $"{copperCost:N2} ({copperCost / total * 100:N2} %)";
        }
        public PanelItemsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                ItemsDetails = JItemController.PanelDetails(connection, PanelData.PanelID);
                ItemsEnclosure = JItemController.PanelEnclosure(connection, PanelData.PanelID);
            }

            TableData = Tables.Details;
            ItemsData = ItemsDetails;

            viewData = new CollectionViewSource() { Source = ItemsData };
            viewData.Filter += DataFilter;
            ItemsList.ItemsSource = viewData.View;

            DataContext = new { UserData, PanelData, JobOrderData };
            ListChanged();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }
        private void Modification_Click(object sender, RoutedEventArgs e)
        {
            ItemsModificationWindow itemsModificationWindow = new()
            {
                UserData = UserData,
                JobOrderData = JobOrderData,
            };

            _ = itemsModificationWindow.ShowDialog();
            Window_Loaded(sender, e);
        }
        private void ModificationsHistory_Click(object sender, RoutedEventArgs e)
        {
            ModificationsHistoryWindow modificationsHistoryWindow = new()
            {
                UserData = UserData,
                JobOrderData = JobOrderData,
            };
            _ = modificationsHistoryWindow.ShowDialog();
        }
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is JItem itemData)
            {
                if (ItemsData.Count > 1)
                {
                    int minIndex = ItemsData.Where(i => i.ItemQty != 0).Min(i => i.ItemSort);

                    if (itemData.ItemSort > minIndex)
                    {
                        int currentIndex = itemData.ItemSort;
                        JItem previousItem = ItemsData.FirstOrDefault(item => item.ItemSort == ItemsData.Where(i => i.ItemSort < currentIndex && i.ItemQty != 0).Max(i => i.ItemSort));
                        int previousIndex = previousItem.ItemSort;

                        itemData.ItemSort = previousIndex;
                        previousItem.ItemSort = currentIndex;

                        ItemsData.Move(ItemsData.IndexOf(itemData), ItemsData.IndexOf(previousItem));

                        string query = $"Update [JobOrder].[PanelsItems] Set ItemSort = {-1} Where PanelID = {itemData.PanelID} And ItemSort = {currentIndex} And ItemTable = '{TableData}'" +
                                       $"Update [JobOrder].[PanelsItems] Set ItemSort = {currentIndex} Where PanelID = {itemData.PanelID} And ItemSort = {previousIndex} And ItemTable = '{TableData}'; " +
                                       $"Update [JobOrder].[PanelsItems] Set ItemSort = {previousIndex} Where PanelID = {itemData.PanelID} And ItemSort = {-1} And ItemTable = '{TableData}'; ";

                        using SqlConnection connection = new(Database.ConnectionString);
                        _ = connection.Execute(query);
                    }
                }

            }
        }
        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is JItem itemData)
            {
                if (ItemsData.Count > 1)
                {
                    int maxIndex = ItemsData.Where(i => i.ItemQty != 0).Max(i => i.ItemSort);

                    if (itemData.ItemSort < maxIndex)
                    {
                        int currentIndex = itemData.ItemSort;
                        JItem nextItem = ItemsData.FirstOrDefault(item => item.ItemSort == ItemsData.Where(i => i.ItemSort > currentIndex && i.ItemQty != 0).Min(i => i.ItemSort));
                        int nextIndex = nextItem.ItemSort;

                        itemData.ItemSort = nextIndex;
                        nextItem.ItemSort = currentIndex;

                        ItemsData.Move(ItemsData.IndexOf(itemData), ItemsData.IndexOf(nextItem));

                        string query = $"Update [JobOrder].[PanelsItems] Set ItemSort = {-1} Where PanelID = {itemData.PanelID} And ItemSort = {currentIndex} And ItemTable = '{TableData}'" +
                                       $"Update [JobOrder].[PanelsItems] Set ItemSort = {currentIndex} Where PanelID = {itemData.PanelID} And ItemSort = {nextIndex} And ItemTable = '{TableData}'; " +
                                       $"Update [JobOrder].[PanelsItems] Set ItemSort = {nextIndex} Where PanelID = {itemData.PanelID} And ItemSort = {-1} And ItemTable = '{TableData}'; ";

                        using SqlConnection connection = new(Database.ConnectionString);
                        _ = connection.Execute(query);
                    }
                }
            }
        }
        private void Details_Click(object sender, RoutedEventArgs e)
        {
            TableData = Tables.Details;
            TableName.Text = TableData.ToString();
            ItemsData = ItemsDetails;

            viewData.Source = ItemsData;
            ItemsList.ItemsSource = viewData.View;
        }
        private void Enclosure_Click(object sender, RoutedEventArgs e)
        {
            TableData = Tables.Enclosure;
            TableName.Text = TableData.ToString();
            ItemsData = ItemsEnclosure;

            viewData.Source = ItemsData;
            ItemsList.ItemsSource = viewData.View;
        }
        private void TableChange_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is JItem itemData)
            {
                if (TableData == Tables.Details)
                {
                    itemData.ItemTable = Tables.Enclosure.ToString();
                    _ = ItemsDetails.Remove(itemData);
                    itemData.ItemSort = ItemsEnclosure.Max(i => i.ItemSort) + 1;
                    ItemsEnclosure.Add(itemData);
                }
                else
                {
                    itemData.ItemTable = Tables.Details.ToString();
                    _ = ItemsEnclosure.Remove(itemData);
                    itemData.ItemSort = ItemsDetails.Max(i => i.ItemSort) + 1;
                    ItemsDetails.Add(itemData);
                }

                using SqlConnection connection = new(Database.ConnectionString);
                string query = $"Update [JobOrder].[PanelsItems] Set " +
                               $"ItemTable = '{itemData.ItemTable}', ItemSort = {itemData.ItemSort} " +
                               $"Where PanelID = {itemData.PanelID} And Code ='{itemData.Code}'";
                _ = connection.Execute(query, itemData);
            }
        }

        #region Filters

        private CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new()
        {
            typeof(JItem).GetProperty("Code"),
            typeof(JItem).GetProperty("Description"),
            typeof(JItem).GetProperty("ItemQty"),
            typeof(JItem).GetProperty("Brand"),
            typeof(JItem).GetProperty("ItemCost"),
            typeof(JItem).GetProperty("ItemPrice"),
            typeof(JItem).GetProperty("ItemTotalCost"),
            typeof(JItem).GetProperty("ItemTotalPrice"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JItem record)
                {
                    string columnName;
                    foreach (PropertyInfo property in filterProperties)
                    {
                        columnName = property.Name;
                        string value;
                        if (property.PropertyType == typeof(DateTime))
                        {
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record):dd/MM/yyyy}";
                        }
                        else
                        {
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record)}".ToUpper();
                        }

                        if (record.ItemQty == 0)
                        {
                            e.Accepted = false;
                            return;
                        }

                        if (!value.Contains(((TextBox)FindName(property.Name)).Text.ToUpper()))
                        {
                            e.Accepted = false;
                            return;
                        }
                    }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }

        private void ApplyFilter(object sender, KeyEventArgs e)
        {
            viewData.View.Refresh();
        }

        private void DeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            foreach (PropertyInfo property in filterProperties)
            {
                ((TextBox)FindName(property.Name)).Text = null;
            }
            viewData.View.Refresh();
        }


        #endregion


    }
}
