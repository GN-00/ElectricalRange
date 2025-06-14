using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201202 : SqlUpdate
    {
        public U202201202()
        {
            Key = nameof(U202201202);
            Command = "CREATE VIEW [JobOrder].[Panels(View)] AS " +
                      "SELECT JobOrder.Panels.PanelID, JobOrder.Panels.JobOrderID, JobOrder.Panels.PurchaseOrdersNumber, JobOrder.Panels.PanelSN, JobOrder.Panels.PanelName, JobOrder.Panels.PanelQty, JobOrder.Panels.Status,  " +
                      "JobOrder.Panels.DateOfCreation, JobOrder.Panels.DateOfDesign, JobOrder.Panels.DateOfSendingForApproval, JobOrder.Panels.DateOfProduction, JobOrder.Panels.DateOfClosing, JobOrder.Panels.DateOfDelivery,  " +
                      "JobOrder.Panels.DateOfHolding, JobOrder.Panels.DateOfCancellation, JobOrder.[PanelsCount(Closed)].ClosedQty, JobOrder.[PanelsCount(Invoiced)].InvoicedQty, JobOrder.[PanelsCount(Delivered)].DeliveredQty,  " +
                      "JobOrder.[PanelsCount(Hold)].HoldQty, JobOrder.[PanelsCount(Canceled)].CanceledQty, JobOrder.Panels.EnclosureType, JobOrder.Panels.EnclosureHeight, JobOrder.Panels.EnclosureWidth, JobOrder.Panels.EnclosureDepth,  " +
                      "JobOrder.Panels.EnclosureIP, JobOrder.Panels.PanelProfit, JobOrder.PanelsDesignCost.PanelDesignCost, JobOrder.Panels.Revision, JobOrder.Panels.IsSpecial, JobOrder.Panels.PanelType, JobOrder.Panels.DrawingManualCode,  " +
                      "JobOrder.Panels.DrawingNo, JobOrder.[Panels(Cost)].PanelPrice AS PanelEstimatedPrice " +
                      "FROM JobOrder.Panels LEFT OUTER JOIN " +
                      "JobOrder.[Panels(Cost)] ON JobOrder.Panels.PanelID = JobOrder.[Panels(Cost)].PanelID LEFT OUTER JOIN " +
                      "JobOrder.PanelsDesignCost ON JobOrder.Panels.PanelID = JobOrder.PanelsDesignCost.PanelID LEFT OUTER JOIN " +
                      "JobOrder.[PanelsCount(Closed)] ON JobOrder.Panels.PanelID = JobOrder.[PanelsCount(Closed)].PanelID LEFT OUTER JOIN " +
                      "JobOrder.[PanelsCount(Delivered)] ON JobOrder.Panels.PanelID = JobOrder.[PanelsCount(Delivered)].PanelID LEFT OUTER JOIN " +
                      "JobOrder.[PanelsCount(Invoiced)] ON JobOrder.Panels.PanelID = JobOrder.[PanelsCount(Invoiced)].PanelID LEFT OUTER JOIN " +
                      "JobOrder.[PanelsCount(Hold)] ON JobOrder.Panels.PanelID = JobOrder.[PanelsCount(Hold)].PanelID LEFT OUTER JOIN " +
                      "JobOrder.[PanelsCount(Canceled)] ON JobOrder.Panels.PanelID = JobOrder.[PanelsCount(Canceled)].PanelID";

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


