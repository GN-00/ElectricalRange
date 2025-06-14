using Dapper;

using ProjectsNow.AttachedProperties;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Printing;
using ProjectsNow.Views;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectsNow.Services
{
    public static class DeliveryNoteSerices
    {
        static void UpdateUI()
        {
            List<UIElement> elements = new()
            {
                EnglishName,
                NameBorder,
                Table,
            };

            foreach (UIElement element in elements)
            {
                if (element == null)
                    continue;

                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                element.Arrange(new Rect(element.DesiredSize));
            }
        }
        private static Grid Table { get; set; }
        private static Border NameBorder { get; set; }
        private static TextBlock EnglishName { get; set; }
        private static void NewTable()
        {
            Table = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(226.772) });

            Table.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(23) });
        }
        private static void NewCell(DPanel panel)
        {
            EnglishName = new TextBlock()
            {
                Text = panel.PanelName,
                FontFamily = new FontFamily("Calibri (Body)"),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(5, 2, 5, 0),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            NameBorder = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = EnglishName,
            };

            Grid.SetRow(NameBorder, Table.RowDefinitions.Count);
            Table.RowDefinitions.Add(new RowDefinition() { MinHeight = 20 });
            Table.Children.Add(NameBorder);
        }

        public static void Print(int JobOrderId, Delivery deliveryData, IView checkPoint)
        {
            MessageBoxResult result = MessageWindow.Show("Printing",
                                                         "Print with watermark?",
                                                         MessageWindowButton.YesNo,
                                                         MessageWindowImage.Question);

            string query;
            List<DPanel> panels;
            DeliveryInfromation deliveryInfromation;
            List<int> deliveriesNumbers = new();
            ObservableCollection<Delivery> deliveries;
            ObservableCollection<TransactionPanel> panelsTransaction;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [JobOrder].[Panels(Delivered)] " +
                        $"Where JobOrderID  = {JobOrderId} " +
                        $"Order By PanelSN";
                panelsTransaction = new ObservableCollection<TransactionPanel>(connection.Query<TransactionPanel>(query));
                deliveries = new ObservableCollection<Delivery>(panelsTransaction.GroupBy(i => i.Reference, ii => ii.Date).Select(m => new Delivery { Number = m.Key, Date = m.ToList()[0] }).OrderByDescending(m => m.Number));

                query = $"Select * From [JobOrder].[DeliveriesInformations] " +
                        $"Where JObOrderId = {JobOrderId} " +
                        $"And DeliveryNumber = '{deliveryData.Number}'";
                deliveryInfromation = connection.QueryFirstOrDefault<DeliveryInfromation>(query);

                query = $"Select * From [JobOrder].[Panels(DeliveryDetails)] " +
                        $"Where JObOrderId = {JobOrderId} " +
                        $"And DeliveryNumber = '{deliveryData.Number}'";
                panels = connection.Query<DPanel>(query).ToList();
            }

            foreach (Delivery delivery in deliveries)
            {
                deliveriesNumbers.Add(int.Parse(delivery.Number.Substring(0, 3)));
            }

            deliveriesNumbers.Sort();

            deliveryInfromation.ShipmentNo = deliveriesNumbers.IndexOf(int.Parse(deliveryData.Number.Substring(0, 3))) + 1;

            foreach (DPanel panel in panels)
            {
                panel.PreviousQty = panelsTransaction.Where(p => p.PanelID == panel.PanelID && int.Parse(p.Reference) < int.Parse(panel.DeliveryNumber)).Sum(p => p.Qty);
            }

            for (int i = 1; i <= panels.Count; i++)
            {
                panels[i - 1].PanelSN = i;
            }

            List<List<DPanel>> pagePanels = new() { new List<DPanel>() };

            int page = 1;
            int pagesNumber;
            double maxTableHeight = 350;

            NewTable();
            foreach (DPanel panel in panels)
            {
                NewCell(panel);
                UpdateUI();

                var lines = EnglishName.GetLines().ToList();
                bool isMultiLine = lines.Count > 1;

            Panel:

                int nameLines = lines.Count;
                if (nameLines >= 1 && isMultiLine)
                {
                    EnglishName.Text = "";
                    UpdateUI();

                    while (Table.ActualHeight + 20 < maxTableHeight)
                    {
                        if (lines.Count == 0)
                            break;

                        if (EnglishName.Text == "")
                            EnglishName.Text += lines[0];
                        else
                            EnglishName.Text += $"\n{lines[0]}";

                        UpdateUI();
                        lines.Remove(lines[0]);
                    }
                }

                if (Table.ActualHeight + 20 > maxTableHeight)
                {
                    DPanel newPanel = new();
                    newPanel.Update(panel);

                    if (!isMultiLine)
                    {
                        pagePanels[page - 1].Add(newPanel);

                        page++;
                        pagePanels.Add(new List<DPanel>());
                        NewTable();
                        NewCell(panel);
                    }
                    else
                    {
                        if (lines.Count != 0)
                        {
                            if (string.IsNullOrWhiteSpace(EnglishName.Text))
                            {
                                page++;
                                pagePanels.Add(new List<DPanel>());
                                NewTable();
                                NewCell(panel);
                            }
                            else
                            {
                                newPanel.PanelName = EnglishName.Text;
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<DPanel>());
                                NewTable();
                                NewCell(panel);
                            }

                            goto Panel;
                        }
                        else
                        {
                            newPanel = new DPanel();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);
                        }
                    }
                }
                else
                {
                    DPanel newPanel = new();
                    newPanel.Update(panel);
                    pagePanels[page - 1].Add(newPanel);

                    if (isMultiLine)
                    {
                        newPanel.PanelName = EnglishName.Text;
                    }
                }
            }

            if (panels.Count != 0)
            {
                pagesNumber = pagePanels.Count;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    DeliveryForm deliveryForm = new()
                    {
                        Page = i,
                        Pages = Convert.ToInt32(pagesNumber),
                        DeliveryInfromation = deliveryInfromation,
                        PanelsData = pagePanels[i - 1]
                    };

                    if (result == MessageBoxResult.Yes)
                    {
                        deliveryForm.BackgroundImage.Visibility = Visibility.Visible;
                    }

                    elements.Add(deliveryForm);
                }

                Printing.Print.PrintPreview(elements, $"Delivery-{deliveryData.Number}", checkPoint);
            }
            else
            {
                _ = MessageView.Show("Items", "There is no panels!!", MessageViewButton.OK, MessageViewImage.Warning);
            }
        }
    }
}
