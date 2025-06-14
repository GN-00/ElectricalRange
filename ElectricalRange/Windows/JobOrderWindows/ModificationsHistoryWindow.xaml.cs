using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class ModificationsHistoryWindow : Window
    {
        public User UserData { get; set; }
        public int JobOrderID { get; set; }
        public JobOrder JobOrderData { get; set; }

        private List<Modification> modifications;
        private ObservableCollection<ModificationItem> items;
        public ModificationsHistoryWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (JobOrderData == null)
                {
                    JobOrderData = JobOrderController.JobOrder(connection, JobOrderID);
                }

                items = ModificationItemController.GetModificationsItems(connection, JobOrderData.ID);
            }

            modifications = items.GroupBy(i => i.ModificationID, ii => ii.Date).Select(m => new Modification() { ID = m.Key, Date = m.ToList()[0] }).OrderBy(m => m.ID).ToList();

            viewDataModification = new CollectionViewSource() { Source = modifications };
            viewDataItems = new CollectionViewSource() { Source = items };

            viewDataItems.Filter += DataFilter;

            ModificationsList.ItemsSource = viewDataModification.View;
            ItemsList.ItemsSource = viewDataItems.View;

            viewDataModification.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Modifications_CollectionChanged);
            viewDataItems.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (viewDataModification.View.Cast<object>().Count() == 0)
            {
                Modifications_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            if (viewDataItems.View.Cast<object>().Count() == 0)
            {
                Items_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            DataContext = new { JobOrderData, UserData };
        }
        private void Modifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            int selectedIndex = ModificationsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationModifications.Text = $"Modifications: {viewDataModification.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationModifications.Text = $"Modification: {selectedIndex + 1} / {viewDataModification.View.Cast<object>().Count()}";
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
        }
        #region Filters

        private CollectionViewSource viewDataModification;
        private CollectionViewSource viewDataItems;
        private readonly List<PropertyInfo> filterProperties = new()
        {
            typeof(ModificationItem).GetProperty("ModificationID"),
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

                        if (ModificationsList.SelectedItem is Modification modification)
                        {
                            if (value != modification.ID.ToString())
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
        private void ModificationsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            int selectedIndex = ModificationsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationModifications.Text = $"Modifications: {viewDataModification.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationModifications.Text = $"Modification: {selectedIndex + 1} / {viewDataModification.View.Cast<object>().Count()}";
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

        private void Print_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
