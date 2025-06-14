using ClosedXML.Excel;

using Dapper;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;

using QuotationItem = ProjectsNow.Printing.QuotationPages.CostSheet.Item;
using PanelItem = ProjectsNow.Printing.QuotationPages.PanelCostSheet.Item;
using Summary = ProjectsNow.Printing.QuotationPages.CostSheet.Summary;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class SelectCostSheetViewModel : ViewModelBase
    {
        public SelectCostSheetViewModel(Quotation quotation, IView checkPoint)
        {
            ViewData = checkPoint;
            QuotationData = quotation;

            PrintCommand = new RelayCommand(Print, CanPrint);
            ExportCommand = new RelayCommand(Export, CanExport);
        }

        public SelectCostSheetViewModel(Quotation quotation, QPanel panel, IView checkPoint)
        {
            ViewData = checkPoint;
            PanelData = panel;
            QuotationData = quotation;

            PrintCommand = new RelayCommand(PanelPrint, CanPanelPrint);
            ExportCommand = new RelayCommand(PanelExport, CanPanelExport);
        }

        public QPanel PanelData { get; }
        public Quotation QuotationData { get; }
        public RelayCommand PrintCommand { get; }
        public RelayCommand ExportCommand { get; }

        private void Print()
        {
            List<QuotationItem> items;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"SELECT * " +
                               $"FROM Quotation.CostSheet " +
                               $"WHERE QuotationID = {QuotationData.QuotationID} " +
                               $"ORDER BY Code";

                items = connection.Query<QuotationItem>(query).ToList();
            }

            for (int i = 1; i <= items.Count; i++)
            {
                items[i - 1].Sort = i;
            }

            decimal pagesNumber = items.Count / 45m;
            if (pagesNumber - Math.Truncate(pagesNumber) != 0)
            {
                pagesNumber = Math.Truncate(pagesNumber) + 1;
            }

            List<FrameworkElement> elements = new();
            if (pagesNumber != 0)
            {
                for (int i = 1; i <= pagesNumber; i++)
                {
                    Printing.QuotationPages.CostSheet.CostSheetPage costSheetPage =
                        new()
                        {
                            QuotationData = QuotationData,
                            Items = items.Where(p => p.Sort > ((i - 1) * 45) && p.Sort <= (i * 45)).ToList(),
                            Page = i,
                            Pages = Convert.ToInt32(pagesNumber)
                        };

                    if (i == pagesNumber)
                    {
                        decimal total = items.Sum(item => item.Total);
                        costSheetPage.TotalPrice.Text = total.ToString("N2");
                        costSheetPage.TotalPercentage.Text = (total / total).ToString("P");

                        decimal enclosurePrice = items.Where(item => item.Type == "Enclosure").Sum(item => item.Total);
                        costSheetPage.EnclosurePrice.Text = enclosurePrice.ToString("N2");
                        costSheetPage.EnclosurePercentage.Text = (enclosurePrice / total).ToString("P");

                        decimal schneiderPrice = items.Where(item => item.Type == "Schneider").Sum(item => item.Total);
                        costSheetPage.SchneiderPrice.Text = schneiderPrice.ToString("N2");
                        costSheetPage.SchneiderPercentage.Text = (schneiderPrice / total).ToString("P");

                        decimal copperPrice = items.Where(item => item.Type == "Copper").Sum(item => item.Total);
                        costSheetPage.CopperPrice.Text = copperPrice.ToString("N2");
                        costSheetPage.CopperPercentage.Text = (copperPrice / total).ToString("P");

                        decimal otherPrice = items.Where(item => item.Type == "Other").Sum(item => item.Total);
                        costSheetPage.OtherPrice.Text = otherPrice.ToString("N2");
                        costSheetPage.OtherPercentage.Text = (otherPrice / total).ToString("P");

                        costSheetPage.DetailsList.Visibility = Visibility.Visible;
                    }

                    elements.Add(costSheetPage);
                }

                Printing.Print.PrintPreview(elements, $"Cost Sheet Q.Code {QuotationData.QuotationCode}", ViewData);
            }
            else
            {
                _ = MessageView.Show("Items", "There are no items!!", MessageViewButton.OK, MessageViewImage.Warning);
            }

            Navigation.ClosePopup();
        }
        private bool CanPrint()
        {
            return true;
        }

        private void Export()
        {
            try
            {
                List<QuotationItem> items;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"SELECT * " +
                                   $"FROM Quotation.CostSheet " +
                                   $"WHERE QuotationID = {QuotationData.QuotationID} " +
                                   $"ORDER BY Code";

                    items = connection.Query<QuotationItem>(query).ToList();
                }

                if (items.Count != 0)
                {
                    string fileName;
                    using XLWorkbook workbook = new();
                    DataTable table = new();
                    using (ObjectReader reader = ObjectReader.Create(items))
                    {
                        table.Load(reader);
                    }
                    table.Columns["Code"].SetOrdinal(0);
                    table.Columns["Description"].SetOrdinal(1);
                    table.Columns["PublicPrice"].SetOrdinal(2);
                    table.Columns["Discount"].SetOrdinal(3);
                    table.Columns["Price"].SetOrdinal(4);
                    table.Columns["Qty"].SetOrdinal(5);
                    table.Columns["Total"].SetOrdinal(6);


                    string worksheetName = "Cost Sheet";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);
                    workSheet.Cell(1, 3).Value = "Public Price";
                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();
                    _ = workSheet.Column(7).AdjustToContents();

                    workSheet.Column(13).Delete();
                    workSheet.Column(12).Delete();
                    workSheet.Column(11).Delete();
                    workSheet.Column(10).Delete();
                    workSheet.Column(9).Delete();
                    workSheet.Column(8).Delete();

                    decimal total = items.Sum(item => item.Total);
                    decimal enclosurePrice = items.Where(item => item.Type == "Enclosure").Sum(item => item.Total);
                    decimal schneiderPrice = items.Where(item => item.Type == "Schneider").Sum(item => item.Total);
                    decimal copperPrice = items.Where(item => item.Type == "Copper").Sum(item => item.Total);
                    decimal otherPrice = items.Where(item => item.Type == "Other").Sum(item => item.Total);

                    List<Summary> summaries = new()
                            {
                                new Summary()
                                {
                                    Category = "Enclosure",
                                    Total = enclosurePrice,
                                    Percentage = enclosurePrice / total * 100
                                },

                                new Summary()
                                {
                                    Category = "Schneider",
                                    Total = schneiderPrice,
                                    Percentage = schneiderPrice / total * 100
                                },

                                new Summary()
                                {
                                    Category = "Copper",
                                    Total = copperPrice,
                                    Percentage = copperPrice / total * 100
                                },

                                new Summary()
                                {
                                    Category = "Other",
                                    Total = otherPrice,
                                    Percentage = otherPrice / total * 100
                                },

                                new Summary()
                                {
                                    Category = "Total",
                                    Total = total,
                                    Percentage = 100
                                },
                            };

                    table = new DataTable();
                    using (ObjectReader reader = ObjectReader.Create(summaries))
                    {
                        table.Load(reader);
                    }
                    table.Columns["Category"].SetOrdinal(0);
                    table.Columns["Total"].SetOrdinal(1);
                    table.Columns["Percentage"].SetOrdinal(2);


                    worksheetName = "Summary";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    workSheet = workbook.Worksheet(worksheetName);
                    workSheet.Cell(1, 3).Value = "Percentage %";
                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();

                    fileName = $"Cost Sheet {QuotationData.QuotationCode}.xlsx";
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
                else
                {
                    _ = MessageView.Show("Items", "There are no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _ = MessageView.Show("Error", ex.Message, MessageViewButton.OK, MessageViewImage.Warning);
            }

            Navigation.ClosePopup();
        }
        private bool CanExport()
        {
            return true;
        }

        private void PanelPrint()
        {
            List<PanelItem> items;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * " +
                               $"From [Quotation].[PanelCostSheet] " +
                               $"Where PanelID = {PanelData.PanelID} " +
                               $"Order By Code";

                items = connection.Query<PanelItem>(query).ToList();
            }

            if (items.Count == 0)
            {
                _ = MessageView.Show("Items", "There are no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                return;
            }

            for (int i = 1; i <= items.Count; i++)
            {
                items[i - 1].Sort = i;
            }

            decimal pagesNumber = items.Count / 45m;
            if (pagesNumber - Math.Truncate(pagesNumber) != 0)
            {
                pagesNumber = Math.Truncate(pagesNumber) + 1;
            }

            List<FrameworkElement> elements = new();
            if (pagesNumber != 0)
            {
                for (int i = 1; i <= pagesNumber; i++)
                {
                    Printing.QuotationPages.PanelCostSheet.CostSheetPage costSheetPage =
                        new()
                        {
                            PanelData = PanelData,
                            QuotationData = QuotationData,
                            Items = items.Where(p => p.Sort > ((i - 1) * 45) && p.Sort <= (i * 45)).ToList(),
                            Page = i,
                            Pages = Convert.ToInt32(pagesNumber)
                        };

                    if (i == pagesNumber)
                    {
                        decimal total = items.Sum(item => item.Total);
                        costSheetPage.TotalPrice.Text = total.ToString("N2");
                        costSheetPage.TotalPercentage.Text = (total / total).ToString("P");

                        decimal enclosurePrice = items.Where(item => item.Type == "Enclosure").Sum(item => item.Total);
                        costSheetPage.EnclosurePrice.Text = enclosurePrice.ToString("N2");
                        costSheetPage.EnclosurePercentage.Text = (enclosurePrice / total).ToString("P");

                        decimal schneiderPrice = items.Where(item => item.Type == "Schneider").Sum(item => item.Total);
                        costSheetPage.SchneiderPrice.Text = schneiderPrice.ToString("N2");
                        costSheetPage.SchneiderPercentage.Text = (schneiderPrice / total).ToString("P");

                        decimal copperPrice = items.Where(item => item.Type == "Copper").Sum(item => item.Total);
                        costSheetPage.CopperPrice.Text = copperPrice.ToString("N2");
                        costSheetPage.CopperPercentage.Text = (copperPrice / total).ToString("P");

                        decimal otherPrice = items.Where(item => item.Type == "Other").Sum(item => item.Total);
                        costSheetPage.OtherPrice.Text = otherPrice.ToString("N2");
                        costSheetPage.OtherPercentage.Text = (otherPrice / total).ToString("P");

                        costSheetPage.DetailsList.Visibility = Visibility.Visible;
                    }

                    elements.Add(costSheetPage);
                }

                Printing.Print.PrintPreview(elements, $"Cost Sheet Q.Code {QuotationData.QuotationCode}", ViewData);
            }

            Navigation.ClosePopup();
        }
        private bool CanPanelPrint()
        {
            return true;
        }

        private void PanelExport()
        {
            try
            {
                List<PanelItem> items;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"SELECT * " +
                                   $"FROM Quotation.PanelCostSheet " +
                                   $"WHERE PanelID = {PanelData.PanelID} " +
                                   $"ORDER BY Code";

                    items = connection.Query<PanelItem>(query).ToList();
                }

                if (items.Count != 0)
                {
                    string fileName;
                    using XLWorkbook workbook = new();
                    DataTable table = new();
                    using (ObjectReader reader = ObjectReader.Create(items))
                    {
                        table.Load(reader);
                    }
                    table.Columns["Code"].SetOrdinal(0);
                    table.Columns["Description"].SetOrdinal(1);
                    table.Columns["PublicPrice"].SetOrdinal(2);
                    table.Columns["Discount"].SetOrdinal(3);
                    table.Columns["Price"].SetOrdinal(4);
                    table.Columns["Qty"].SetOrdinal(5);
                    table.Columns["Total"].SetOrdinal(6);


                    string worksheetName = "Cost Sheet";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);
                    workSheet.Cell(1, 3).Value = "Public Price";
                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();
                    _ = workSheet.Column(7).AdjustToContents();

                    workSheet.Column(13).Delete();
                    workSheet.Column(12).Delete();
                    workSheet.Column(11).Delete();
                    workSheet.Column(10).Delete();
                    workSheet.Column(9).Delete();
                    workSheet.Column(8).Delete();

                    decimal total = items.Sum(item => item.Total);
                    decimal enclosurePrice = items.Where(item => item.Type == "Enclosure").Sum(item => item.Total);
                    decimal schneiderPrice = items.Where(item => item.Type == "Schneider").Sum(item => item.Total);
                    decimal copperPrice = items.Where(item => item.Type == "Copper").Sum(item => item.Total);
                    decimal otherPrice = items.Where(item => item.Type == "Other").Sum(item => item.Total);

                    List<Summary> summaries = new()
                        {
                            new Summary()
                            {
                                Category = "Enclosure",
                                Total = enclosurePrice,
                                Percentage = enclosurePrice / total * 100
                            },

                            new Summary()
                            {
                                Category = "Schneider",
                                Total = schneiderPrice,
                                Percentage = schneiderPrice / total * 100
                            },

                            new Summary()
                            {
                                Category = "Copper",
                                Total = copperPrice,
                                Percentage = copperPrice / total * 100
                            },

                            new Summary()
                            {
                                Category = "Other",
                                Total = otherPrice,
                                Percentage = otherPrice / total * 100
                            },

                            new Summary()
                            {
                                Category = "Total",
                                Total = total,
                                Percentage = 100
                            },

                        };

                    table = new DataTable();
                    using (ObjectReader reader = ObjectReader.Create(summaries))
                    {
                        table.Load(reader);
                    }
                    table.Columns["Category"].SetOrdinal(0);
                    table.Columns["Total"].SetOrdinal(1);
                    table.Columns["Percentage"].SetOrdinal(2);

                    worksheetName = "Summary";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    workSheet = workbook.Worksheet(worksheetName);
                    workSheet.Cell(1, 3).Value = "Percentage %";
                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();

                    fileName = $"Cost Sheet {QuotationData.QuotationCode} {PanelData.PanelName}.xlsx";
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
                else
                {
                    _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _ = MessageView.Show("Error", ex.Message, MessageViewButton.OK, MessageViewImage.Warning);
            }

            Navigation.ClosePopup();
        }
        private bool CanPanelExport()
        {
            return true;
        }
    }
}