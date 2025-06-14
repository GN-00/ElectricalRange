using Dapper;

using ProjectsNow.AttachedProperties;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Printing;
using ProjectsNow.Printing.Finance;
using ProjectsNow.Printing.SpecialInvoices;
using ProjectsNow.Views;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Invoice = ProjectsNow.Data.Finance.Invoice;

namespace ProjectsNow.Services
{
    public static class JobOrdersInvoicesServices
    {
        private static bool IsNormalForm { get; set; }
        private static Grid Table { get; set; }
        private static Grid NameCell { get; set; }
        private static Border NameBorder { get; set; }
        private static TextBlock EnglishName { get; set; }
        private static TextBlock ArabicName { get; set; }

        static void UpdateUI()
        {
            List<UIElement> elements = new()
            {
                EnglishName,
                ArabicName,
                NameCell,
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

            if (IsNormalForm)
                Table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
            else
                Table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(320) });

            Table.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
        }
        private static void NewCell(InvoiceItem panel)
        {
            EnglishName = new TextBlock()
            {
                Text = panel.Description,
                FontFamily = new FontFamily("Calibri (Body)"),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(5, 2, 5, 0),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            ArabicName = new TextBlock()
            {
                Text = "A",
                FontFamily = new FontFamily("Calibri (Body)"),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(5, 2, 5, 2),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            Grid.SetRow(ArabicName, 1);
            NameCell = new Grid();
            NameCell.RowDefinitions.Add(new RowDefinition());
            NameCell.RowDefinitions.Add(new RowDefinition());
            NameCell.Children.Add(EnglishName);
            NameCell.Children.Add(ArabicName);
            NameBorder = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = NameCell,
            };

            Grid.SetRow(NameBorder, Table.RowDefinitions.Count);
            Table.RowDefinitions.Add(new RowDefinition() { MinHeight = 42 });
            Table.Children.Add(NameBorder);
        }
        private static void NewCell(ProformaInvoicePanel panel)
        {
            EnglishName = new TextBlock()
            {
                Text = panel.Description,
                FontFamily = new FontFamily("Calibri (Body)"),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(5, 2, 5, 0),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            ArabicName = new TextBlock()
            {
                Text = "A",
                FontFamily = new FontFamily("Calibri (Body)"),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(5, 2, 5, 2),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            Grid.SetRow(ArabicName, 1);
            NameCell = new Grid();
            NameCell.RowDefinitions.Add(new RowDefinition());
            NameCell.RowDefinitions.Add(new RowDefinition());
            NameCell.Children.Add(EnglishName);
            NameCell.Children.Add(ArabicName);
            NameBorder = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = NameCell,
            };

            Grid.SetRow(NameBorder, Table.RowDefinitions.Count);
            Table.RowDefinitions.Add(new RowDefinition() { MinHeight = 42 });
            Table.Children.Add(NameBorder);
        }
        public static void PrintInvoice(string invoiceNumber, IView checkPoint = null)
        {
            //bool pagesEidt = false;

            if (invoiceNumber == "202209067" ||
                invoiceNumber == "202209068" ||
                invoiceNumber == "202209069" ||
                invoiceNumber == "202211097" ||
                invoiceNumber == "202211100" ||
                invoiceNumber == "202211114")
            {
                IsNormalForm = false;
                PrintInvoiceJO7059_7_22(invoiceNumber, checkPoint);
                return;
            }

            if (invoiceNumber == "202210081" ||
                invoiceNumber == "202211104" ||
                invoiceNumber == "202211106")
            {
                IsNormalForm = false;
                PrintInvoiceJO7076_8_22(invoiceNumber, checkPoint);
                return;
            }


            if (invoiceNumber == "202406115")
            {
                IsNormalForm = false;
                PrintingInvoice202406115Form(invoiceNumber, checkPoint);
                return;
            }

            IsNormalForm = true;
            MessageBoxResult result = MessageWindow.Show("Printing",
                                                         "Print with watermark?",
                                                         MessageWindowButton.YesNo,
                                                         MessageWindowImage.Question);

            Invoice invoice;
            List<InvoiceItem> panels;
            PanelsInvoiceForm invoiceForm;
            //List<InvoiceEdit> edits;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query;
                query = $"Select * From [Finance].[CustomersInvoices(View)] Where Code  = '{invoiceNumber}'";
                invoice = connection.QueryFirstOrDefault<Invoice>(query);

                query = $"Select * From [Finance].[CustomersInvoicesItems] Where InvoiceId = {invoice.Id}";
                panels = connection.Query<InvoiceItem>(query).ToList();

                //query = $"Select * From [Finance].[CustomersInvoicesEdits] Where InvoiceId = {invoice.Id}";
                //edits = connection.Query<InvoiceEdit>(query).ToList();
            }

            if (invoice.IsReturn)
            {
                invoice.NetPrice *= -1;
                invoice.VATValue *= -1;
                invoice.GrossPrice *= -1;

                foreach (InvoiceItem item in panels)
                {
                    item.NetPrice *= -1;
                    item.UnitNetPrice *= -1;
                    item.GrossPrice *= -1;
                    item.UnitGrossPrice *= -1;
                    item.VATValue *= -1;
                    item.UnitVATValue *= -1;
                    item.UnitOriginalPrice *= -1;
                }
            }

            if (invoice.IsPercentageInvoice)
            {
                PercentageInvoice(invoice, panels, result, checkPoint);
                return;
            }

            List<List<InvoiceItem>> pagePanels = new() { new List<InvoiceItem>() };

            int page = 1;
            double maxHeight1FirstPage = 332;
            double maxHeight2FirstPage = 620;
            double maxHeight1 = 512;
            double maxHeight2 = 800;

            int pagesNumber;

            NewTable();
            int sn = 1;
            foreach (InvoiceItem panel in panels)
            {
                panel.SN = sn++;
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

                    //First Page Only
                    if (page == 1)
                    {
                        while (Table.ActualHeight + 25 < maxHeight2FirstPage)
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
                    else
                    {
                        while (Table.ActualHeight + 25 < maxHeight2)
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
                }

                //First Page Only
                if (page == 1)
                {
                    if (Table.ActualHeight > maxHeight1FirstPage)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2FirstPage)
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                                NewCell(panel);

                            }
                            else
                            {
                                newPanel.Description = EnglishName.Text;
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                                NewCell(panel);

                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }
                else
                {
                    if (Table.ActualHeight > maxHeight1)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2)
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                                NewCell(panel);
                            }
                            else
                            {
                                newPanel.Description = EnglishName.Text;
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                                NewCell(panel);

                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }
            }

            int TableEndPage = pagePanels.Count();
            List<InvoiceItem> lastPage = pagePanels.Last();
            if (page == 1)
            {
                if (Table.ActualHeight > maxHeight1FirstPage)
                {

                    if (lastPage.Count != 1)
                    {
                        if (Table.ActualHeight > maxHeight2FirstPage)
                        {
                            InvoiceItem lastPanel = pagePanels.Last().Last();
                            pagePanels.Last().Remove(lastPanel);
                            pagePanels.Add(new List<InvoiceItem>());
                            pagePanels.Last().Add(lastPanel);

                            TableEndPage++;
                        }
                        else
                        {
                            pagePanels.Add(new List<InvoiceItem>());
                        }

                    }
                    else
                    {
                        pagePanels.Add(new List<InvoiceItem>());
                    }

                    page++;
                }
            }
            else
            {
                if (Table.ActualHeight > maxHeight1)
                {
                    if (lastPage.Count != 1)
                    {
                        InvoiceItem lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<InvoiceItem>());
                        pagePanels.Last().Add(lastPanel);

                        TableEndPage++;
                    }
                    else
                    {
                        pagePanels.Add(new List<InvoiceItem>());
                    }

                    page++;
                }
            }

