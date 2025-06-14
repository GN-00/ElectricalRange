using Dapper.Contrib.Extensions;

using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Views;
using ProjectsNow.Views.FinanceView;

using System.Collections.ObjectModel;
using System.Data;

using TransactionPanel = ProjectsNow.Data.JobOrders.TransactionPanel;

namespace ProjectsNow.Services
{
    internal static class FinanceServices
    {
        public static User UserData => Navigation.UserData;

        internal static void GetJobOrderInvoices(JobOrder order, IView checkPoint)
        {
            if (UserData.Access(order))
            {
                Navigation.To(new JobOrderInvoicesView(order), checkPoint);
            }
        }
        internal static bool CanGetJobOrderInvoices(JobOrder order)
        {
            if (order == null)
                return false;

            return true;
        }

        public static bool SaveInvoice(this IDbConnection connection, JobOrderInvoice invoice, ObservableCollection<TransactionPanel> panels)
        {
            Invoice newInvoice = new()
            {
                JobOrderId = invoice.JobOrderID,
                Code = invoice.Number,
                CustomerId = invoice.CustomerID,
                Date = invoice.Date,
                Number = invoice.CodeNumber,
                Year = invoice.Date.Year,
                Month = invoice.Date.Month,
                NetPrice = (double)invoice.NetPrice,
                VAT = (double)invoice.VAT,
                VATPercentage = (double)invoice.VATPercentage,
                VATValue = (double)invoice.VATValue,
                GrossPrice = (double)invoice.GrossPrice,
                Type = "Panels"
            };

            _ = connection.Insert(newInvoice);

            ObservableCollection<InvoiceItem> items = new();
            foreach (TransactionPanel panel in panels)
            {
                InvoiceItem item = new()
                {
                    InvoiceId = newInvoice.Id,
                    PanelId = panel.PanelID,
                    SN = panel.PanelSN.Value,
                    Description = panel.PanelName,
                    ArabicDescription = panel.PanelTypeArabicInfo,
                    Qty = panel.Qty,
                    NetPrice = (double)panel.NetPrice,
                    UnitNetPrice = (double)panel.UnitNetPrice,
                    VAT = (double)invoice.VAT,
                    VATValue = (double)panel.VATValue,
                    UnitVATValue = (double)panel.UnitVATValue,
                    GrossPrice = (double)panel.GrossPrice,
                    UnitGrossPrice = (double)panel.UnitGrossPrice,
                    Code = invoice.JobOrderCode + "-" + panel.PanelSN
                };

                items.Add(item);
            }

            _ = connection.Insert(items);

            return true;
        }
    }
}
