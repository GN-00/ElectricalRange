using ClosedXML.Excel;

using Dapper;

using DocumentFormat.OpenXml;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Data;
using ProjectsNow.Printing.QuotationsStatus.CustomersReports;
using ProjectsNow.Views.QuotationsStatusViews;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ProjectsNow.Services
{
    public static class QuotationsStatusServices
    {
        public static FixedDocument Print(QuotationsReportViewModel data)
        {
            ObservableCollection<Quotation> list;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [QuotationsStatus].[QuotationsReport] " +
                               $"Where DateIssued Between @StartDate And @EndDate ";

                if (data.Lost || data.Win || data.OnGoing || data.Hold || data.Cancel)
                {
                    string status = "";

                    if (data.OnGoing) status += "'On Going', ";
                    if (data.Cancel) status += "'Cancel', ";
                    if (data.Lost) status += "'Lost', ";
                    if (data.Win) status += "'Win', ";
                    if (data.Hold) status += "'Hold', ";

                    query += $"And Status in ({status.Substring(0, status.Length - 2)}) ";
                }

                if (data.Estimator != null)
                {
                    query += $"And EstimationID = {data.Estimator.Id} ";
                }

                if (data.Salesman != null)
                {
                    query += $"And SalesmanID = {data.Salesman.Id} ";
                }

                if (data.Customer != null)
                {
                    query += $"And CustomerID = {data.Customer.CustomerID} ";
                }

                if (data.Bidding || data.OnHand)
                {
                    string type = "";

                    if (data.Bidding) type += "'Bidding', ";
                    if (data.OnHand) type += "'On Hand', ";

                    query += $"And Type in ({type.Substring(0, type.Length - 2)}) ";
                }

                query += $"Order By QuotationYear, QuotationMonth, QuotationNumber ";
                list = new ObservableCollection<Quotation>(connection.Query<Quotation>(query, data));
            }

            if (list.Count == 0)
                return null;

            //405
            decimal maxRow = 25;
            double rowHeight = 25;
            //double totalHeight = 300 + 105;

            for (int i = 1; i <= list.Count; i++)
            {
                list[i - 1].SN = i;
            }

            decimal pages = list.Count / maxRow;
            if (pages - Math.Truncate(pages) != 0)
            {
                pages = Math.Truncate(pages) + 1;
            }

            pages++;

            int fontSize = 14;
            string fontFamily = "Times New Roman";

            Grid mainPage;
            Grid contentsGrid;
            Grid table;
            Grid totalGrid;
            Grid onHandGrid;
            Grid biddingGrid;
            Grid projectsGrid;
            TextBlock textBlock;
            Border border;

            List<FrameworkElement> elements = new();
            if (pages != 0)
            {
                for (int i = 1; i <= pages; i++)
                {
                    var quotations = list.Where(p => p.SN > ((i - 1) * maxRow) && p.SN <= (i * maxRow)).ToList();

                    mainPage = new Grid
                    {
                        Height = 790,
                        Width = 1120,
                    };
                    contentsGrid = new Grid() { Margin = new Thickness(20) };
                    mainPage.Children.Add(contentsGrid);

                    table = new Grid() { Margin = new Thickness(0, 80, 0, 0) };
                    contentsGrid.Children.Add(table);

                    table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
                    table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });
                    table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(190) });
                    table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(230) });
                    table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120) });
                    table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(105) });
                    table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(105) });
                    table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60) });
                    table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                    table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60) });

                    table.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowHeight) });

                    #region Title

                    textBlock = new TextBlock()
                    {
                        Text = $"Quotations Report",
                        FontSize = 30,
                        Margin = new Thickness(0, 20, 0, 0),
                        FontFamily = new FontFamily(fontFamily),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    contentsGrid.Children.Add(textBlock);

                    textBlock = new TextBlock()
                    {
                        Text = $"Date: {data.StartDate:dd/MM/yyyy} - {data.EndDate:dd/MM/yyyy}",
                        FontSize = 14,
                        Margin = new Thickness(10, 60, 0, 0),
                        FontFamily = new FontFamily(fontFamily),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };
                    contentsGrid.Children.Add(textBlock);

                    textBlock = new TextBlock()
                    {
                        Text = $"Page: {i}/{pages}",
                        FontSize = 14,
                        Margin = new Thickness(0, 60, 10, 0),
                        FontFamily = new FontFamily(fontFamily),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Right,
                    };
                    contentsGrid.Children.Add(textBlock);

                    #endregion

                    #region Header

                    if (i != pages)
                    {
                        //SN
                        textBlock = new TextBlock()
                        {
                            Text = "SN",
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 1, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 0);

                        //Date
                        textBlock = new TextBlock()
                        {
                            Text = "Date",
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 1, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 1);

                        //Project Name
                        textBlock = new TextBlock()
                        {
                            Text = "Project Name",
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 1, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 2);

                        //Customer Name
                        textBlock = new TextBlock()
                        {
                            Text = "Customer Name",
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 1, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 3);

                        //Quotation Code
                        textBlock = new TextBlock()
                        {
                            Text = "Quotation Code",
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 1, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 4);

                        //Estimator
                        textBlock = new TextBlock()
                        {
                            Text = "Estimation",
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 1, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 5);

                        //Salesman
                        textBlock = new TextBlock()
                        {
                            Text = "Salesman",
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 1, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 6);


                        //Type
                        textBlock = new TextBlock()
                        {
                            Text = "Type",
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 1, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 7);

                        //Total
                        textBlock = new TextBlock()
                        {
                            Text = "Total",
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 1, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 8);

                        //Status
                        textBlock = new TextBlock()
                        {
                            Text = "Status",
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 1, 1, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 9);

                        ////Status
                        //textBlock = new TextBlock()
                        //{
                        //    Text = "Month",
                        //    FontSize = fontSize,
                        //    FontFamily = new FontFamily(fontFamily),
                        //    VerticalAlignment = VerticalAlignment.Center,
                        //    HorizontalAlignment = HorizontalAlignment.Center,
                        //};
                        //border = new Border()
                        //{
                        //    BorderBrush = Brushes.Black,
                        //    BorderThickness = new Thickness(1, 1, 1, 1),
                        //    Child = textBlock,
                        //};
                        //_ = table.Children.Add(border);
                        //Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        //Grid.SetColumn(border, 9);

                    }

                    #endregion

                    #region Details
                    foreach (Quotation quotation in quotations)
                    {
                        table.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowHeight) });

                        //SN
                        textBlock = new TextBlock()
                        {
                            Text = quotation.SN.ToString(),
                            FontSize = fontSize,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 0, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 0);


                        //Date
                        textBlock = new TextBlock()
                        {
                            Text = quotation.DateIssued.ToString("dd-MM-yyyy"),
                            FontSize = 12,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 0, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 1);

                        //Project Name
                        textBlock = new TextBlock()
                        {
                            Text = quotation.Project,
                            FontSize = 11,
                            Margin = new Thickness(5, 0, 5, 0),
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 0, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 2);

                        //Customer Name
                        textBlock = new TextBlock()
                        {
                            Text = quotation.Customer,
                            FontSize = 11,
                            Margin = new Thickness(5, 0, 5, 0),
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 0, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 3);

                        //Quotation Code
                        textBlock = new TextBlock()
                        {
                            Text = quotation.QuotationCode,
                            FontSize = 11,
                            Margin = new Thickness(5, 0, 5, 0),
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 0, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 4);

                        //Estimation
                        textBlock = new TextBlock()
                        {
                            Text = quotation.EstimationName,
                            FontSize = 11,
                            Margin = new Thickness(5, 0, 5, 0),
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 0, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 5);

                        //Salesman
                        textBlock = new TextBlock()
                        {
                            Text = quotation.Salesman,
                            FontSize = 11,
                            Margin = new Thickness(5, 0, 5, 0),
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 0, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 6);

                        //Type
                        textBlock = new TextBlock()
                        {
                            Text = quotation.Type,
                            FontSize = 11,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 0, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 7);

                        //Total
                        textBlock = new TextBlock()
                        {
                            Text = quotation.Total.ToString("N2"),
                            FontSize = 11,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 0, 0, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 8);

                        //Status
                        textBlock = new TextBlock()
                        {
                            Text = quotation.StatusInfo,
                            FontSize = 11,
                            FontFamily = new FontFamily(fontFamily),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        };
                        border = new Border()
                        {
                            BorderBrush = Brushes.Black,
                            BorderThickness = new Thickness(1, 0, 1, 1),
                            Child = textBlock,
                        };
                        _ = table.Children.Add(border);
                        Grid.SetRow(border, table.RowDefinitions.Count - 1);
                        Grid.SetColumn(border, 9);
                    }
                    #endregion

                    #region Total

                    if (i == pages)
                    {
                        totalGrid = new Grid() { Margin = new Thickness(0, 80, 0, 0), VerticalAlignment = VerticalAlignment.Top };
                        totalGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        totalGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        totalGrid.ColumnDefinitions.Add(new ColumnDefinition());

                        if (data.OnHand)
                        {
                            var onHandList = list.Where(q => q.Type == "On Hand");
                            decimal onGoing = onHandList.Where(q => q.StatusInfo == "On Going").Sum(q => q.Total);
                            decimal lost = onHandList.Where(q => q.StatusInfo == "Lost").Sum(q => q.Total);
                            decimal win = onHandList.Where(q => q.StatusInfo == "Win").Sum(q => q.Total);
                            decimal hold = onHandList.Where(q => q.StatusInfo == "Hold").Sum(q => q.Total);
                            decimal cancel = onHandList.Where(q => q.StatusInfo == "Cancel").Sum(q => q.Total);
                            decimal total = onHandList.Sum(q => q.Total);

                            onHandGrid = new Grid() { Margin = new Thickness(0, 0, 5, 0) };
                            totalGrid.Children.Add(onHandGrid);
                            Grid.SetColumn(onHandGrid, 0);
                            onHandGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
                            onHandGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Win ? 25 : 0) });
                            onHandGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.OnGoing ? 25 : 0) });
                            onHandGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Hold ? 25 : 0) });
                            onHandGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Cancel ? 25 : 0) });
                            onHandGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Lost ? 25 : 0) });
                            onHandGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });

                            onHandGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            onHandGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            onHandGrid.ColumnDefinitions.Add(new ColumnDefinition());

                            textBlock = new TextBlock()
                            {
                                Text = "Summary for On Hand Projects",
                                FontSize = fontSize,
                                FontFamily = new FontFamily(fontFamily),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center,
                            };
                            border = new Border()
                            {
                                BorderBrush = Brushes.Black,
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                Child = textBlock,
                            };
                            _ = onHandGrid.Children.Add(border);
                            Grid.SetRow(border, 0);
                            Grid.SetColumn(border, 0);
                            Grid.SetColumnSpan(border, 3);

                            if (data.Win)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Win",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 1);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{win:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 1);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{win / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 1);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.OnGoing)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "On Going",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 2);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{onGoing:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 2);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{onGoing / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 2);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.Hold)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Hold",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 3);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{hold:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 3);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{hold / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 3);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.Cancel)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Cancel",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 4);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{cancel:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 4);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{cancel / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 4);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.Lost)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Lost",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 5);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{lost:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 5);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{lost / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 5);
                                Grid.SetColumn(border, 2);
                            }

                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Total",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 6);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{total:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 6);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = onHandGrid.Children.Add(border);
                                Grid.SetRow(border, 6);
                                Grid.SetColumn(border, 2);
                            }
                        }

                        if (data.Bidding)
                        {
                            var biddingList = list.Where(q => q.Type == "Bidding");
                            decimal lost = biddingList.Where(q => q.StatusInfo == "Lost").Sum(q => q.Total);
                            decimal onGoing = biddingList.Where(q => q.StatusInfo == "On Going").Sum(q => q.Total);
                            decimal win = biddingList.Where(q => q.StatusInfo == "Win").Sum(q => q.Total);
                            decimal hold = biddingList.Where(q => q.StatusInfo == "Hold").Sum(q => q.Total);
                            decimal cancel = biddingList.Where(q => q.StatusInfo == "Cancel").Sum(q => q.Total);
                            decimal total = biddingList.Sum(q => q.Total);

                            biddingGrid = new Grid() { Margin = new Thickness(2.5, 0, 2.5, 0) };
                            totalGrid.Children.Add(biddingGrid);
                            Grid.SetColumn(biddingGrid, 1);
                            biddingGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
                            biddingGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.OnGoing ? 25 : 0) });
                            biddingGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Hold ? 25 : 0) });
                            biddingGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Cancel ? 25 : 0) });
                            biddingGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Lost ? 25 : 0) });
                            biddingGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });

                            biddingGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            biddingGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            biddingGrid.ColumnDefinitions.Add(new ColumnDefinition());

                            textBlock = new TextBlock()
                            {
                                Text = "Summary for Bidding Projects",
                                FontSize = fontSize,
                                FontFamily = new FontFamily(fontFamily),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center,
                            };
                            border = new Border()
                            {
                                BorderBrush = Brushes.Black,
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                Child = textBlock,
                            };
                            _ = biddingGrid.Children.Add(border);
                            Grid.SetRow(border, 0);
                            Grid.SetColumn(border, 0);
                            Grid.SetColumnSpan(border, 3);

                            if (data.OnGoing)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "On Going",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 1);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{onGoing:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 1);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{onGoing / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 1);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.Hold)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Hold",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 2);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{hold:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 2);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{hold / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 2);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.Cancel)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Cancel",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 3);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{cancel:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 3);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{cancel / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 3);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.Lost)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Lost",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 4);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{lost:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 4);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{lost / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 4);
                                Grid.SetColumn(border, 2);
                            }

                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Total",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 6);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{total:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 6);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = biddingGrid.Children.Add(border);
                                Grid.SetRow(border, 6);
                                Grid.SetColumn(border, 2);
                            }
                        }

                        if (data.OnHand || data.Bidding)
                        {
                            decimal lost = list.Where(q => q.StatusInfo == "Lost").Sum(q => q.Total);
                            decimal onGoing = list.Where(q => q.StatusInfo == "On Going").Sum(q => q.Total);
                            decimal win = list.Where(q => q.StatusInfo == "Win").Sum(q => q.Total);
                            decimal hold = list.Where(q => q.StatusInfo == "Hold").Sum(q => q.Total);
                            decimal cancel = list.Where(q => q.StatusInfo == "Cancel").Sum(q => q.Total);
                            decimal total = list.Sum(q => q.Total);

                            projectsGrid = new Grid() { Margin = new Thickness(5, 0, 0, 0) };
                            totalGrid.Children.Add(projectsGrid);
                            Grid.SetColumn(projectsGrid, 2);
                            projectsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
                            projectsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Win ? 25 : 0) });
                            projectsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.OnGoing ? 25 : 0) });
                            projectsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Hold ? 25 : 0) });
                            projectsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Cancel ? 25 : 0) });
                            projectsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(data.Lost ? 25 : 0) });
                            projectsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });

                            projectsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            projectsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            projectsGrid.ColumnDefinitions.Add(new ColumnDefinition());

                            textBlock = new TextBlock()
                            {
                                Text = "Summary for Total Projects",
                                FontSize = fontSize,
                                FontFamily = new FontFamily(fontFamily),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center,
                            };
                            border = new Border()
                            {
                                BorderBrush = Brushes.Black,
                                BorderThickness = new Thickness(1, 1, 1, 1),
                                Child = textBlock,
                            };
                            _ = projectsGrid.Children.Add(border);
                            Grid.SetRow(border, 0);
                            Grid.SetColumn(border, 0);
                            Grid.SetColumnSpan(border, 3);

                            if (data.Win)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Win",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 1);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{win:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 1);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{win / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 1);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.OnGoing)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "On Going",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 2);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{onGoing:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 2);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{onGoing / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 2);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.Hold)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Hold",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 3);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{hold:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 3);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{hold / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 3);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.Cancel)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Cancel",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 4);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{cancel:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 4);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{cancel / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 4);
                                Grid.SetColumn(border, 2);
                            }
                            if (data.Lost)
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Lost",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 5);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{lost:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 5);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                if (total > 0)
                                    textBlock.Text = $"{lost / total * 100:N2} %";

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 5);
                                Grid.SetColumn(border, 2);
                            }
                            {
                                textBlock = new TextBlock()
                                {
                                    Text = "Total",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(1, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 6);
                                Grid.SetColumn(border, 0);

                                textBlock = new TextBlock()
                                {
                                    Text = $"{total:N2}",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };
                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 6);
                                Grid.SetColumn(border, 1);

                                textBlock = new TextBlock()
                                {
                                    Text = $"-",
                                    FontSize = fontSize,
                                    FontFamily = new FontFamily(fontFamily),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                };

                                border = new Border()
                                {
                                    BorderBrush = Brushes.Black,
                                    BorderThickness = new Thickness(0, 0, 1, 1),
                                    Child = textBlock,
                                };
                                _ = projectsGrid.Children.Add(border);
                                Grid.SetRow(border, 6);
                                Grid.SetColumn(border, 2);
                            }
                        }

                        //if (height >= 0)
                        //{
                        contentsGrid.Children.Add(totalGrid);
                        //}
                        //else
                        //{
                        //    elements.Add(mainPage);

                        //    pages++;

                        //    mainPage = new Grid
                        //    {
                        //        Height = 790,
                        //        Width = 1120,
                        //    };
                        //    contentsGrid = new Grid() { Margin = new Thickness(20) };
                        //    mainPage.Children.Add(contentsGrid);

                        //    contentsGrid.Children.Add(total);
                        //    total.Margin = new Thickness(0, 80, 0, 0);
                        //    total.VerticalAlignment = VerticalAlignment.Top;

                        //    textBlock = new TextBlock()
                        //    {
                        //        Text = $"Quotations Report",
                        //        FontSize = 34,
                        //        Margin = new Thickness(0, 20, 0, 0),
                        //        FontFamily = new FontFamily(fontFamily),
                        //        VerticalAlignment = VerticalAlignment.Top,
                        //        HorizontalAlignment = HorizontalAlignment.Center,
                        //    };
                        //    contentsGrid.Children.Add(textBlock);

                        //    textBlock = new TextBlock()
                        //    {
                        //        Text = $"Date: {data.StartDate:dd/MM/yyyy} - {data.EndDate:dd/MM/yyyy}",
                        //        FontSize = 14,
                        //        Margin = new Thickness(10, 60, 0, 0),
                        //        FontFamily = new FontFamily(fontFamily),
                        //        VerticalAlignment = VerticalAlignment.Top,
                        //        HorizontalAlignment = HorizontalAlignment.Left,
                        //    };
                        //    contentsGrid.Children.Add(textBlock);

                        //    textBlock = new TextBlock()
                        //    {
                        //        Text = $"Page: {i + 1}/{pages}",
                        //        FontSize = 14,
                        //        Margin = new Thickness(0, 60, 10, 0),
                        //        FontFamily = new FontFamily(fontFamily),
                        //        VerticalAlignment = VerticalAlignment.Top,
                        //        HorizontalAlignment = HorizontalAlignment.Right,
                        //    };
                        //    contentsGrid.Children.Add(textBlock);
                        //}
                    }

                    #endregion

                    elements.Add(mainPage);
                }

                //Printing.Print.PrintPreview(elements, $"Quotations Report");
                return Printing.Print.GetFixedDocument(elements);
            }
            else
            {
                _ = MessageWindow.Show("Quotations", "There is no Quotations!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return null;
            }
        }
        public static FixedDocument PrintCustomerStatus(QuotationsReportViewModel data)
        {
            string salesman = data.Customer.SalesmanName;
            string customer = data.Customer.CustomerName;
            ObservableCollection<Quotation> list;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [QuotationsStatus].[QuotationsReport] " +
                               $"Where DateIssued Between @StartDate And @EndDate ";

                if (data.Lost || data.Win || data.OnGoing || data.Hold || data.Cancel)
                {
                    string status = "";

                    if (data.OnGoing) status += "'On Going', ";
                    if (data.Cancel) status += "'Cancel', ";
                    if (data.Lost) status += "'Lost', ";
                    if (data.Win) status += "'Win', ";
                    if (data.Hold) status += "'Hold', ";

                    query += $"And Status in ({status.Substring(0, status.Length - 2)}) ";
                }

                if (data.Estimator != null)
                {
                    query += $"And EstimationID = {data.Estimator.Id} ";
                }

                if (data.Salesman != null)
                {
                    query += $"And SalesmanID = {data.Salesman.Id} ";
                }

                if (data.Customer != null)
                {
                    query += $"And CustomerID = {data.Customer.CustomerID} ";
                }

                if (data.Bidding || data.OnHand)
                {
                    string type = "";

                    if (data.Bidding) type += "'Bidding', ";
                    if (data.OnHand) type += "'On Hand', ";

                    query += $"And Type in ({type.Substring(0, type.Length - 2)}) ";
                }

                query += $"Order By QuotationYear, QuotationMonth, QuotationNumber ";
                list = new ObservableCollection<Quotation>(connection.Query<Quotation>(query, data));
            }

            if (list.Count == 0)
                return null;

            decimal onGoing = list.Where(q => q.StatusInfo == "On Going").Sum(q => q.Total);
            decimal lost = list.Where(q => q.StatusInfo == "Lost").Sum(q => q.Total);
            decimal win = list.Where(q => q.StatusInfo == "Win").Sum(q => q.Total);
            decimal hold = list.Where(q => q.StatusInfo == "Hold").Sum(q => q.Total);
            decimal cancel = list.Where(q => q.StatusInfo == "Cancel").Sum(q => q.Total);

            List<IRow> rowsrList = new();

            if (list.Any(i => i.Status == "Win"))
            {
                rowsrList.Add(new Title("Win"));
                foreach (var item in list.Where(i => i.Status == "Win"))
                {
                    rowsrList.Add(new Record(item));
                }
                rowsrList.Add(new Total("Win", win));
                rowsrList.Add(new EmptyRow());
            }

            if (list.Any(i => i.Status == "On Going"))
            {
                rowsrList.Add(new Title("On Going"));
                foreach (var item in list.Where(i => i.Status == "On Going"))
                {
                    rowsrList.Add(new Record(item));
                }
                rowsrList.Add(new Total("On Going", onGoing));
                rowsrList.Add(new EmptyRow());
            }

            if (list.Any(i => i.Status == "Hold"))
            {
                rowsrList.Add(new Title("Hold"));
                foreach (var item in list.Where(i => i.Status == "Hold"))
                {
                    rowsrList.Add(new Record(item));
                }
                rowsrList.Add(new Total("Hold", hold));
                rowsrList.Add(new EmptyRow());
            }

            if (list.Any(i => i.Status == "Cancel"))
            {
                rowsrList.Add(new Title("Cancel"));
                foreach (var item in list.Where(i => i.Status == "Cancel"))
                {
                    rowsrList.Add(new Record(item));
                }
                rowsrList.Add(new Total("Cancel", cancel));
                rowsrList.Add(new EmptyRow());
            }

            if (list.Any(i => i.Status == "Lost"))
            {
                rowsrList.Add(new Title_Lossed());
                foreach (var item in list.Where(i => i.Status == "Lost"))
                {
                    rowsrList.Add(new Record_Lossed(item));
                }
                rowsrList.Add(new Total("Lossed", lost));
            }

            decimal maxRow = 28;
            decimal pages = rowsrList.Count / maxRow;
            if (pages - Math.Truncate(pages) != 0)
            {
                pages = Math.Truncate(pages) + 1;
            }

            List<FrameworkElement> elements = new();
            if (pages != 0)
            {
                for (int i = 1; i <= pages; i++)
                {
                    var rows = rowsrList.Where(r => rowsrList.IndexOf(r) >= ((i - 1) * maxRow) && rowsrList.IndexOf(r) < (i * maxRow)).ToList();

                    CustomerQuotationsReport report = new(customer, salesman)
                    {
                        ListData = rows,
                    };

                    elements.Add(report);
                }

                //Printing.Print.PrintPreview(elements, $"Quotations Report");
                return Printing.Print.GetFixedDocument(elements);
            }
            else
            {
                _ = MessageWindow.Show("Quotations", "There is no Quotations!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return null;
            }
        }
        public static void Export(QuotationsReportViewModel data)
        {
            ObservableCollection<Quotation> list;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [QuotationsStatus].[QuotationsReport] " +
                               $"Where DateIssued Between @StartDate And @EndDate ";

                if (data.Lost || data.Win || data.OnGoing || data.Hold || data.Cancel)
                {
                    string status = "";

                    if (data.OnGoing) status += "'On Going', ";
                    if (data.Lost) status += "'Lost', ";
                    if (data.Win) status += "'Win', ";
                    if (data.Hold) status += "'Hold', ";
                    if (data.Cancel) status += "'Cancel', ";

                    query += $"And Status in ({status.Substring(0, status.Length - 2)}) ";
                }

                if (data.Estimator != null)
                {
                    query += $"And EstimationID = {data.Estimator.Id} ";
                }

                if (data.Salesman != null)
                {
                    query += $"And SalesmanID = {data.Salesman.Id} ";
                }

                if (data.Customer != null)
                {
                    query += $"And CustomerID = {data.Customer.CustomerID} ";
                }

                if (data.Bidding || data.OnHand)
                {
                    string type = "";

                    if (data.Bidding) type += "'Bidding', ";
                    if (data.OnHand) type += "'On Hand', ";

                    query += $"And Type in ({type.Substring(0, type.Length - 2)}) ";
                }

                query += $"Order By QuotationYear, QuotationMonth, QuotationNumber ";
                list = new ObservableCollection<Quotation>(connection.Query<Quotation>(query, data));
            }

            if (list.Count == 0)
                return;

            try
            {
                List<ExcelQuotation> excelList = new();
                foreach (Quotation quotation in list)
                {
                    ExcelQuotation excelQuotation = new()
                    {
                        Date = quotation.DateIssued.ToString("dd-MM-yyyy"),
                        Project = quotation.Project,
                        Customer = quotation.Customer,
                        Quotation = quotation.QuotationCode,
                        Estimation = quotation.EstimationName,
                        Salesman = quotation.Salesman,
                        Type = quotation.Type,
                        Total = "SR " + quotation.Total.ToString("N2"),
                        Status = quotation.Status,
                        Comment = quotation.Note,
                    };

                    excelList.Add(excelQuotation);
                }

                if (excelList.Count != 0)
                {
                    string fileName;
                    using XLWorkbook workbook = new();
                    DataTable table = new();
                    using (ObjectReader reader = ObjectReader.Create(excelList))
                    {
                        table.Load(reader);
                    }

                    table.Columns["Date"].SetOrdinal(0);
                    table.Columns["Project"].SetOrdinal(1);
                    table.Columns["Customer"].SetOrdinal(2);
                    table.Columns["Quotation"].SetOrdinal(3);
                    table.Columns["Estimation"].SetOrdinal(4);
                    table.Columns["Salesman"].SetOrdinal(5);
                    table.Columns["Type"].SetOrdinal(6);
                    table.Columns["Total"].SetOrdinal(7);
                    table.Columns["Status"].SetOrdinal(8);
                    table.Columns["Comment"].SetOrdinal(9);

                    string worksheetName = $"Quotations Report";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    _ = workSheet.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Cell(1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Cell(1, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();
                    _ = workSheet.Column(7).AdjustToContents();
                    _ = workSheet.Column(8).AdjustToContents();
                    _ = workSheet.Column(9).AdjustToContents();
                    _ = workSheet.Column(10).AdjustToContents();

                    workSheet.Cell(1, 2).Value = "Customer Name";
                    workSheet.Cell(1, 3).Value = "Project Name";
                    workSheet.Cell(1, 4).Value = "Quotation Code";

                    workSheet.Row(1).InsertRowsAbove(3);

                    workSheet.Range(workSheet.Cell(2, 1), workSheet.Cell(2, 10)).Merge();
                    workSheet.Cell(2, 1).Value = "Quotations Report";
                    workSheet.Cell(2, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    workSheet.Cell(2, 1).Style.Font.FontSize = 24;
                    workSheet.Cell(2, 1).Style.Font.SetBold();

                    int lastRow = workSheet.Rows().Count() + 2;
                    XLColor lastColor = XLColor.FromArgb(220, 230, 241, 255);

                    if (data.OnHand)
                    {
                        var onHandList = list.Where(x => x.Type == "On Hand");
                        Group OnHand = new()
                        {
                            Name = "On Hand",
                            Lost = onHandList.Where(q => q.StatusInfo == "Lost").Sum(q => q.Total),
                            Win = onHandList.Where(q => q.StatusInfo == "Win").Sum(q => q.Total),
                            OnGoing = onHandList.Where(q => q.StatusInfo == "On Going").Sum(q => q.Total),
                            Hold = onHandList.Where(q => q.StatusInfo == "Hold").Sum(q => q.Total),
                            Cancel = onHandList.Where(q => q.StatusInfo == "Cancel").Sum(q => q.Total),
                            Total = onHandList.Sum(q => q.Total),
                        };

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189, 255));
                        workSheet.Cell(lastRow, 1).Value = "Summary for On Hand Projects";
                        workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 1).Style.Font.SetBold();

                        lastRow++;
                        lastColor = XLColor.FromArgb(220, 230, 241, 255);

                        if (data.Win)
                        {
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                            workSheet.Cell(lastRow, 1).Value = "Total Win";
                            workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                            workSheet.Cell(lastRow, 7).Value = $"{OnHand.Win:N2}  " +
                                                               $"({(OnHand.Total != 0 ? (OnHand.Win / OnHand.Total * 100).ToString("N2") : "-")}%)";

                            workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                            lastRow++;
                            if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                            {
                                lastColor = XLColor.White;
                            }
                            else
                            {
                                lastColor = XLColor.FromArgb(220, 230, 241, 255);
                            }
                        }

                        if (data.OnGoing)
                        {
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                            workSheet.Cell(lastRow, 1).Value = "Total On Going";
                            workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                            workSheet.Cell(lastRow, 7).Value = $"{OnHand.OnGoing:N2}  " +
                                                               $"({(OnHand.Total != 0 ? (OnHand.OnGoing / OnHand.Total * 100).ToString("N2") : "-")}%)";

                            workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                            lastRow++;
                            if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                            {
                                lastColor = XLColor.White;
                            }
                            else
                            {
                                lastColor = XLColor.FromArgb(220, 230, 241, 255);
                            }
                        }

                        if (data.Hold)
                        {
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                            workSheet.Cell(lastRow, 1).Value = "Total Hold";
                            workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                            workSheet.Cell(lastRow, 7).Value = $"{OnHand.Hold:N2}  " +
                                                               $"({(OnHand.Total != 0 ? (OnHand.Hold / OnHand.Total * 100).ToString("N2") : "-")}%)";
                            workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                            lastRow++;
                            if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                            {
                                lastColor = XLColor.White;
                            }
                            else
                            {
                                lastColor = XLColor.FromArgb(220, 230, 241, 255);
                            }
                        }

                        if (data.Cancel)
                        {
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                            workSheet.Cell(lastRow, 1).Value = "Total Cancel";
                            workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                            workSheet.Cell(lastRow, 7).Value = $"{OnHand.Cancel:N2}  " +
                                                               $"({(OnHand.Total != 0 ? (OnHand.Cancel / OnHand.Total * 100).ToString("N2") : "-")}%)";
                            workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                            lastRow++;
                            if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                            {
                                lastColor = XLColor.White;
                            }
                            else
                            {
                                lastColor = XLColor.FromArgb(220, 230, 241, 255);
                            }
                        }

                        if (data.Lost)
                        {
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                            workSheet.Cell(lastRow, 1).Value = "Total Lost";
                            workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                            workSheet.Cell(lastRow, 7).Value = $"{OnHand.Lost:N2}  " +
                                                               $"({(OnHand.Total != 0 ? (OnHand.Lost / OnHand.Total * 100).ToString("N2") : "-")}%)";
                            workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                            lastRow++;
                            if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                            {
                                lastColor = XLColor.White;
                            }
                            else
                            {
                                lastColor = XLColor.FromArgb(220, 230, 241, 255);
                            }
                        }

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                        workSheet.Cell(lastRow, 1).Value = "Total On Hand";
                        workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                        workSheet.Cell(lastRow, 7).Value = $"{OnHand.Total:N2}";
                        workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                        lastRow += 2;
                        if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                        {
                            lastColor = XLColor.White;
                        }
                        else
                        {
                            lastColor = XLColor.FromArgb(220, 230, 241, 255);
                        }
                    }

                    if (data.Bidding)
                    {
                        var biddingList = list.Where(x => x.Type == "Bidding");
                        Group bidding = new()
                        {
                            Name = "Bidding",
                            Lost = biddingList.Where(q => q.StatusInfo == "Lost").Sum(q => q.Total),
                            Win = biddingList.Where(q => q.StatusInfo == "Win").Sum(q => q.Total),
                            OnGoing = biddingList.Where(q => q.StatusInfo == "On Going").Sum(q => q.Total),
                            Hold = biddingList.Where(q => q.StatusInfo == "Hold").Sum(q => q.Total),
                            Cancel = biddingList.Where(q => q.StatusInfo == "Cancel").Sum(q => q.Total),
                            Total = biddingList.Sum(q => q.Total),
                        };

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189, 255));
                        workSheet.Cell(lastRow, 1).Value = "Summary for Bidding Projects";
                        workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 1).Style.Font.SetBold();

                        lastRow++;
                        lastColor = XLColor.FromArgb(220, 230, 241, 255);

                        if (data.Win)
                        {
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                            workSheet.Cell(lastRow, 1).Value = "Total Win";
                            workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                            workSheet.Cell(lastRow, 7).Value = $"{bidding.Win:N2}  " +
                                                               $"({(bidding.Total != 0 ? (bidding.Win / bidding.Total * 100).ToString("N2") : "-")}%)";
                            workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                            lastRow++;
                            if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                            {
                                lastColor = XLColor.White;
                            }
                            else
                            {
                                lastColor = XLColor.FromArgb(220, 230, 241, 255);
                            }
                        }

                        if (data.OnGoing)
                        {
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                            workSheet.Cell(lastRow, 1).Value = "Total On Going";
                            workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                            workSheet.Cell(lastRow, 7).Value = $"{bidding.OnGoing:N2}  " +
                                                               $"({(bidding.Total != 0 ? (bidding.OnGoing / bidding.Total * 100).ToString("N2") : "-")}%)";
                            workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                            lastRow++;
                            if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                            {
                                lastColor = XLColor.White;
                            }
                            else
                            {
                                lastColor = XLColor.FromArgb(220, 230, 241, 255);
                            }
                        }

                        if (data.Hold)
                        {
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                            workSheet.Cell(lastRow, 1).Value = "Total Hold";
                            workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                            workSheet.Cell(lastRow, 7).Value = $"{bidding.Hold:N2}  " +
                                                               $"({(bidding.Total != 0 ? (bidding.Hold / bidding.Total * 100).ToString("N2") : "-")}%)";
                            workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                            lastRow++;
                            if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                            {
                                lastColor = XLColor.White;
                            }
                            else
                            {
                                lastColor = XLColor.FromArgb(220, 230, 241, 255);
                            }
                        }

                        if (data.Cancel)
                        {
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                            workSheet.Cell(lastRow, 1).Value = "Total Cancel";
                            workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                            workSheet.Cell(lastRow, 7).Value = $"{bidding.Cancel:N2}  " +
                                                               $"({(bidding.Total != 0 ? (bidding.Cancel / bidding.Total * 100).ToString("N2") : "-")}%)";
                            workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                            lastRow++;
                            if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                            {
                                lastColor = XLColor.White;
                            }
                            else
                            {
                                lastColor = XLColor.FromArgb(220, 230, 241, 255);
                            }
                        }

                        if (data.Lost)
                        {
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                            workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                            workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                            workSheet.Cell(lastRow, 1).Value = "Total Lost";
                            workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                            workSheet.Cell(lastRow, 7).Value = $"{bidding.Lost:N2}  " +
                                                               $"({(bidding.Total != 0 ? (bidding.Lost / bidding.Total * 100).ToString("N2") : "-")}%)";
                            workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                            workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                            lastRow++;
                            if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                            {
                                lastColor = XLColor.White;
                            }
                            else
                            {
                                lastColor = XLColor.FromArgb(220, 230, 241, 255);
                            }
                        }

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                        workSheet.Cell(lastRow, 1).Value = "Total Bidding";
                        workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                        workSheet.Cell(lastRow, 7).Value = $"{bidding.Total:N2}";
                        workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                        lastRow += 2;
                        if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                        {
                            lastColor = XLColor.White;
                        }
                        else
                        {
                            lastColor = XLColor.FromArgb(220, 230, 241, 255);
                        }
                    }

                    Group projects = new()
                    {
                        Name = "Projects",
                        Lost = list.Where(q => q.StatusInfo == "Lost").Sum(q => q.Total),
                        Win = list.Where(q => q.StatusInfo == "Win").Sum(q => q.Total),
                        OnGoing = list.Where(q => q.StatusInfo == "On Going").Sum(q => q.Total),
                        Hold = list.Where(q => q.StatusInfo == "Hold").Sum(q => q.Total),
                        Cancel = list.Where(q => q.StatusInfo == "Cancel").Sum(q => q.Total),
                        Total = list.Sum(q => q.Total),
                    };

                    workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Merge();
                    workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(XLColor.FromArgb(79, 129, 189, 255));
                    workSheet.Cell(lastRow, 1).Value = "Summary for Total Projects";
                    workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                    workSheet.Cell(lastRow, 1).Style.Font.SetBold();

                    lastRow++;
                    lastColor = XLColor.FromArgb(220, 230, 241, 255);


                    if (data.Win)
                    {
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                        workSheet.Cell(lastRow, 1).Value = "Total Win";
                        workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                        workSheet.Cell(lastRow, 7).Value = $"{projects.Win:N2}  " +
                                                           $"({(projects.Total != 0 ? (projects.Win / projects.Total * 100).ToString("N2") : "-")}%)";
                        workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                        lastRow++;
                        if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                        {
                            lastColor = XLColor.White;
                        }
                        else
                        {
                            lastColor = XLColor.FromArgb(220, 230, 241, 255);
                        }
                    }

                    if (data.OnGoing)
                    {
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                        workSheet.Cell(lastRow, 1).Value = "Total On Going";
                        workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                        workSheet.Cell(lastRow, 7).Value = $"{projects.OnGoing:N2}  " +
                                                           $"({(projects.Total != 0 ? (projects.OnGoing / projects.Total * 100).ToString("N2") : "-")}%)";
                        workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                        lastRow++;
                        if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                        {
                            lastColor = XLColor.White;
                        }
                        else
                        {
                            lastColor = XLColor.FromArgb(220, 230, 241, 255);
                        }
                    }

                    if (data.Hold)
                    {
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                        workSheet.Cell(lastRow, 1).Value = "Total Hold";
                        workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                        workSheet.Cell(lastRow, 7).Value = $"{projects.Hold:N2}  " +
                                                           $"({(projects.Total != 0 ? (projects.Hold / projects.Total * 100).ToString("N2") : "-")}%)";
                        workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                        lastRow++;
                        if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                        {
                            lastColor = XLColor.White;
                        }
                        else
                        {
                            lastColor = XLColor.FromArgb(220, 230, 241, 255);
                        }
                    }

                    if (data.Cancel)
                    {
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                        workSheet.Cell(lastRow, 1).Value = "Total Cancel";
                        workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                        workSheet.Cell(lastRow, 7).Value = $"{projects.Cancel:N2}  " +
                                                           $"({(projects.Total != 0 ? (projects.Cancel / projects.Total * 100).ToString("N2") : "-")}%)";
                        workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                        lastRow++;
                        if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                        {
                            lastColor = XLColor.White;
                        }
                        else
                        {
                            lastColor = XLColor.FromArgb(220, 230, 241, 255);
                        }
                    }

                    if (data.Lost)
                    {
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                        workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                        workSheet.Cell(lastRow, 1).Value = "Total Lost";
                        workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                        workSheet.Cell(lastRow, 7).Value = $"{projects.Lost:N2}  " +
                                                           $"({(projects.Total != 0 ? (projects.Lost / projects.Total * 100).ToString("N2") : "-")}%)";
                        workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                        workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                        lastRow++;
                        if (lastColor == XLColor.FromArgb(220, 230, 241, 255))
                        {
                            lastColor = XLColor.White;
                        }
                        else
                        {
                            lastColor = XLColor.FromArgb(220, 230, 241, 255);
                        }
                    }

                    workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Merge();
                    workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Merge();
                    workSheet.Range(workSheet.Cell(lastRow, 7), workSheet.Cell(lastRow, 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                    workSheet.Range(workSheet.Cell(lastRow, 1), workSheet.Cell(lastRow, 10)).Style.Fill.SetBackgroundColor(lastColor);

                    workSheet.Cell(lastRow, 1).Value = "Total Projects";
                    workSheet.Cell(lastRow, 1).Style.Font.FontSize = 14;
                    workSheet.Cell(lastRow, 1).Style.Font.SetBold();
                    workSheet.Cell(lastRow, 7).Value = $"{projects.Total:N2}";
                    workSheet.Cell(lastRow, 7).Style.Font.FontSize = 14;
                    workSheet.Cell(lastRow, 7).Style.Font.SetBold();

                    fileName = $"{worksheetName} {DateTime.Now:dd-MM-yyyy} .xlsx";
                    fileName = fileName.Replace("/", "-");

                    SaveFileDialog saveFileDialog = new()
                    {
                        FileName = fileName,
                        DefaultExt = ".xlsx",
                        Filter = "Excel Worksheets|*.xlsx",
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                }

            }
            catch (Exception ex)
            {
                _ = MessageWindow.Show("Error", ex.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }

        private class Year
        {
            public int Value { get; set; }
        }
        public class Quotation
        {
            public int Id { get; set; } //
            public int JobOrderID { get; set; }//
            public int SalesmanID { get; set; } //
            public int SN { get; set; } //
            public DateTime DateIssued { get; set; }//
            public string QuotationCode { get; set; }//
            public string Project { get; set; }//
            public string Customer { get; set; }//
            public string EstimationName { get; set; }//
            public string Salesman { get; set; }//
            public string Type { get; set; }//
            public decimal Total { get; set; }//
            public string Status { get; set; }//
            public string StatusInfo
            {
                get
                {
                    if (JobOrderID != 0)
                    {
                        return "Win";
                    }
                    else if (Status == null)
                    {
                        return "On Going";
                    }
                    else
                    {
                        return Status;
                    }
                }
            }
            public int QuotationYear { get; set; }//
            public int QuotationMonth { get; set; }//

            public string Note { get; set; } //

        }
        private class Group
        {
            public string Name { get; set; }
            public decimal Lost { get; set; }
            public decimal Win { get; set; }
            public decimal OnGoing { get; set; }
            public decimal Hold { get; set; }
            public decimal Cancel { get; set; }
            public decimal Total { get; set; }
        }
        private class ExcelQuotation
        {
            public string Date { get; set; }
            public string Project { get; set; }
            public string Customer { get; set; }
            public string Quotation { get; set; }
            public string Estimation { get; set; }
            public string Salesman { get; set; }
            public string Type { get; set; }
            public string Total { get; set; }
            public string Status { get; set; }
            public string Comment { get; set; }
        }
    }
}