using Dapper;

using ProjectsNow.AttachedProperties;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Printing;
using ProjectsNow.Printing.JobOrderPages;
using ProjectsNow.Views;
using ProjectsNow.Windows.MessageWindows;

using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectsNow.Services
{
    public static class WarrantyCertificateServices
    {
        private static Grid Table { get; set; }
        private static Border NameBorder { get; set; }
        private static TextBlock Name { get; set; }

        static void UpdateUI()
        {
            List<UIElement> elements = new()
            {
                Name,
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
        private static void NewTable()
        {
            Table = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(560) });
            Table.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
        }
        private static void NewCell(WPanel panel)
        {
            Name = new TextBlock()
            {
                Text = panel.Name,
                FontFamily = new FontFamily("Calibri (Body)"),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(5, 2, 5, 2),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            NameBorder = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = Name,
            };

            Grid.SetRow(NameBorder, Table.RowDefinitions.Count);
            Table.RowDefinitions.Add(new RowDefinition() { MinHeight = 30 });
            Table.Children.Add(NameBorder);
        }

        public static void PrintWarranty(int warrantyId, IView checkPoint = null)
        {
            //MessageBoxResult result = MessageWindow.Show("Printing",
            //                                             "Print with watermark?",
            //                                             MessageWindowButton.YesNo,
            //                                             MessageWindowImage.Question);

            Warranty warranty;
            List<WPanel> panels;
            WarrantyCertificateForm warrantyForm;
            WarrantyCertificatePanelsForm panelsForm;
            WarrantyCertificateDetailsForm warrantyDetailsForm;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query;
                query = $"Select * From [JobOrder].[Warranties(View)] " +
                        $"Where Id  = {warrantyId}";
                warranty = connection.QueryFirstOrDefault<Warranty>(query);

                query = $"Select * From [JobOrder].[WarrantiesPanels(View)] " +
                        $"Where WarrantyId = {warrantyId} " +
                        $"Order By SN";
                panels = connection.Query<WPanel>(query).ToList();
            }

            foreach (WPanel panel in panels)
            {
                panel.SN = panels.IndexOf(panel) + 1;
            }

            List<List<WPanel>> pagePanels = new() { new List<WPanel>() };

            int page = 1;
            double maxHeight = 750; //old one (810) - 1cm

            int pagesNumber;

            NewTable();
            foreach (WPanel panel in panels)
            {
                NewCell(panel);
                UpdateUI();

                var lines = Name.GetLines().ToList();
                bool isMultiLine = lines.Count > 1;

            Panel:

                int nameLines = lines.Count;
                if (nameLines >= 1 && isMultiLine)
                {
                    Name.Text = "";
                    UpdateUI();

                    while (Table.ActualHeight + 25 < maxHeight)
                    {
                        if (lines.Count == 0)
                            break;

                        if (Name.Text == "")
                            Name.Text += lines[0];
                        else
                            Name.Text += $"\n{lines[0]}";

                        UpdateUI();
                        lines.Remove(lines[0]);
                    }
                }

                if ((Table.ActualHeight + 25) > maxHeight)
                {
                    WPanel newPanel = new();
                    newPanel.Update(panel);

                    if (!isMultiLine)
                    {
                        pagePanels[page - 1].Add(newPanel);

                        page++;
                        pagePanels.Add(new List<WPanel>());
                        NewTable();
                    }
                    else
                    {
                        if (lines.Count != 0)
                        {
                            newPanel.Name = Name.Text;
                            pagePanels[page - 1].Add(newPanel);

                            page++;
                            pagePanels.Add(new List<WPanel>());
                            NewTable();

                            goto Panel;
                        }
                    }
                }
                else
                {
                    WPanel newPanel = new();
                    newPanel.Update(panel);
                    pagePanels[page - 1].Add(newPanel);

                    if (isMultiLine)
                    {
                        newPanel.Name = Name.Text;
                    }
                }
            }

            if (panels.Count != 0)
            {
                pagesNumber = pagePanels.Count + 2;
                List<FrameworkElement> elements = new();
                warrantyForm = new WarrantyCertificateForm(warranty);
                warrantyForm.SetPage(1, pagesNumber);
                elements.Add(warrantyForm);

                for (int i = 1; i <= pagePanels.Count; i++)
                {
                    panelsForm = new WarrantyCertificatePanelsForm(pagePanels[i - 1]);
                    panelsForm.SetPage(i + 1, pagesNumber);

                    //if (result == MessageBoxResult.Yes)
                    //{
                    //    PanelsForm.BackgroundImage.Source = AppData.CompanyWatermark;
                    //}

                    elements.Add(panelsForm);
                }

                warrantyDetailsForm = new WarrantyCertificateDetailsForm(warranty);
                warrantyDetailsForm.SetPage(pagesNumber, pagesNumber);
                elements.Add(warrantyDetailsForm);

                Print.PrintPreview(elements, $"Warranty-{warranty.Code}", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no panels!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }

        }
    }
}
