using Dapper;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Events;
using ProjectsNow.Services;

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
using System.Windows.Media;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Delivery_Windows
{
    public partial class DeliveryWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public ObservableCollection<JPanel> PanelsData { get; set; }

        private ObservableCollection<Delivery> deliveries;
        private ObservableCollection<JPanel> tempPanelsData;
        private ObservableCollection<TransactionPanel> panelsTransaction;
        public DeliveryWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [JobOrder].[Panels(Delivered)] " +
                               $"Where JobOrderID  = {JobOrderData.ID} " +
                               $"Order By PanelSN";
                panelsTransaction = new ObservableCollection<TransactionPanel>(connection.Query<TransactionPanel>(query));
            }

            deliveries = new ObservableCollection<Delivery>(panelsTransaction.GroupBy(i => i.Reference, ii => ii.Date).Select(m => new Delivery { Number = m.Key, Date = m.ToList()[0] }).OrderByDescending(m => m.Number));

            viewDataDeliveries = new CollectionViewSource() { Source = deliveries };
            viewDataPanels = new CollectionViewSource() { Source = panelsTransaction };

            viewDataPanels.Filter += DataFilter;
            DeliveriesList.ItemsSource = viewDataDeliveries.View;
            PanelsList.ItemsSource = viewDataPanels.View;

            viewDataDeliveries.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DeliveriesCollectionChanged);
            viewDataPanels.View.CollectionChanged += new NotifyCollectionChangedEventHandler(PanelsCollectionChanged);

            if (viewDataDeliveries.View.Cast<object>().Count() == 0)
            {
                DeliveriesCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            if (viewDataPanels.View.Cast<object>().Count() == 0)
            {
                PanelsCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            tempPanelsData = new ObservableCollection<JPanel>();
            foreach (JPanel panel in PanelsData)
            {
                JPanel newPanel = new();
                newPanel.Update(panel);
                tempPanelsData.Add(newPanel);
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
        private void DeliveriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = DeliveriesList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationDeliveries.Text = $"Deliveries: {viewDataDeliveries.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationDeliveries.Text = $"Delivery: {selectedIndex + 1} / {viewDataDeliveries.View.Cast<object>().Count()}";
            }
        }
        private void PanelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewDataPanels.View.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
            }
        }
        private void DeliveriesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataPanels.View.Refresh();
            int selectedIndex = DeliveriesList.SelectedIndex;
            if (selectedIndex == -1)
            {
                NavigationDeliveries.Text = $"Deliveries: {viewDataDeliveries.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationDeliveries.Text = $"Delivery: {selectedIndex + 1} / {viewDataDeliveries.View.Cast<object>().Count()}";
            }
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
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

        #region Filters
        private CollectionViewSource viewDataDeliveries;
        private CollectionViewSource viewDataPanels;
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is TransactionPanel panel)
                {
                    if (DeliveriesList.SelectedItem is Delivery delivey)
                    {
                        if (delivey.Number != panel.Reference)
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
        #endregion

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Visible;
            ControlLock2.Visibility = Visibility.Collapsed;

            int deliveryNumber;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select IsNull(DeliveryNumber,0) " +
                               $"From [JobOrder].[DeliveryNumber] " +
                               $"Where Year = {DateTime.Now.Year}";
                deliveryNumber = connection.QueryFirstOrDefault<int>(query) + 1;
            }

            Delivery newDelivery = new() { Number = $"{(DateTime.Now.Year - Database.CompanyCreationYear - 5) * 1000 + deliveryNumber}{DateTime.Now.Month:00}{DateTime.Now.Year.ToString().Substring(2, 2)}", Date = DateTime.Now };
            deliveries.Insert(0, newDelivery);
            DeliveriesList.SelectedItem = newDelivery;

            LoadingControl.Visibility = Visibility.Visible;

            PanelsList.ContextMenu = (ContextMenu)Resources["PanelsListContextMenu"];
            PanelsList.RowStyle = (Style)Resources["NewPanels"];
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveriesList.SelectedItem is Delivery delivery)
            {
                LoadingControl.Visibility = Visibility.Visible;
                ShowEvent.Do();
                ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Collapsed;
                ControlLock2.Visibility = Visibility.Visible;

                string query;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    foreach (TransactionPanel panel in panelsTransaction.Where(i => i.Reference == delivery.Number))
                    {
                        query = $"Insert Into [JobOrder].[PanelsTransactions] " +
                                $"(PanelID, Reference, Qty, Date, Action, JobOrderID) " +
                                $"Values " +
                                $"(@PanelID, @Reference, @Qty, @Date, 'Delivered', @JobOrderID) Select @@IDENTITY";
                        panel.TransactionID = (int)(decimal)connection.ExecuteScalar(query, panel);

                        JPanel panelData = PanelsData.FirstOrDefault(i => i.PanelID == panel.PanelID);
                        panelData.DeliveredQty += panel.Qty;

                        if (panelData.PanelQty == panelData.DeliveredQty)
                        {
                            panelData.Status = Statuses.Delivered.ToString();
                            _ = connection.Execute($"Update [JobOrder].[Panels] Set Status ='{Statuses.Delivered}' Where PanelID = {panelData.PanelID}");
                        }
                    }
                }

                PanelsList.ContextMenu = null;
                PanelsList.RowStyle = (Style)Resources["Panels"];

                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveriesList.SelectedItem is Delivery delivery)
            {
                LoadingControl.Visibility = Visibility.Visible;
                ShowEvent.Do();

                ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Collapsed;
                ControlLock2.Visibility = Visibility.Visible;

                List<TransactionPanel> list = new();
                foreach (TransactionPanel panel in panelsTransaction.Where(i => i.Reference == delivery.Number))
                {
                    JPanel panelData = tempPanelsData.FirstOrDefault(i => i.PanelID == panel.PanelID);
                    panelData.DeliveredQty += panel.Qty * -1;
                    list.Add(panel);
                }

                foreach (TransactionPanel panel in list)
                {
                    _ = panelsTransaction.Remove(panel);
                }

                _ = deliveries.Remove(delivery);

                PanelsList.ContextMenu = null;
                PanelsList.RowStyle = (Style)Resources["Panels"];

                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveriesList.SelectedItem is Delivery deliveryData)
            {
                DeliveryNoteSerices.Print(JobOrderData.ID, deliveryData, null);
                //    MessageBoxResult result = MessageWindow.Show("Printing", "Print with watermark?", MessageWindowButton.YesNo, MessageWindowImage.Question);

                //    string query;
                //    List<DPanel> panels;
                //    DeliveryInfromation deliveryInfromation;
                //    List<int> deliveriesNumbers = new List<int>();
                //    using (SqlConnection connection = new SqlConnection(Database.ConnectionString))
                //    {
                //        query = $"Select * From [JobOrder].[DeliveriesInformations] Where JObOrderId = {JobOrderData.ID} And DeliveryNumber = {deliveryData.Number}";
                //        deliveryInfromation = connection.QueryFirstOrDefault<DeliveryInfromation>(query);

                //        query = $"Select * From [JobOrder].[Panels(DeliveryDetails)] Where JObOrderId = {JobOrderData.ID} And DeliveryNumber = {deliveryData.Number}";
                //        panels = connection.Query<DPanel>(query).ToList();
                //    }

                //    foreach (Delivery delivery in deliveries)
                //    {
                //        deliveriesNumbers.Add(int.Parse(delivery.Number.Substring(0, 3)));
                //    }

                //    deliveriesNumbers.Sort();

                //    deliveryInfromation.ShipmentNo = deliveriesNumbers.IndexOf(int.Parse(deliveryData.Number.Substring(0, 3))) + 1;

                //    foreach (DPanel panel in panels)
                //    {
                //        panel.PreviousQty = panelsTransaction.Where(p => p.PanelID == panel.PanelID && int.Parse(p.Reference) < int.Parse(panel.DeliveryNumber)).Sum(p => p.Qty);
                //    }

                //    for (int i = 1; i <= panels.Count; i++)
                //    {
                //        panels[i - 1].PanelSN = i;
                //    }

                //    List<List<DPanel>> pagePanels = new List<List<DPanel>>() { new List<DPanel>() };

                //    int page = 1;
                //    int pagesNumber;
                //    double maxTableHeight = 350;

                //    NewTable();
                //    foreach (DPanel panel in panels)
                //    {
                //        NewCell(panel);
                //        UpdateUI();

                //        var lines = EnglishName.GetLines().ToList();
                //        bool isMultiLine = lines.Count > 1;

                //    Panel:

                //        int nameLines = lines.Count;
                //        if (nameLines >= 1 && isMultiLine)
                //        {
                //            EnglishName.Text = "";
                //            UpdateUI();

                //            while (Table.ActualHeight + 25 < maxTableHeight)
                //            {
                //                if (lines.Count == 0)
                //                    break;

                //                if (EnglishName.Text == "")
                //                    EnglishName.Text += lines[0];
                //                else
                //                    EnglishName.Text += $"\n{lines[0]}";

                //                UpdateUI();
                //                lines.Remove(lines[0]);
                //            }
                //        }

                //        if (Table.ActualHeight + 20 > maxTableHeight)
                //        {
                //            DPanel newPanel = new DPanel();
                //            newPanel.Update(panel);

                //            if (!isMultiLine)
                //            {
                //                pagePanels[page - 1].Add(newPanel);

                //                page++;
                //                pagePanels.Add(new List<DPanel>());
                //                NewTable();
                //            }
                //            else
                //            {
                //                if (lines.Count != 0)
                //                {
                //                    newPanel.PanelName = EnglishName.Text;
                //                    pagePanels[page - 1].Add(newPanel);

                //                    page++;
                //                    pagePanels.Add(new List<DPanel>());
                //                    NewTable();

                //                    goto Panel;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            DPanel newPanel = new DPanel();
                //            newPanel.Update(panel);
                //            pagePanels[page - 1].Add(newPanel);

                //            if (isMultiLine)
                //            {
                //                newPanel.PanelName = EnglishName.Text;
                //            }
                //        }
                //    }

                //    if (panels.Count != 0)
                //    {
                //        pagesNumber = pagePanels.Count;
                //        List<FrameworkElement> elements = new List<FrameworkElement>();
                //        for (int i = 1; i <= pagesNumber; i++)
                //        {
                //            DeliveryForm deliveryForm = new DeliveryForm()
                //            {
                //                Page = i,
                //                Pages = Convert.ToInt32(pagesNumber),
                //                DeliveryInfromation = deliveryInfromation,
                //                PanelsData = pagePanels[i - 1]
                //            };

                //            if (result == MessageBoxResult.Yes)
                //            {
                //                deliveryForm.BackgroundImage.Visibility = Visibility.Visible;
                //            }

                //            elements.Add(deliveryForm);
                //        }

                //        Print.PrintPreview(elements, $"Delivery-{deliveryData.Number}");
                //    }
                //    else
                //    {
                //        _ = MessageWindow.Show("Items", "There is no panels!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                //    }
            }
        }
        private void AddPanels_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveriesList.SelectedItem is Delivery delivery)
            {
                PanelsWindow window = new()
                {
                    DeliveryData = delivery,
                    PanelsData = tempPanelsData,
                    PanelsTransaction = panelsTransaction,
                };
                _ = window.ShowDialog();
            }
        }
        private void DeletePanels_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is TransactionPanel panel)
            {
                JPanel panelData = tempPanelsData.FirstOrDefault(i => i.PanelID == panel.PanelID);
                panelData.DeliveredQty += panel.Qty * -1;
                _ = panelsTransaction.Remove(panel);
            }
        }

        //static void UpdateUI()
        //{
        //    List<UIElement> elements = new List<UIElement>()
        //    {
        //        EnglishName,
        //        NameBorder,
        //        Table,
        //    };

        //    foreach (UIElement element in elements)
        //    {
        //        if (element == null)
        //            continue;

        //        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //        element.Arrange(new Rect(element.DesiredSize));
        //    }
        //}
        //private static Grid Table { get; set; }
        //private static Border NameBorder { get; set; }
        //private static TextBlock EnglishName { get; set; }
        //private static void NewTable()
        //{
        //    Table = new Grid()
        //    {
        //        HorizontalAlignment = HorizontalAlignment.Center
        //    };
        //    Table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(226.772) });

        //    Table.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(23) });
        //}
        //private static void NewCell(DPanel panel)
        //{
        //    EnglishName = new TextBlock()
        //    {
        //        Text = panel.PanelName,
        //        FontFamily = new FontFamily("Calibri (Body)"),
        //        FontWeight = FontWeights.Bold,
        //        FontSize = 14,
        //        Margin = new Thickness(5, 2, 5, 0),
        //        TextWrapping = TextWrapping.Wrap,
        //        VerticalAlignment = VerticalAlignment.Center,
        //        HorizontalAlignment = HorizontalAlignment.Left,
        //    };

        //    NameBorder = new Border()
        //    {
        //        BorderBrush = Brushes.Black,
        //        BorderThickness = new Thickness(0, 0, 1, 1),
        //        Child = EnglishName,
        //    };

        //    Grid.SetRow(NameBorder, Table.RowDefinitions.Count);
        //    Table.RowDefinitions.Add(new RowDefinition() { MinHeight = 20 });
        //    Table.Children.Add(NameBorder);
        //}
    }
}
