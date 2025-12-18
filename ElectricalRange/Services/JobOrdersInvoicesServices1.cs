using Dapper;

using Microsoft.Data.SqlClient;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Finance;
using ProjectsNow.Printing;
using ProjectsNow.Printing.Finance;
using ProjectsNow.Views;
using ProjectsNow.Windows.MessageWindows;

using System.Globalization;
using System.Windows;
using System.Windows.Media;

using Invoice = ProjectsNow.Data.Finance.Invoice;

namespace ProjectsNow.Services
{

    public static class JobOrdersInvoicesService
    {
        #region Constants

        private const double FirstPageHeight = 620;
        private const double OtherPageHeight = 800;
        private const double RowMinHeight = 42;

        private const double ColumnWidthNormal = 300;
        private const double ColumnWidthWide = 320;

        private const double FontSize = 14;

        private static readonly FontFamily DefaultFontFamily = new("Calibri");
        private static readonly FontWeight DefaultFontWeight = FontWeights.Bold;

        #endregion

        #region ===================== PUBLIC API =====================

        public static void PrintInvoice(string invoiceNumber, IView checkPoint = null)
        {
            bool isNormalForm = IsNormalInvoice(invoiceNumber);

            var invoice = LoadInvoice(invoiceNumber);
            var items = LoadInvoiceItems(invoice.Id);

            NormalizeReturnInvoice(invoice, items);
            AssignSerialNumbers(items);

            double columnWidth = isNormalForm ? ColumnWidthNormal : ColumnWidthWide;

            var pages = Paginate(
                items,
                columnWidth,
                FirstPageHeight,
                OtherPageHeight);

            var forms = BuildNormalInvoicePages(
                pages,
                invoice,
                askWatermark: true);

            Print.PrintPreview(forms, $"Invoice-{invoiceNumber}", checkPoint);
        }

        public static void PrintPercentageInvoice(
            Invoice invoice,
            List<InvoiceItem> panels,
            MessageBoxResult watermarkResult,
            IView checkPoint)
        {
            if (invoice.Code == "202412224")
            {
                // PercentageInvoice202412224(invoice, panels, watermarkResult, checkPoint);
                return;
            }

            AssignSerialNumbers(panels);

            var pages = Paginate(
                panels,
                ColumnWidthNormal,
                FirstPageHeight,
                OtherPageHeight,
                cloneItems: true);

            var forms = BuildPercentageInvoicePages(
                pages,
                invoice,
                watermarkResult);

            Print.PrintPreview(forms, $"Invoice-{invoice.Code}", checkPoint);
        }

        #endregion

        #region ===================== PAGINATION ENGINE =====================

        private static List<List<InvoiceItem>> Paginate(
            List<InvoiceItem> items,
            double columnWidth,
            double firstPageHeight,
            double otherPageHeight,
            bool cloneItems = false)
        {
            var pages = new List<List<InvoiceItem>>();
            var currentPage = new List<InvoiceItem>();

            double currentHeight = 0;
            int pageIndex = 0;

            foreach (var item in items)
            {
                var panel = cloneItems ? Clone(item) : item;

                double rowHeight = CalculateRowHeight(panel.Description, columnWidth);
                double pageLimit = pageIndex == 0 ? firstPageHeight : otherPageHeight;

                if (currentHeight + rowHeight > pageLimit)
                {
                    pages.Add(currentPage);
                    currentPage = new List<InvoiceItem>();
                    currentHeight = 0;
                    pageIndex++;
                }

                currentPage.Add(panel);
                currentHeight += rowHeight;
            }

            if (currentPage.Any())
                pages.Add(currentPage);

            return pages;
        }

        private static double CalculateRowHeight(string text, double width)
        {
            double textHeight = MeasureTextHeight(text, width);
            return Math.Max(RowMinHeight, textHeight + 10);
        }

        #endregion

        #region ===================== TEXT MEASUREMENT =====================

        private static double MeasureTextHeight(string text, double width)
        {
            var ft = new FormattedText(
                text ?? string.Empty,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    DefaultFontFamily,
                    FontStyles.Normal,
                    DefaultFontWeight,
                    FontStretches.Normal),
                FontSize,
                Brushes.Black,
                1.0);

