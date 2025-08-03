using Dapper;

using Microsoft.Data.SqlClient;

using ProjectsNow.AttachedProperties;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Views;
using ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectsNow.Services
{
    public static class PurchaseOrdersServices
    {
        private static Grid Table { get; set; }
        private static Border DescriptionBorder { get; set; }
        private static TextBlock DescriptionTextBlock { get; set; }

        private static void UpdateUI()
        {
            List<UIElement> elements = new()
            {
                DescriptionTextBlock,
                DescriptionBorder,
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
            Table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(283.465) });
            Table.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.6 * AppData.cm) });
        }
        private static void NewCell(CompanyPOTransaction transaction)
        {
            DescriptionTextBlock = new TextBlock()
            {
                Text = transaction.Description,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 12,
                FontFamily = new FontFamily("Calibri (Body)"),
                Margin = new Thickness(5, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            DescriptionBorder = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = DescriptionTextBlock,
            };
            Grid.SetRow(DescriptionBorder, Table.RowDefinitions.Count + 1);
            Table.RowDefinitions.Add(new RowDefinition() { MinHeight = 0.6 * AppData.cm });
            Table.Children.Add(DescriptionBorder);
        }
        public static void Print(CompanyPO order, IView checkPoint)
        {
            List<List<CompanyPOTransaction>> pagesTransactions = new();
            pagesTransactions.Add(new List<CompanyPOTransaction>());

            string query;
            CompanyPO OrderData;
            List<CompanyPOTransaction> Items;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (order.Revised)
                {
                    query = $"Select * From [Purchase].[Orders(Revisions)] " +
                            $"Where ID = {order.ID} ";
                }
                else
                {
                    query = $"Select * From [Purchase].[OrdersView] " +
                            $"Where ID = {order.ID} ";
                }

                OrderData = connection.QueryFirstOrDefault<CompanyPO>(query);

                if (order.Revised)
                {
                    query = $"Select * From [Purchase].[Transactions(Revisions)] " +
                            $"Where PurchaseOrderID = {OrderData.ID} " +
                            $"Order By Code";
                }
                else
                {
                    query = $"Select * From [Purchase].[TransactionsView] " +
                            $"Where PurchaseOrderID = {OrderData.ID} " +
                            $"Order By Code";
                }


                Items = connection.Query<CompanyPOTransaction>(query).ToList();
            }

            int page = 1;
            double maxHeight = 250;
            double maxHeight2 = 477;

            int pagesNumber;

            NewTable();
            foreach (CompanyPOTransaction transaction in Items)
            {
                transaction.SN = Items.IndexOf(transaction) + 1;

                NewCell(transaction);
                UpdateUI();

                var lines = DescriptionTextBlock.GetLines().ToList();
                bool isMultiLine = lines.Count > 1;

            Panel:

                int nameLines = lines.Count;
                if (nameLines >= 1 && isMultiLine)
                {
                    DescriptionTextBlock.Text = "";
                    UpdateUI();

                    while (Table.ActualHeight + 25 < maxHeight2)
                    {
                        if (lines.Count == 0)
                            break;

                        DescriptionTextBlock.Text += "\n" + lines[0];
                        UpdateUI();
                        lines.Remove(lines[0]);
                    }
                }

                if (Table.ActualHeight > maxHeight)
                {
                    if ((Table.ActualHeight + 25) > maxHeight2)
                    {
                        page++;
                        pagesTransactions.Add(new List<CompanyPOTransaction>());
                        CompanyPOTransaction newTransaction = new();
                        newTransaction.Update(transaction);
                        NewTable();

                        if (!isMultiLine)
                        {
                            pagesTransactions[page - 1].Add(newTransaction);
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
                        CompanyPOTransaction newTransaction = new();
                        newTransaction.Update(transaction);
                        pagesTransactions[page - 1].Add(newTransaction);
                    }
                }
                else
                {
                    CompanyPOTransaction newTransaction = new();
                    newTransaction.Update(transaction);
                    pagesTransactions[page - 1].Add(newTransaction);
                }
            }

            if (Table.ActualHeight > maxHeight)
            {
                page++;
                pagesTransactions.Add(new List<CompanyPOTransaction>());
            }

            pagesNumber = pagesTransactions.Count;
            List<FrameworkElement> elements = new();
            if (pagesNumber != 0)
            {
                for (int i = 1; i <= pagesNumber; i++)
                {
                    Printing.Purchase.PurchaseOrderForm purchaseOrderForm = new()
                    {
                        Page = i,
                        Pages = Convert.ToInt32(pagesNumber),
                        CompanyPOData = OrderData,
                        Transactions = pagesTransactions[i - 1],
                        AllTransactions = Items,
                    };
                    elements.Add(purchaseOrderForm);
                }

                Printing.Print.PrintPreview(elements, $"Purchase Order {OrderData.Number}", checkPoint);
            }
            else
            {
                _ = MessageView.Show("Statement", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
            }
        }
        internal static void GetOrderCode(CompanyPO orderData)
        {
            orderData.Code =
                 $"{orderData.Number}" +
                 $"/ER-PCAPS/{orderData.Date.Month:00}/{orderData.Date.Year}";
        }

        internal static void Revisions(CompanyPO order, IView viewData)
        {
            Navigation.OpenPopup(new PurchaseOrrderRevisionsView(order, viewData), System.Windows.Controls.Primitives.PlacementMode.Center, true);
        }
    }
}