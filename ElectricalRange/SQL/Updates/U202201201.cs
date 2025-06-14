using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201201 : SqlUpdate
    {
        public U202201201()
        {
            Key = nameof(U202201201);
            Command = "ALTER VIEW [JobOrder].[Panels(InvoiceDetails)] AS " +
                      "SELECT JobOrder.PanelsTransactions.Reference AS InvoiceNumber, JobOrder.PanelsTransactions.PanelID, JobOrder.Panels.PurchaseOrdersNumber, JobOrder.Panels.PanelSN, JobOrder.Panels.PanelName, " +
                      "JobOrder.PanelsTransactions.Qty AS InvoicedQty, JobOrder.Panels.PanelType, Quotation.Quotations.VAT, Quotation.Quotations.Discount, Quotation.QuotationsPanels.PanelProfit, JobOrder.Panels.IsSpecial, " +
                      "JobOrder.[Panels(Cost)].PanelPrice, JobOrder.[Panels(Cost)].PanelPrice * JobOrder.PanelsTransactions.Qty AS PanelsPrice, JobOrder.[Panels(Cost)].VATVlue AS PanelVATValue, " +
                      "JobOrder.[Panels(Cost)].VATVlue * JobOrder.PanelsTransactions.Qty AS PanelsVATValue, JobOrder.[Panels(Cost)].PanelFinalPrice, JobOrder.[Panels(Cost)].PanelFinalPrice * JobOrder.PanelsTransactions.Qty AS PanelsFinalPrice " +
                      "FROM JobOrder.[Panels(Cost)] RIGHT OUTER JOIN " +
                      "JobOrder.PanelsTransactions ON JobOrder.[Panels(Cost)].PanelID = JobOrder.PanelsTransactions.PanelID LEFT OUTER JOIN " +
                      "Quotation.QuotationsPanels ON JobOrder.PanelsTransactions.PanelID = Quotation.QuotationsPanels.PanelID LEFT OUTER JOIN " +
                      "JobOrder.Panels ON JobOrder.PanelsTransactions.PanelID = JobOrder.Panels.PanelID LEFT OUTER JOIN " +
                      "Quotation.Quotations RIGHT OUTER JOIN " +
                      "JobOrder.JobOrders ON Quotation.Quotations.QuotationID = JobOrder.JobOrders.QuotationID ON JobOrder.Panels.JobOrderID = JobOrder.JobOrders.ID " +
                      "WHERE(JobOrder.PanelsTransactions.Action = N'Invoiced')";

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


