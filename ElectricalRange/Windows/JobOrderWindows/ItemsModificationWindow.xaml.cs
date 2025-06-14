using Dapper;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.References;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Events;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class ItemsModificationWindow : Window
    {
        public User UserData { get; set; }
        public int JobOrderID { get; set; }
        public JobOrder JobOrderData { get; set; }

        private List<ModificationItem> jobOrderItems;
        private List<Modification> modifications;
        private ObservableCollection<ModificationItem> items;
        private ObservableCollection<ModificationPanel> panels;
        private ObservableCollection<Reference> referencesData;

        public ItemsModificationWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            referencesData = AppData.ReferencesListData;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (JobOrderData == null)
                {
                    JobOrderData = JobOrderController.JobOrder(connection, JobOrderID);
                }

                panels = ModificationPanelController.GetPanels(connection, JobOrderData.ID);
                items = ModificationItemController.GetModificationsItems(connection, JobOrderData.ID);
                jobOrderItems = ModificationItemController.GetAllItems(connection, JobOrderData.ID);

                if (referencesData == null)
                {
                    referencesData =
                        AppData.ReferencesListData =
                            new ObservableCollection<Reference>(ReferenceController.GetReferences(connection));
                }
            }

            viewDataPanels = new CollectionViewSource() { Source = panels };
            viewDataItems = new CollectionViewSource() { Source = items };

            viewDataItems.Filter += DataFilter;

            PanelsList.ItemsSource = viewDataPanels.View;
            ItemsList.ItemsSource = viewDataItems.View;

            viewDataPanels.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Panels_CollectionChanged);
            viewDataItems.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (viewDataPanels.View.Cast<object>().Count() == 0)
            {
                Panels_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            if (viewDataItems.View.Cast<object>().Count() == 0)
            {
                Items_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            modifications = items.GroupBy(i => i.ModificationID, ii => ii.Date).Select(m => new Modification() { ID = m.Key, Date = m.ToList()[0] }).OrderByDescending(m => m.ID).ToList();
            ModificationsList.ItemsSource = modifications;
            ModificationsList.SelectedItem = modifications.FirstOrDefault();

            DataContext = new { JobOrderData, UserData };
        }
        private void Panels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            int selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";
            }
        }
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationItems.Text = $"Items: {viewDataItems.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationItems.Text = $"Item: {selectedIndex + 1} / {viewDataItems.View.Cast<object>().Count()}";
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewDataPanels.View.SortDescriptions.Add(new SortDescription("ItemSort", ListSortDirection.Ascending));
            }
        }

        #region Filters

        private CollectionViewSource viewDataPanels;
        private CollectionViewSource viewDataItems;
        private readonly List<PropertyInfo> filterProperties = new()
        {
            typeof(ModificationPanel).GetProperty("PanelID"),
        };

        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is ModificationItem record)
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
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record)}";
                        }

                        if (PanelsList.SelectedItem is ModificationPanel panel)
                        {
                            if (value != panel.PanelID.ToString())
                            {
                                e.Accepted = false;
                                return;
                            }
                        }
                    }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }

        #endregion
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            int selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";
            }
        }
        private void ItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationItems.Text = $"Items: {viewDataItems.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationItems.Text = $"Item: {selectedIndex + 1} / {viewDataItems.View.Cast<object>().Count()}";
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Visible;
            ControlLock2.Visibility = Visibility.Collapsed;
            int newID = 0;
            if (modifications.Count != 0)
            {
                newID = modifications.Max(m => m.ID);
            }

            Modification newModification = new() { ID = ++newID, JobOrderID = JobOrderData.ID, Date = DateTime.Now };
            modifications.Insert(0, newModification);
            ModificationsList.SelectedItem = newModification;

            ItemsList.ContextMenu = (ContextMenu)Resources["ItemsListContextMenu"];
            ItemsList.RowStyle = (Style)Resources["NewItems"];
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ModificationsList.SelectedItem is Modification modificationData)
            {
                LoadingControl.Visibility = Visibility.Visible;
                ShowEvent.Do();
                ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Collapsed;
                ControlLock2.Visibility = Visibility.Visible;

                string query;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    foreach (ModificationItem item in items.Where(i => i.ModificationID == modificationData.ID))
                    {
                        if (item.ItemSort == -1)
                        {
                            query = $"Select Max(ItemSort) From [JobOrder].[PanelsItems] Where PanelID ={item.PanelID} And ItemTable = '{item.ItemTable}'";
                            item.ItemSort = (int)connection.ExecuteScalar(query) + 1;
                        }
                        query = $"Insert Into [JobOrder].[PanelsItems] " +
                                $"(PanelID, Category, Code,  Description, Unit, ItemQty, Brand, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort, ModificationID, Source, Date) " +
                                $"Values " +
                                $"(@PanelID, @Category, @Code,  @Description, @Unit, @ItemQty, @Brand, @ItemCost, @ItemDiscount, @ItemTable, @ItemType, @ItemSort, @ModificationID, @Source, @Date) " +
                                $"Select @@IDENTITY";

                        item.ItemID = (int)(decimal)connection.ExecuteScalar(query, item);
                    }
                }
                ItemsList.ContextMenu = null;
                ItemsList.RowStyle = (Style)Resources["Items"];
                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (ModificationsList.SelectedItem is Modification modificationData)
            {
                LoadingControl.Visibility = Visibility.Visible;
                ShowEvent.Do();
                ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Collapsed;
                ControlLock2.Visibility = Visibility.Visible;

                List<ModificationItem> list = new();
                foreach (ModificationItem item in items.Where(i => i.ModificationID == modificationData.ID))
                {
                    ModificationItem jobOrderItem = jobOrderItems.FirstOrDefault(i => i.Code == item.Code);
                    jobOrderItem.ItemQty += item.ItemQty * -1;
                    list.Add(item);
                }

                foreach (ModificationItem item in list)
                {
                    _ = items.Remove(item);
                }


                ItemsList.ContextMenu = null;
                ItemsList.RowStyle = (Style)Resources["Items"];
                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (ModificationsList.SelectedIndex > 0)
            {
                ModificationsList.SelectedIndex -= 1;
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (ModificationsList.SelectedIndex < ModificationsList.Items.Count - 1)
            {
                ModificationsList.SelectedIndex += 1;
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is ModificationPanel panel)
            {
                Items_Standard_Window window = new()
                {
                    PanelID = panel.PanelID,
                    ItemData = null,
                    ActionData = Actions.New,
                    ModificationData = ModificationsList.SelectedItem as Modification,
                    ItemsData = items,
                    ReferencesData = referencesData,
                    JobOrderItemsData = jobOrderItems,
                };
                _ = window.ShowDialog();
            }
        }
        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is ModificationPanel panel)
            {
                Items_Standard_Window window = new()
                {
                    PanelID = panel.PanelID,
                    ItemData = null,
                    ActionData = Actions.Remove,
                    ModificationData = ModificationsList.SelectedItem as Modification,
                    ItemsData = items,
                    ReferencesData = referencesData,
                    JobOrderItemsData = jobOrderItems,
                };
                _ = window.ShowDialog();
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is ModificationItem item)
            {
                if (ModificationsList.SelectedItem is Modification modification)
                {
                    if (item.ModificationID == modification.ID)
                    {
                        if (item.ItemType == ItemTypes.Standard.ToString())
                        {
                            Items_Standard_Window window = new()
                            {
                                PanelID = (PanelsList.SelectedItem as ModificationPanel).PanelID,
                                ItemData = item,
                                ActionData = Actions.Edit,
                                ModificationData = ModificationsList.SelectedItem as Modification,
                                ItemsData = items,
                                ReferencesData = referencesData,
                                JobOrderItemsData = jobOrderItems,
                            };
                            _ = window.ShowDialog();
                        }
                        else
                        {
                            if (item.Source == "Additional")
                            {
                                Items_New_Window window = new()
                                {
                                    PanelID = (PanelsList.SelectedItem as ModificationPanel).PanelID,
                                    ItemData = item,
                                    ActionData = Actions.Edit,
                                    ModificationData = ModificationsList.SelectedItem as Modification,
                                    ItemsData = items,
                                    JobOrderItemsData = jobOrderItems,
                                };
                                _ = window.ShowDialog();
                            }
                            else
                            {
                                Items_Standard_Window window = new()
                                {
                                    PanelID = (PanelsList.SelectedItem as ModificationPanel).PanelID,
                                    ItemData = item,
                                    ActionData = Actions.Edit,
                                    ModificationData = ModificationsList.SelectedItem as Modification,
                                    ItemsData = items,
                                    ReferencesData = referencesData,
                                    JobOrderItemsData = jobOrderItems,
                                };
                                _ = window.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        _ = MessageWindow.Show("Posted Item!", "Can't edit posted items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    }
                }
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is ModificationItem item)
            {
                if (ModificationsList.SelectedItem is Modification modification)
                {
                    if (item.ModificationID == modification.ID)
                    {
                        ModificationItem jobOrderItem = jobOrderItems.FirstOrDefault(i => i.Code == item.Code);
                        jobOrderItem.ItemQty += item.ItemQty * -1;
                        _ = items.Remove(item);
                    }
                    else
                    {
                        _ = MessageWindow.Show("Posted Item!", "Can't delete posted items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    }
                }
            }
        }
    }
}
