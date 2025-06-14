using Dapper;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class SelectionWindow : Window
    {
        public User UserData { get; set; }
        public string GroupName { get; set; }
        public QPanel PanelData { get; set; }
        public ObservableCollection<QItem> Items { get; set; }

        private Group group;
        private List<string> items;
        private List<Filter> filters;
        private List<List<string>> values;
        public SelectionWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            items = new List<string>();
            values = new List<List<string>>()
            {
                new(){ null, null, null, null, null, null, null },
                new(){ null, null, null, null, null, null, null },
                new(){ null, null, null, null, null, null, null },
                new(){ null, null, null, null, null, null, null },
                new(){ null, null, null, null, null, null, null },
                new(){ null, null, null, null, null, null, null },
                new(){ null, null, null, null, null, null, null },
                new(){ null, null, null, null, null, null, null },
                new(){ null, null, null, null, null, null, null },
                new(){ null, null, null, null, null, null, null },
            };

            using SqlConnection connection = new(Database.ConnectionString);
            group = connection.QueryFirstOrDefault<Group>($"Select * From [Reference].[GroupProperties] Where Name ='{GroupName}'");
            filters = connection.Query<Filter>($"Select * From [Reference].[Filters] where GroupID = {group.ID}").ToList();

            GroupTitle.Text = group.Name;
            GroupImage.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/Icons/{group.Image}.png"));

            for (int i = 1; i <= 9; i++)
            {
                items.Add(group.GetType().GetProperty($"Items{i}").GetValue(group) as string);

                if ((string)group.GetType().GetProperty($"Property{i}").GetValue(group) != null)
                {
                    ((TabItem)FindName($"Tab{i}")).Visibility = Visibility.Visible;
                    ((TabItem)FindName($"Tab{i}")).Header = (string)group.GetType().GetProperty($"Property{i}").GetValue(group);

                    for (int ii = 1; ii <= 6; ii++)
                    {
                        if ((string)group.GetType().GetProperty($"Property{i}{ii}").GetValue(group) != null)
                        {
                            ((Grid)FindName($"Grid{i}{ii}")).Visibility = Visibility.Visible;
                            ((TextBlock)FindName($"Label{i}{ii}")).Text = (string)group.GetType().GetProperty($"Property{i}{ii}").GetValue(group);

                            string query = FilterController.PropertyFilter(filters.FirstOrDefault(item => item.PropertyName == $"Property{i}{ii}"), group, values);
                            ((ListBox)FindName($"ListBox{i}{ii}")).ItemsSource = connection.Query<string>(query).ToList();
                        }
                    }
                }
            }
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedItem != null)
            {
                int Selected_i = int.Parse(((ListBox)sender).Name.Substring(7, 1));
                int Selected_ii = int.Parse(((ListBox)sender).Name.Substring(8, 1));

                if (((ListBox)sender).SelectedItem.ToString() != values[Selected_i][Selected_ii])
                {
                    values[Selected_i][Selected_ii] = ((ListBox)sender).SelectedItem.ToString();
                    ((Image)FindName("Image" + Selected_i + Selected_ii)).Source = new BitmapImage(new Uri("pack://application:,,,/Images/Icons/Green.png"));

                    using SqlConnection connection = new(Database.ConnectionString);
                    for (int i = 1; i <= 9; i++)
                    {
                        if ((string)group.GetType().GetProperty($"Property{i}").GetValue(group) != null)
                        {
                            for (int ii = 1; ii <= 6; ii++)
                            {
                                if (group.GetType().GetProperty($"Property{i}{ii}").GetValue(group) != null)
                                {
                                    if (i == Selected_i && ii == Selected_ii)
                                    { }
                                    else
                                    {
                                        ((ListBox)FindName("ListBox" + i + ii)).SelectionChanged -= ListBox_SelectionChanged;

                                        string query = FilterController.PropertyFilter(filters.FirstOrDefault(item => item.PropertyName == $"Property{i}{ii}"), group, values);
                                        ((ListBox)FindName($"ListBox{i}{ii}")).ItemsSource = connection.Query<string>(query).ToList();

                                        ((ListBox)FindName("ListBox" + i + ii)).SelectionChanged += ListBox_SelectionChanged;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool isReady = true;
            string message = "Please select: ";
            for (int i = 1; i <= 9; i++)
            {
                if (((TabItem)FindName($"Tab{i}")).Visibility == Visibility.Visible)
                {
                    for (int ii = 1; ii <= 6; ii++)
                    {
                        if (((Grid)FindName($"Grid{i}{ii}")).Visibility == Visibility.Visible)
                        {
                            if (values[i][ii] == null)
                            {
                                isReady = false;
                                message += $"\n ({group.GetType().GetProperty($"Property{i}").GetValue(group)}) {group.GetType().GetProperty($"Property{i}{ii}").GetValue(group)}";
                            }
                        }
                    }
                }
            }

            if (isReady == true)
            {
                string query;
                int detailsCounter = Items.Where(x => x.ItemTable == Tables.Details.ToString()).Count();
                int enclosureCounter = Items.Where(x => x.ItemTable == Tables.Enclosure.ToString()).Count();
                List<QItem> itemsData = new();
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    for (int i = 1; i <= 9; i++)
                    {
                        if (items[i - 1] != null)
                        {
                            query = $"{FilterController.ItemFilter(filters.FirstOrDefault(f => f.PropertyName == $"Items{i}"), group, values)}; ";
                            itemsData.AddRange(connection.Query<QItem>(query));
                        }
                    }

                    query = "";
                    if (group.Category == "Enclosure")
                    {
                        int newDetailsCount = itemsData.Where(i => i.ItemTable == Tables.Details.ToString()).Count();
                        int newEnclosureCount = itemsData.Where(i => i.ItemTable == Tables.Enclosure.ToString()).Count();

                        List<QItem> smartEnclosure = new();
                        smartEnclosure.AddRange(Items.Where(i => i.SelectionGroup == SelectionGroups.SmartEnclosure.ToString() && i.ItemTable == Tables.Details.ToString()));
                        smartEnclosure.AddRange(Items.Where(i => i.SelectionGroup == SelectionGroups.SmartEnclosure.ToString() && i.ItemTable == Tables.Enclosure.ToString()));
                        int oldDetailsCount = smartEnclosure.Where(i => i.ItemTable == Tables.Details.ToString()).Count();
                        int oldEnclosureCount = smartEnclosure.Where(i => i.ItemTable == Tables.Enclosure.ToString()).Count();

                        query += $"Delete From [Quotation].[QuotationsPanelsItems] Where PanelID = {PanelData.PanelID} And SelectionGroup = '{SelectionGroups.SmartEnclosure}';";
                        query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = (ItemSort - {oldDetailsCount} + {newDetailsCount}) Where (PanelID = {PanelData.PanelID}) And (ItemTable = '{Tables.Details}'); ";
                        query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = (ItemSort - {oldEnclosureCount} + {newEnclosureCount}) Where (PanelID = {PanelData.PanelID}) And (ItemTable = '{Tables.Enclosure}'); ";

                        foreach (QItem item in smartEnclosure)
                        {
                            _ = Items.Remove(item);
                        }

                        foreach (QItem item in Items.Where(i => i.ItemTable == Tables.Details.ToString()))
                        {
                            item.ItemSort = item.ItemSort - oldDetailsCount + newDetailsCount;
                        }

                        foreach (QItem item in Items.Where(i => i.ItemTable == Tables.Enclosure.ToString()))
                        {
                            item.ItemSort = item.ItemSort - oldEnclosureCount + newEnclosureCount;
                        }
                    }

                    int enclosureGroupDetailsCount = 0;
                    int enclosureGroupEnclosureCount = 0;
                    foreach (QItem itemData in itemsData)
                    {
                        if (itemData.ItemType == null)
                        {
                            itemData.ItemType = ItemTypes.Standard.ToString();
                        }

                        itemData.SelectionGroup = SelectionGroups.SmartEnclosure.ToString();
                        query += $"Insert Into [Quotation].[QuotationsPanelsItems] " +
                                 $"(PanelID, Article1, Article2, Category, Code, Description, Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort, SelectionGroup) " +
                                 $"Values ";

                        if (group.Category == "Enclosure")
                        {
                            if (itemData.ItemTable == Tables.Details.ToString())
                            {
                                itemData.ItemSort = enclosureGroupDetailsCount++;
                            }
                            else
                            {
                                itemData.ItemSort = enclosureGroupEnclosureCount++;
                            }

                            query += $"({PanelData.PanelID}, '{itemData.Article1}', '{itemData.Article2}', '{itemData.Category}', '{itemData.Code}', '{itemData.Description}', '{itemData.Unit}', '{itemData.ItemQty}', '{itemData.Brand}', '{itemData.Remarks}', {itemData.ItemCost}, {itemData.ItemDiscount}, '{itemData.ItemTable}', '{itemData.ItemType}', {itemData.ItemSort}, '{itemData.SelectionGroup}') ";
                        }
                        else
                        {
                            if (itemData.ItemTable == Tables.Details.ToString())
                            {
                                itemData.ItemSort = ++detailsCounter;
                            }
                            else
                            {
                                itemData.ItemSort = ++enclosureCounter;
                            }

                            query += $"({PanelData.PanelID}, '{itemData.Article1}', '{itemData.Article2}', '{itemData.Category}', '{itemData.Code}', '{itemData.Description}', '{itemData.Unit}', '{itemData.ItemQty}', '{itemData.Brand}', '{itemData.Remarks}', {itemData.ItemCost}, {itemData.ItemDiscount}, '{itemData.ItemTable}', '{itemData.ItemType}', {itemData.ItemSort}, '{itemData.SelectionGroup}') ";
                        }

                        query += $"; ";

                        Items.Add(itemData);
                    }

                    _ = connection.Execute(query);
                    if (group.Category == "Enclosure" && group.Name != "Disbo")
                    {
                        QPanelController.UpdateEnclosure(connection, PanelData, group, values, null);
                    }
                    else if (group.Category == "Enclosure" && group.Name == "Disbo")
                    {
                        QPanelController.UpdateEnclosure(connection, PanelData, group, values, Items);
                    }
                }

                Close();
            }
            else
            {
                _ = MessageWindow.Show(message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow_Click(sender, e);
        }
    }
}