            ft.MaxTextWidth = width;
            return ft.Height;
        }

        #endregion

        #region ===================== PAGE BUILDERS =====================

        private static List<FrameworkElement> BuildNormalInvoicePages(
            List<List<InvoiceItem>> pages,
            Invoice invoice,
            bool askWatermark)
        {
            var result = new List<FrameworkElement>();

            MessageBoxResult watermarkResult = MessageBoxResult.No;
            if (askWatermark)
            {
                watermarkResult = MessageWindow.Show(
                    "Printing",
                    "Print with watermark?",
                    MessageWindowButton.YesNo,
                    MessageWindowImage.Question);
            }

            for (int i = 0; i < pages.Count; i++)
            {
                var form = new PanelsInvoiceForm
                {
                    PageNumber = i + 1,
                    TotalPages = pages.Count,
                    PanelsData = pages[i],
                    InvoiceData = invoice
                };

                if (i == pages.Count - 1)
                {
                    form.TableTotal = true;
                    form.NetPrice = invoice.NetPrice;
                    form.VATPercentage = invoice.VATPercentage;
                    form.VATValue = invoice.VATValue;
                    form.GrossPrice = invoice.GrossPrice;
                }

                ApplyWatermark(form, watermarkResult);
                result.Add(form);
            }

            return result;
        }

        private static List<FrameworkElement> BuildPercentageInvoicePages(
            List<List<InvoiceItem>> pages,
            Invoice invoice,
            MessageBoxResult watermarkResult)
        {
            var result = new List<FrameworkElement>();

            for (int i = 0; i < pages.Count; i++)
            {
                var form = new PanelsInvoicePercentageForm
                {
                    Page = i + 1,
                    Pages = pages.Count,
                    PanelsData = pages[i],
                    InvoiceData = invoice,
                    VATPercentage = invoice.VATPercentage
                };

                if (i == pages.Count - 1)
                {
                    form.NetPrice = invoice.NetPrice;
                    form.VATValue = invoice.VATValue;
                    form.GrossPrice = invoice.GrossPrice;
                }

                ApplyWatermark(form, watermarkResult);
                result.Add(form);
            }

            return result;
        }

        private static void ApplyWatermark(dynamic form, MessageBoxResult watermarkResult)
        {
            if (watermarkResult == MessageBoxResult.Yes)
            {
                form.BackgroundImage.Source = AppData.CompanyWatermark;
            }
        }

        #endregion

        #region ===================== HELPERS =====================

        private static void AssignSerialNumbers(List<InvoiceItem> items)
        {
            int sn = 1;
            foreach (var item in items)
                item.SN = sn++;
        }

        private static InvoiceItem Clone(InvoiceItem source)
        {
            var clone = new InvoiceItem();
            clone.Update(source);
            return clone;
        }

        #endregion

        #region ===================== DATABASE =====================

        private static Invoice LoadInvoice(string invoiceNumber)
        {
            using SqlConnection connection = new(Database.ConnectionString);

            return connection.QueryFirstOrDefault<Invoice>(
                "SELECT * FROM [Finance].[CustomersInvoices(View)] WHERE Code = @Code",
                new { Code = invoiceNumber });
        }

        private static List<InvoiceItem> LoadInvoiceItems(int invoiceId)
        {
            using SqlConnection connection = new(Database.ConnectionString);

            return connection.Query<InvoiceItem>(
                "SELECT * FROM [Finance].[CustomersInvoicesItems] WHERE InvoiceId = @Id",
                new { Id = invoiceId }).ToList();
        }

        #endregion

        #region ===================== BUSINESS RULES =====================

        private static bool IsNormalInvoice(string invoiceNumber)
        {
            string[] special =
            {
                "202209067","202209068","202209069",
                "202211097","202211100","202211114",
                "202210081","202211104","202211106",
                "202406115"
            };

            return !special.Contains(invoiceNumber);
        }

        private static void NormalizeReturnInvoice(
            Invoice invoice,
            List<InvoiceItem> items)
        {
            if (!invoice.IsReturn)
                return;

            invoice.NetPrice *= -1;
            invoice.VATValue *= -1;
            invoice.GrossPrice *= -1;

            foreach (var item in items)
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

        #endregion
    }

}