            if (panels.Count != 0)
            {
                
                //if (edits.Count > 0)
                //    EditPages(pagePanels, panels, edits);

                pagesNumber = pagePanels.Count;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    if (i == pagesNumber)
                    {
                        invoiceForm = new PanelsInvoiceForm()
                        {
                            PageNumber = i,
                            TotalPages = pagesNumber,
                            NetPrice = invoice.NetPrice,
                            VATPercentage = invoice.VATPercentage,
                            VATValue = invoice.VATValue,
                            GrossPrice = invoice.GrossPrice,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };

                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }
                    else
                    {
                        invoiceForm = new PanelsInvoiceForm()
                        {
                            PageNumber = i,
                            TotalPages = pagesNumber,
                            VATPercentage = panels.Max(p => p.VAT) * 100,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };

                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }

                    if (i == TableEndPage)
                    {
                        invoiceForm.NetPrice = invoice.NetPrice;
                        invoiceForm.VATPercentage = invoice.VATPercentage;
                        invoiceForm.VATValue = invoice.VATValue;
                        invoiceForm.GrossPrice = invoice.GrossPrice;
                        invoiceForm.TableTotal = true;
                    }

                    elements.Add(invoiceForm);
                }

                Print.PrintPreview(elements, $"Invoice-{invoiceNumber}", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no panels!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }

        }

        

        //private static int EditPages(List<List<InvoiceItem>> pagePanels, List<InvoiceItem> panels, List<InvoiceEdit> edits)
        //{
        //    pagePanels = new List<List<InvoiceItem>>();
        //    foreach (InvoiceEdit edit in edits)
        //    {
        //        pagePanels.Add(new List<InvoiceItem>());

        //        for (int i = 0; i < edit.PagePanels; i++)
        //        {
        //            pagePanels.Add()
        //        }
        //    }

        //    return edits.Count;

        //}

        private static void PercentageInvoice(Invoice invoice, List<InvoiceItem> panels, MessageBoxResult result, IView checkPoint)
        {
            if (invoice.Code == "202412224")
            {
                PercentageInvoice202412224(invoice, panels,  result, checkPoint);
                return;
            }
            PanelsInvoicePercentageForm invoiceForm;

            List<List<InvoiceItem>> pagePanels = new() { new List<InvoiceItem>() };

            int page = 1;
            double maxHeight1FirstPage = 332;
            double maxHeight2FirstPage = 620;
            double maxHeight1 = 512;
            double maxHeight2 = 800;

            int pagesNumber;

            NewTable();
            int sn = 1;
            foreach (InvoiceItem panel in panels)
            {
                panel.SN = sn++;
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

                    //First Page Only
                    if (page == 1)
                    {
                        while (Table.ActualHeight + 25 < maxHeight2FirstPage)
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
                    else
                    {
                        while (Table.ActualHeight + 25 < maxHeight2)
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
                }

                //First Page Only
                if (page == 1)
                {
                    UpdateUI();
                    if (Table.ActualHeight > maxHeight1FirstPage)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2FirstPage)
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(EnglishName.Text))
                                {
                                    page++;
                                    pagePanels.Add(new List<InvoiceItem>());
                                    NewTable();
                                }
                                else
                                {
                                    newPanel.Description = EnglishName.Text;
                                    pagePanels[page - 1].Add(newPanel);

                                    page++;
                                    pagePanels.Add(new List<InvoiceItem>());
                                    NewTable();
                                }


                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }
                else
                {
                    if (Table.ActualHeight > maxHeight1)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2)
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(EnglishName.Text))
                                {
                                    page++;
                                    pagePanels.Add(new List<InvoiceItem>());
                                    NewTable();
                                }
                                else
                                {
                                    newPanel.Description = EnglishName.Text;
                                    pagePanels[page - 1].Add(newPanel);

                                    page++;
                                    pagePanels.Add(new List<InvoiceItem>());
                                    NewTable();
                                }

                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }

            }

            List<InvoiceItem> lastPage = pagePanels.Last();
            if (page == 1)
            {
                if (Table.ActualHeight > maxHeight1FirstPage)
                {

                    if (lastPage.Count != 1)
                    {
                        InvoiceItem lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<InvoiceItem>());
                        pagePanels.Last().Add(lastPanel);
                    }
                    else
                    {
                        pagePanels.Add(new List<InvoiceItem>());
                    }

                    page++;
                }
            }
            else
            {
                if (Table.ActualHeight > maxHeight1)
                {
                    if (lastPage.Count != 1)
                    {
                        InvoiceItem lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<InvoiceItem>());
                        pagePanels.Last().Add(lastPanel);
                    }
                    else
                    {
                        pagePanels.Add(new List<InvoiceItem>());
                    }

                    page++;
                }
            }


            if (panels.Count != 0)
            {
                pagesNumber = pagePanels.Count;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    if (i == pagesNumber)
                    {
                        invoiceForm = new PanelsInvoicePercentageForm()
                        {
                            Page = i,
                            Pages = pagesNumber,
                            NetPrice = invoice.NetPrice,
                            VATPercentage = invoice.VATPercentage,
                            VATValue = invoice.VATValue,
                            GrossPrice = invoice.GrossPrice,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };


                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }
                    else
                    {
                        invoiceForm = new PanelsInvoicePercentageForm()
                        {
                            Page = i,
                            Pages = pagesNumber,
                            VATPercentage = invoice.VATPercentage,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };

                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }

                    elements.Add(invoiceForm);
                }

                Print.PrintPreview(elements, $"Invoice-{invoice.Code}", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no panels!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }
        public static void PrintInvoiceJO7059_7_22(string invoiceNumber, IView checkPoint = null)
        {
            MessageBoxResult result = MessageWindow.Show("Printing",
                                                          "Print with watermark?",
                                                          MessageWindowButton.YesNo,
                                                          MessageWindowImage.Question);

            Printing.JobOrderPages.JO7059.InvoiceForm invoiceForm;
            Invoice invoice;
            List<InvoiceItem> panels;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query;
                query = $"Select * From [Finance].[CustomersInvoices(View)] Where Code  = {invoiceNumber}";
                invoice = connection.QueryFirstOrDefault<Invoice>(query);

                query = $"Select * From [Finance].[CustomersInvoicesItems] Where InvoiceId = {invoice.Id}";
                panels = connection.Query<InvoiceItem>(query).ToList();
            }

            List<List<InvoiceItem>> pagePanels = new() { new List<InvoiceItem>() };

            int page = 1;
            double maxHeight = 190;
            double maxHeight2 = 560;

            int pagesNumber;

            NewTable();
            int sn = 1;
            foreach (InvoiceItem panel in panels)
            {
                panel.SN = sn++;
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

                    while (Table.ActualHeight + 25 < maxHeight2)
                    {
                        if (lines.Count == 0)
                            break;

                        EnglishName.Text += lines[0];
                        UpdateUI();
                        lines.Remove(lines[0]);
                    }
                }

                if (Table.ActualHeight > maxHeight)
                {
                    if ((Table.ActualHeight + 25) > maxHeight2)
                    {
                        page++;
                        pagePanels.Add(new List<InvoiceItem>());
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        NewTable();

                        if (!isMultiLine)
                        {
                            pagePanels[page - 1].Add(newPanel);
                        }
                        else
                        {
                            if (lines.Count != 0)
                            {
                                goto Panel;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);
                    }
                }
                else
                {
                    InvoiceItem newPanel = new();
                    newPanel.Update(panel);
                    pagePanels[page - 1].Add(newPanel);
                }
            }

            if (Table.ActualHeight > maxHeight)
            {
                page++;
                pagePanels.Add(new List<InvoiceItem>());
            }

            if (panels.Count != 0)
            {
                pagesNumber = pagePanels.Count;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    if (i == pagesNumber)
                    {
                        invoiceForm = new Printing.JobOrderPages.JO7059.InvoiceForm()
                        {
                            VATPercentage = invoice.VATPercentage,
                            TotalCost = invoice.NetPrice,
                            TotalVAT = invoice.VATValue,
                            TotalPrice = invoice.GrossPrice,
                            Page = i,
                            Pages = pagesNumber,
                            InvoiceData = invoice,
                            PanelsData = pagePanels[i - 1],
                        };

                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        invoiceForm = new Printing.JobOrderPages.JO7059.InvoiceForm()
                        {
                            Page = i,
                            Pages = pagesNumber,
                            InvoiceData = invoice,
                            VATPercentage = invoice.VATPercentage,
                            PanelsData = pagePanels[i - 1],
                        };

                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Visibility = Visibility.Visible;
                        }
                    }

                    elements.Add(invoiceForm);
                }

                Print.PrintPreview(elements, $"Invoice-{invoiceNumber}", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no panels!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }

        }
        public static void PrintInvoiceJO7076_8_22(string invoiceNumber, IView checkPoint = null)
        {
            MessageBoxResult result = MessageWindow.Show("Printing", "Print with watermark?", MessageWindowButton.YesNo, MessageWindowImage.Question);
            Invoice invoice;
            List<InvoiceItem> panels;
            Printing.JobOrderPages.JO7076.InvoiceForm invoiceForm;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query;
                query = $"Select * From [Finance].[CustomersInvoices(View)] Where Code  = {invoiceNumber}";
                invoice = connection.QueryFirstOrDefault<Invoice>(query);

                query = $"Select * From [Finance].[CustomersInvoicesItems] Where InvoiceId = {invoice.Id}";
                panels = connection.Query<InvoiceItem>(query).ToList();
            }

            List<List<InvoiceItem>> pagePanels = new() { new List<InvoiceItem>() };

            int page = 1;
            double maxHeight1FirstPage = 332;
            double maxHeight2FirstPage = 620;
            double maxHeight1 = 512;
            double maxHeight2 = 800;

            int pagesNumber;

            NewTable();
            int sn = 1;
            foreach (InvoiceItem panel in panels)
            {
                panel.SN = sn++;
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

                    //First Page Only
                    if (page == 1)
                    {
                        while (Table.ActualHeight + 25 < maxHeight2FirstPage)
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
                    else
                    {
                        while (Table.ActualHeight + 25 < maxHeight2)
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
                }

                //First Page Only
                if (page == 1)
                {
                    if (Table.ActualHeight > maxHeight1FirstPage)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2FirstPage)
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                            }
                            else
                            {
                                newPanel.Description = EnglishName.Text;
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();

                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }
                else
                {
                    if (Table.ActualHeight > maxHeight1)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2)
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                            }
                            else
                            {
                                newPanel.Description = EnglishName.Text;
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();

                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }
            }

            List<InvoiceItem> lastPage = pagePanels.Last();
            if (page == 1)
            {
                if (Table.ActualHeight > maxHeight1FirstPage)
                {

                    if (lastPage.Count != 1)
                    {
                        InvoiceItem lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<InvoiceItem>());
                        pagePanels.Last().Add(lastPanel);
                    }
                    else
                    {
                        pagePanels.Add(new List<InvoiceItem>());
                    }

                    page++;
                }
            }
            else
            {
                if (Table.ActualHeight > maxHeight1)
                {
                    if (lastPage.Count != 1)
                    {
                        InvoiceItem lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<InvoiceItem>());
                        pagePanels.Last().Add(lastPanel);
                    }
                    else
                    {
                        pagePanels.Add(new List<InvoiceItem>());
                    }

                    page++;
                }
            }


            if (panels.Count != 0)
            {
                pagesNumber = pagePanels.Count;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    if (i == pagesNumber)
                    {
                        invoiceForm = new Printing.JobOrderPages.JO7076.InvoiceForm()
                        {
                            Page = i,
                            Pages = pagesNumber,
                            TotalCost = invoice.NetPrice,
                            VATPercentage = invoice.VATPercentage,
                            TotalVAT = invoice.VATValue,
                            TotalPrice = invoice.GrossPrice,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };


                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }
                    else
                    {
                        invoiceForm = new Printing.JobOrderPages.JO7076.InvoiceForm()
                        {
                            Page = i,
                            Pages = pagesNumber,
                            VATPercentage = invoice.VATPercentage,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };

                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }

                    elements.Add(invoiceForm);
                }

                Print.PrintPreview(elements, $"Invoice-{invoiceNumber}", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no panels!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }
        internal static void PrintProformaInvoice(int invoiceId, IView checkPoint)
        {
            MessageBoxResult result = MessageWindow.Show("Printing",
                                                         "Print with watermark?",
                                                         MessageWindowButton.YesNo,
                                                         MessageWindowImage.Question);

            ProformaInvoice invoice;
            List<ProformaInvoicePanel> panels;
            ProformaInvoiceForm invoiceForm;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query;
                query = $"Select * From [Finance].[ProformaInvoices] Where Id  = '{invoiceId}'";
                invoice = connection.QueryFirstOrDefault<ProformaInvoice>(query);

                query = $"Select * From [Finance].[ProformaInvoicesPanels] Where InvoiceId = {invoice.Id}";
                panels = connection.Query<ProformaInvoicePanel>(query).ToList();
            }

            List<List<ProformaInvoicePanel>> pagePanels = new() { new List<ProformaInvoicePanel>() };

            int page = 1;
            //double maxHeight1FirstPage = 332;
            //double maxHeight2FirstPage = 620;
            //double maxHeight1 = 512;
            //double maxHeight2 = 800;
            double maxHeight1FirstPage = 367;
            double maxHeight2FirstPage = 655;
            double maxHeight1 = 547;
            double maxHeight2 = 835;

            int pagesNumber;

            NewTable();
            int sn = 1;
            foreach (ProformaInvoicePanel panel in panels)
            {
                panel.SN = sn++;
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

                    //First Page Only
                    if (page == 1)
                    {
                        while (Table.ActualHeight + 25 < maxHeight2FirstPage)
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
                    else
                    {
                        while (Table.ActualHeight + 25 < maxHeight2)
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
                }

                //First Page Only
                if (page == 1)
                {
                    if (Table.ActualHeight > maxHeight1FirstPage)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2FirstPage)
                        {
                            ProformaInvoicePanel newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<ProformaInvoicePanel>());
                                NewTable();
                            }
                            else
                            {
                                newPanel.Description = EnglishName.Text;
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<ProformaInvoicePanel>());
                                NewTable();

                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            ProformaInvoicePanel newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        ProformaInvoicePanel newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }
                else
                {
                    if (Table.ActualHeight > maxHeight1)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2)
                        {
                            ProformaInvoicePanel newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<ProformaInvoicePanel>());
                                NewTable();
                            }
                            else
                            {
                                newPanel.Description = EnglishName.Text;
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<ProformaInvoicePanel>());
                                NewTable();

                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            ProformaInvoicePanel newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        ProformaInvoicePanel newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }
            }

            List<ProformaInvoicePanel> lastPage = pagePanels.Last();
            if (page == 1)
            {
                if (Table.ActualHeight > maxHeight1FirstPage)
                {

                    if (lastPage.Count != 1)
                    {
                        ProformaInvoicePanel lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<ProformaInvoicePanel>());
                        pagePanels.Last().Add(lastPanel);
                    }
                    else
                    {
                        pagePanels.Add(new List<ProformaInvoicePanel>());
                    }

                    page++;
                }
            }
            else
            {
                if (Table.ActualHeight > maxHeight1)
                {
                    if (lastPage.Count != 1)
                    {
                        ProformaInvoicePanel lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<ProformaInvoicePanel>());
                        pagePanels.Last().Add(lastPanel);
                    }
                    else
                    {
                        pagePanels.Add(new List<ProformaInvoicePanel>());
                    }

                    page++;
                }
            }


            if (panels.Count != 0)
            {
                pagesNumber = pagePanels.Count;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    if (i == pagesNumber)
                    {
                        invoiceForm = new ProformaInvoiceForm()
                        {
                            PageNumber = i,
                            TotalPages = pagesNumber,
                            VATPercentage = invoice.VAT * 100,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };


                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }
                    else
                    {
                        invoiceForm = new ProformaInvoiceForm()
                        {
                            PageNumber = i,
                            TotalPages = pagesNumber,
                            VATPercentage = invoice.VAT * 100,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };

                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }

                    elements.Add(invoiceForm);
                }

                Print.PrintPreview(elements, $"Proforma Invoice-{invoice.Code}", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no panels!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }

        }

        private static void PrintingInvoice202406115Form(string invoiceNumber, IView checkPoint)
        {

            IsNormalForm = true;
            MessageBoxResult result = MessageWindow.Show("Printing",
                                                         "Print with watermark?",
                                                         MessageWindowButton.YesNo,
                                                         MessageWindowImage.Question);

            Invoice invoice;
            List<InvoiceItem> panels;
            Printing.SpecialInvoices.Invoice202406115Form invoiceForm;
            //List<InvoiceEdit> edits;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query;
                query = $"Select * From [Finance].[CustomersInvoices(View)] Where Code  = '{invoiceNumber}'";
                invoice = connection.QueryFirstOrDefault<Invoice>(query);

                query = $"Select * From [Finance].[CustomersInvoicesItems] Where InvoiceId = {invoice.Id}";
                panels = connection.Query<InvoiceItem>(query).ToList();

                //query = $"Select * From [Finance].[CustomersInvoicesEdits] Where InvoiceId = {invoice.Id}";
                //edits = connection.Query<InvoiceEdit>(query).ToList();
            }

            List<List<InvoiceItem>> pagePanels = new() { new List<InvoiceItem>() };

            int page = 1;
            double maxHeight1FirstPage = 332;
            double maxHeight2FirstPage = 620;
            double maxHeight1 = 512;
            double maxHeight2 = 800;

            int pagesNumber;

            NewTable();
            int sn = 1;
            foreach (InvoiceItem panel in panels)
            {
                panel.SN = sn++;
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

                    //First Page Only
                    if (page == 1)
                    {
                        while (Table.ActualHeight + 25 < maxHeight2FirstPage)
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
                    else
                    {
                        while (Table.ActualHeight + 25 < maxHeight2)
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
                }

                //First Page Only
                if (page == 1)
                {
                    if (Table.ActualHeight > maxHeight1FirstPage)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2FirstPage)
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                            }
                            else
                            {
                                newPanel.Description = EnglishName.Text;
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();

                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }
                else
                {
                    if (Table.ActualHeight > maxHeight1)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2)
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                            }
                            else
                            {
                                newPanel.Description = EnglishName.Text;
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();

                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }

            }

            List<InvoiceItem> lastPage = pagePanels.Last();
            if (page == 1)
            {
                if (Table.ActualHeight > maxHeight1FirstPage)
                {

                    if (lastPage.Count != 1)
                    {
                        InvoiceItem lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<InvoiceItem>());
                        pagePanels.Last().Add(lastPanel);
                    }
                    else
                    {
                        pagePanels.Add(new List<InvoiceItem>());
                    }

                    page++;
                }
            }
            else
            {
                if (Table.ActualHeight > maxHeight1)
                {
                    if (lastPage.Count != 1)
                    {
                        InvoiceItem lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<InvoiceItem>());
                        pagePanels.Last().Add(lastPanel);
                    }
                    else
                    {
                        pagePanels.Add(new List<InvoiceItem>());
                    }

                    page++;
                }
            }


            if (panels.Count != 0)
            {
                pagesNumber = pagePanels.Count;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    if (i == pagesNumber)
                    {
                        invoiceForm = new Printing.SpecialInvoices.Invoice202406115Form ()
                        {
                            Page = i,
                            Pages = pagesNumber,
                            NetPrice = invoice.NetPrice,
                            VATPercentage = invoice.VATPercentage,
                            VATValue = invoice.VATValue,
                            GrossPrice = invoice.GrossPrice,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };


                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }
                    else
                    {
                        invoiceForm = new Printing.SpecialInvoices.Invoice202406115Form()
                        {
                            Page = i,
                            Pages = pagesNumber,
                            VATPercentage = invoice.VATPercentage,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };

                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }

                    elements.Add(invoiceForm);
                }

                Print.PrintPreview(elements, $"Invoice-{invoice.Code}", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no panels!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }

        private static void PercentageInvoice202412224(Invoice invoice, List<InvoiceItem> panels, MessageBoxResult result, IView checkPoint)
        {
            PanelsInvoicePercentageForm202412224 invoiceForm;

            List<List<InvoiceItem>> pagePanels = new() { new List<InvoiceItem>() };

            int page = 1;
            double maxHeight1FirstPage = 732;
            double maxHeight2FirstPage = 720;
            double maxHeight1 = 512;
            double maxHeight2 = 800;

            int pagesNumber;

            NewTable();
            int sn = 1;
            foreach (InvoiceItem panel in panels)
            {
                panel.SN = sn++;
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

                    //First Page Only
                    if (page == 1)
                    {
                        while (Table.ActualHeight + 25 < maxHeight2FirstPage)
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
                    else
                    {
                        while (Table.ActualHeight + 25 < maxHeight2)
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
                }

                //First Page Only
                if (page == 1)
                {
                    UpdateUI();
                    if (Table.ActualHeight > maxHeight1FirstPage)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2FirstPage)
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(EnglishName.Text))
                                {
                                    page++;
                                    pagePanels.Add(new List<InvoiceItem>());
                                    NewTable();
                                }
                                else
                                {
                                    newPanel.Description = EnglishName.Text;
                                    pagePanels[page - 1].Add(newPanel);

                                    page++;
                                    pagePanels.Add(new List<InvoiceItem>());
                                    NewTable();
                                }


                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }
                else
                {
                    if (Table.ActualHeight > maxHeight1)
                    {
                        if ((Table.ActualHeight + 25) > maxHeight2)
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);

                            if (!isMultiLine)
                            {
                                pagePanels[page - 1].Add(newPanel);

                                page++;
                                pagePanels.Add(new List<InvoiceItem>());
                                NewTable();
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(EnglishName.Text))
                                {
                                    page++;
                                    pagePanels.Add(new List<InvoiceItem>());
                                    NewTable();
                                }
                                else
                                {
                                    newPanel.Description = EnglishName.Text;
                                    pagePanels[page - 1].Add(newPanel);

                                    page++;
                                    pagePanels.Add(new List<InvoiceItem>());
                                    NewTable();
                                }

                                if (lines.Count != 0)
                                {
                                    goto Panel;
                                }
                            }
                        }
                        else
                        {
                            InvoiceItem newPanel = new();
                            newPanel.Update(panel);
                            pagePanels[page - 1].Add(newPanel);

                            if (isMultiLine)
                            {
                                newPanel.Description = EnglishName.Text;
                            }
                        }
                    }
                    else
                    {
                        InvoiceItem newPanel = new();
                        newPanel.Update(panel);
                        pagePanels[page - 1].Add(newPanel);

                        if (isMultiLine)
                        {
                            newPanel.Description = EnglishName.Text;
                        }
                    }
                }

            }

            List<InvoiceItem> lastPage = pagePanels.Last();
            if (page == 1)
            {
                if (Table.ActualHeight > maxHeight1FirstPage)
                {

                    if (lastPage.Count != 1)
                    {
                        InvoiceItem lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<InvoiceItem>());
                        pagePanels.Last().Add(lastPanel);
                    }
                    else
                    {
                        pagePanels.Add(new List<InvoiceItem>());
                    }

                    page++;
                }
            }
            else
            {
                if (Table.ActualHeight > maxHeight1)
                {
                    if (lastPage.Count != 1)
                    {
                        InvoiceItem lastPanel = pagePanels.Last().Last();
                        pagePanels.Last().Remove(lastPanel);
                        pagePanels.Add(new List<InvoiceItem>());
                        pagePanels.Last().Add(lastPanel);
                    }
                    else
                    {
                        pagePanels.Add(new List<InvoiceItem>());
                    }

                    page++;
                }
            }


            if (panels.Count != 0)
            {
                pagesNumber = pagePanels.Count;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    if (i == pagesNumber)
                    {
                        invoiceForm = new PanelsInvoicePercentageForm202412224()
                        {
                            Page = i,
                            Pages = pagesNumber,
                            NetPrice = invoice.NetPrice,
                            VATPercentage = invoice.VATPercentage,
                            VATValue = invoice.VATValue,
                            GrossPrice = invoice.GrossPrice,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };


                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }
                    else
                    {
                        invoiceForm = new PanelsInvoicePercentageForm202412224()
                        {
                            Page = i,
                            Pages = pagesNumber,
                            VATPercentage = invoice.VATPercentage,
                            PanelsData = pagePanels[i - 1],
                            InvoiceData = invoice,
                        };

                        if (result == MessageBoxResult.Yes)
                        {
                            invoiceForm.BackgroundImage.Source = AppData.CompanyWatermark;
                        }
                    }

                    elements.Add(invoiceForm);
                }

                Print.PrintPreview(elements, $"Invoice-{invoice.Code}", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no panels!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }
    }
}
