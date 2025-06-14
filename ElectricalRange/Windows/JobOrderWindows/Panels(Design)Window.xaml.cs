using Dapper;

using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Windows.MessageWindows;

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
    public partial class Panels_Design_Window : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public ObservableCollection<JPanel> PanelsData { get; set; }

        private readonly string filter1 = Statuses.New.ToString();
        public Panels_Design_Window()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewData1 = new CollectionViewSource() { Source = PanelsData };
            viewData2 = new CollectionViewSource() { Source = PanelsData };

            viewData1.Filter += Data1Filter;
            viewData2.Filter += Data2Filter;

            List1.ItemsSource = viewData1.View;
            List2.ItemsSource = viewData2.View;

            viewData1.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            viewData2.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (viewData1.View.Cast<object>().Count() == 0 || viewData2.View.Cast<object>().Count() == 0)
            {
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            DataContext = new { JobOrderData, UserData };
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

        private void Action_Click(object sender, RoutedEventArgs e)
        {
            if (List1.SelectedItem is JPanel panelData)
            {
                panelData.Status = Statuses.Designing.ToString();
                panelData.DateOfDesign = DateTime.Now;

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Update [JobOrder].[Panels] Set " +
                                   $"Status = @Status , " +
                                   $"DateOfDesign = @DateOfDesign " +
                                   $"Where PanelID = @PanelID";

                    _ = connection.Execute(query, panelData);
                }
                viewData1.View.Refresh();
                viewData2.View.Refresh();
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (List2.SelectedItem is JPanel panelData)
            {
                if (panelData.Status == Statuses.Designing.ToString())
                {
                    panelData.Status = Statuses.New.ToString();
                    panelData.DateOfDesign = null;

                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        string query = $"Update [JobOrder].[Panels] Set " +
                                       $"Status = @Status , " +
                                       $"DateOfDesign = @DateOfDesign " +
                                       $"Where PanelID = @PanelID";

                        _ = connection.Execute(query, panelData);
                    }
                    viewData1.View.Refresh();
                    viewData2.View.Refresh();
                }
                else
                {
                    if (panelData.Status == Statuses.Waiting_Approval.ToString())
                    {
                        _ = MessageWindow.Show("Panels", "The panel is awaiting approval!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    }
                    else if (panelData.Status == Statuses.Production.ToString())
                    {
                        _ = MessageWindow.Show("Panels", "The panel under fabrication!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    }
                    else if (panelData.Status == Statuses.Closed.ToString())
                    {
                        _ = MessageWindow.Show("Panels", "The panel closed!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    }
                    else if (panelData.Status == Statuses.Hold.ToString())
                    {
                        _ = MessageWindow.Show("Panels", "The panel is hold!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    }
                    else if (panelData.Status == Statuses.Canceled.ToString())
                    {
                        _ = MessageWindow.Show("Panels", "The panel is Canceled!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    }
                }
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = List1.SelectedIndex;
            if (selectedIndex == -1)
            {
                Navigation1.Text = $"Panels: {viewData1.View.Cast<object>().Count()}";
            }
            else
            {
                Navigation1.Text = $"Panel: {selectedIndex + 1} / {viewData1.View.Cast<object>().Count()}";
            }

            selectedIndex = List2.SelectedIndex;
            if (selectedIndex == -1)
            {
                Navigation2.Text = $"Panels: {viewData2.View.Cast<object>().Count()}";
            }
            else
            {
                Navigation2.Text = $"Panel: {selectedIndex + 1} / {viewData2.View.Cast<object>().Count()}";
            }
        }

        private void List1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int selectedIndex = List1.SelectedIndex;
            if (selectedIndex == -1)
            {
                Navigation1.Text = $"Panels: {viewData1.View.Cast<object>().Count()}";
            }
            else
            {
                Navigation1.Text = $"Panel: {selectedIndex + 1} / {viewData1.View.Cast<object>().Count()}";
            }
        }

        private void List2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int selectedIndex = List2.SelectedIndex;
            if (selectedIndex == -1)
            {
                Navigation2.Text = $"Panels: {viewData2.View.Cast<object>().Count()}";
            }
            else
            {
                Navigation2.Text = $"Panel: {selectedIndex + 1} / {viewData2.View.Cast<object>().Count()}";
            }
        }

        #region Filters

        private CollectionViewSource viewData1;
        private CollectionViewSource viewData2;
        private readonly List<PropertyInfo> filterProperties = new()
        {
            typeof(JPanel).GetProperty("Status"),
        };
        private void Data1Filter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JPanel record)
                {
                    string columnName;
                    foreach (PropertyInfo property in filterProperties)
                    {
                        columnName = property.Name;
                        string value;
                        value = $"{record.GetType().GetProperty(columnName).GetValue(record)}";

                        if (value != filter1)
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

        private void Data2Filter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JPanel panel)
                {
                    if (panel.DateOfDesign == null)
                    {
                        e.Accepted = false;
                        return;
                    }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }
        #endregion

    }
}
