using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.References;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class ModificationsWindow : Window
    {
        public User UserData { get; set; }
        public int JobOrderID { get; set; }
        public JobOrder JobOrderData { get; set; }

        private List<Modification> modifications;
        private ObservableCollection<ModificationItem> itemsData;
        private ObservableCollection<ModificationPanel> panelsData;
        private ObservableCollection<Reference> referencesData;
        private ICollectionView panelsView;
        private ICollectionView itemsView;
        public ModificationsWindow()
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

                panelsData = ModificationPanelController.GetPanels(connection, JobOrderData.ID);
                itemsData = ModificationItemController.GetModificationsItems(connection, JobOrderData.ID);

                if (referencesData == null)
                {
                    referencesData =
                        AppData.ReferencesListData =
                            new ObservableCollection<Reference>(ReferenceController.GetReferences(connection));
                }
            }

            panelsView = CollectionViewSource.GetDefaultView(panelsData);
            itemsView = CollectionViewSource.GetDefaultView(itemsData);

            itemsView.Filter = new Predicate<object>(DataFilter);
            panelsView.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
            itemsView.SortDescriptions.Add(new SortDescription("ModificationID", ListSortDirection.Descending));
            itemsView.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            itemsView.GroupDescriptions.Add(new PropertyGroupDescription("ModificationID"));

            panelsView.CollectionChanged += new NotifyCollectionChangedEventHandler(Panels_CollectionChanged);
            itemsView.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            modifications = itemsData.GroupBy(i => i.ModificationID, ii => ii.Date).Select(m => new Modification() { ID = m.Key, Date = m.ToList()[0] }).OrderByDescending(m => m.ID).ToList();

            DataContext = new { JobOrderData, panelsView, itemsView, modifications };

            ModificationsList.SelectedItem = modifications.FirstOrDefault();
        }
        private void Panels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            itemsView.Refresh();
            int selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                PanelsNavigation.Text = $"{panelsView.Cast<object>().Count()}";
            }
            else
            {
                PanelsNavigation.Text = $"{selectedIndex + 1} / {panelsView.Cast<object>().Count()}";
            }
        }
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                ItemsNavigation.Text = $"{itemsView.Cast<ModificationItem>().Where(i => !i.IsGhostRecord).Count()}";
            }
            else
            {
                ItemsNavigation.Text = $"{selectedIndex + 1} / {itemsView.Cast<ModificationItem>().Where(i => !i.IsGhostRecord).Count()}";
            }
        }
        private void Panels_SelectedChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Panels_CollectionChanged(null, null);
        }
        private void ItemsSelectedChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Items_CollectionChanged(null, null);
        }

        private void NewForm_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is ModificationPanel panel)
            {
                ModificationItem ghostRecord = itemsData.FirstOrDefault(i => i.PanelID == panel.PanelID && i.IsGhostRecord);
                if (ghostRecord != null)
                {
                    string message = $"New modification form already available!\n" +
                                     $"Check Form Id: {ghostRecord.ModificationID}.";
                    MessageWindow.Show("Add", message, MessageWindowButton.OK, MessageWindowImage.Information);
                    return;
                }

                int newID = 0;
                if (modifications.Count != 0)
                {
                    newID = modifications.Max(m => m.ID);
                }

                Modification newModification = new()
                {
                    ID = ++newID,
                    JobOrderID =
                    JobOrderData.ID,
                    Date = DateTime.Today
                };

                modifications.Add(newModification);
                ModificationsList.SelectedItem = newModification;

                ModificationItem ghostItem = new()
                {
                    ModificationID = newModification.ID,
                    PanelID = panel.PanelID,
                    IsGhostRecord = true,
                    Date = DateTime.Today,
                };
                itemsData.Add(ghostItem);
            }
        }

        private void NewItem_Clicked(object sender, RoutedEventArgs e)
        {
            //if (PanelsList.SelectedItem is ModificationPanel panel)
            //{
            //    Items_Standard_Window window = new Items_Standard_Window()
            //    {
            //        PanelID = panel.PanelID,
            //        ItemData = null,
            //        ActionData = Actions.New,
            //        ModificationData = ModificationsList.SelectedItem as Modification,
            //        ItemsData = itemsData,
            //        ReferencesData = referencesData,
            //        JobOrderItemsData = jobOrderItems,
            //    };
            //    _ = window.ShowDialog();
            //}
        }
        private void ReturnItem_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

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


        private void DeleteModification_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void Items_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }


        #region Filters
        private bool DataFilter(object item)
        {
            if (PanelsList.SelectedItem is ModificationPanel panel)
            {
                bool result = true;
                int value = (item as ModificationItem).PanelID;
                int checkValue = panel.PanelID;

                if (value != checkValue)
                {
                    result = false;
                }

                return result;
            }
            else
            {
                return false;
            }
        }


        #endregion


    }
}
