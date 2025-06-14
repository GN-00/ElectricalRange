using Dapper;

using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Store;
using ProjectsNow.Printing.Store;
using ProjectsNow.Views;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Item = ProjectsNow.Printing.Store.Item;

namespace ProjectsNow.Printing
{
    public static class PrintingHelper
    {
        public static void PrintReturnInvoice(int invoiceId, IView checkPoint = null)
        {
            string query;
            ReturnInvoice returnInvoice;
            List<Item> items;
            ReturnInvoiceForm returnInvoiceForm;
            using (SqlConnection connection = new(Data.Database.ConnectionString))
            {
                query = $"Select * From [Store].[ReturnInvoices(View)] Where ID = {invoiceId}";
                returnInvoice = connection.QueryFirstOrDefault<ReturnInvoice>(query);

                query = $"Select * From [Store].[ReturnInvoicesItems(View)] Where InvoiceID = {invoiceId}";
                items = connection.Query<Item>(query).ToList();
            }

            int sn = 0;
            foreach (Item item in items)
            {
                item.SN = ++sn;
            }

            Grid grid;
            TextBlock textBlock;
            List<List<Item>> pageItems = new() { new List<Item>() };

            int page = 1;
            double hight = 0;
            double actualhight;
            double maxHeight = 245;
            double maxHeight2 = 480;

            int pagesNumber;
            foreach (Item item in items)
            {
                grid = new Grid()
                {
                    Width = 299,
                    MinHeight = 35,
                };
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                textBlock = new TextBlock()
                {
                    Text = item.Code,
                    FontFamily = new System.Windows.Media.FontFamily("Calibri (Body)"),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                grid.Children.Add(textBlock);

                textBlock = new TextBlock()
                {
                    Text = item.Description,
                    FontFamily = new System.Windows.Media.FontFamily("Calibri (Body)"),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                Grid.SetRow(textBlock, 1);
                grid.Children.Add(textBlock);

                grid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                grid.Arrange(new Rect(grid.DesiredSize));

                actualhight = grid.ActualHeight;

                if ((hight + actualhight) > maxHeight)
                {
                    if (items.IndexOf(item) == items.Count - 1)
                    {
                        page++;
                        pageItems.Add(new List<Item>());
                        pageItems[page - 1].Add(item);
                        hight = actualhight;
                    }
                    else
                    {

                        if ((hight + actualhight) > maxHeight2)
                        {
                            page++;
                            pageItems.Add(new List<Item>());
                            pageItems[page - 1].Add(item);
                            hight = actualhight;
                        }
                        else
                        {
                            hight += actualhight;
                            pageItems[page - 1].Add(item);
                        }
                    }
                }
                else
                {
                    hight += actualhight;
                    pageItems[page - 1].Add(item);
                }
            }

            if (items.Count != 0)
            {
                pagesNumber = pageItems.Count;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    if (i == pagesNumber)
                    {
                        returnInvoiceForm = new ReturnInvoiceForm()
                        {
                            VATPercentage = items.Max(p => p.VAT),
                            TotalCost = items.Sum(p => p.TotalCost),
                            TotalVAT = items.Sum(p => p.VATValue),
                            TotalPrice = items.Sum(p => p.TotalPrice),
                            Page = i,
                            Pages = pagesNumber,
                            InvoiceData = returnInvoice,
                            ItemsData = pageItems[i - 1],
                        };
                    }
                    else
                    {
                        returnInvoiceForm = new ReturnInvoiceForm()
                        {
                            VATPercentage = items.Max(p => p.VAT),
                            Page = i,
                            Pages = pagesNumber,
                            InvoiceData = returnInvoice,
                            ItemsData = pageItems[i - 1],
                        };
                    }
                    elements.Add(returnInvoiceForm);
                }

                Print.PrintPreview(elements, $"Invoice-{returnInvoice.Code}", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }
        public static void PrintInternalInvoice(int invoiceId, IView checkPoint = null)
        {
            string query;
            InvoiceInformation invoice;
            InternalInvoice internalInvoiceForm;
            ObservableCollection<Item> items;
            List<FrameworkElement> elements = new();
            using (SqlConnection connection = new(Data.Database.ConnectionString))
            {
                query = $"Select * From [Store].[InvoicesInformations] " +
                        $"Where ID  = {invoiceId}";
                invoice = connection.QueryFirstOrDefault<InvoiceInformation>(query);

                query = $"Select * From [Store].[InvoicesItemsInformations] " +
                        $"Where InvoiceID  = {invoiceId} " +
                        $"Order By Code";
                items = new ObservableCollection<Item>(connection.Query<Item>(query));
            }

            int sn = 0;
            foreach (Item item in items)
            {
                item.SN = ++sn;
            }

            double maxRow = 8;
            double pages = items.Count / maxRow;
            if (pages - Math.Truncate(pages) != 0)
            {
                pages = Math.Truncate(pages) + 1;
            }


            if (pages != 0)
            {
                for (int i = 1; i <= pages; i++)
                {
                    if (i == pages)
                    {
                        internalInvoiceForm = new InternalInvoice()
                        {
                            VATPercentage = items.Max(p => p.VAT),
                            TotalCost = items.Sum(p => p.TotalCost),
                            TotalVAT = items.Sum(p => p.VATValue),
                            TotalPrice = items.Sum(p => p.TotalPrice),
                            Page = i,
                            Pages = (int)pages,
                            InvoiceData = invoice,
                            ItemsData = items.Where(p => p.SN > ((i - 1) * maxRow) && p.SN <= (i * maxRow)).ToList(),
                        };
                    }
                    else
                    {
                        internalInvoiceForm = new InternalInvoice()
                        {
                            VATPercentage = items.Max(p => p.VAT),
                            Page = i,
                            Pages = (int)pages,
                            InvoiceData = invoice,
                            ItemsData = items.Where(p => p.SN > ((i - 1) * maxRow) && p.SN <= (i * maxRow)).ToList(),
                        };
                    }

                    elements.Add(internalInvoiceForm);
                }

                Print.PrintPreview(elements, $"Internal Invoice-{invoice.InvoiceNumber}", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }
    }
}
