using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201196 : SqlUpdate
    {
        public U202201196()
        {
            Key = nameof(U202201196);
            Command = "ALTER VIEW [Quotation].[QuotationsPanelsCost] AS " +
                      "SELECT Quotation.[QuotationsPanelsCost-1].PanelID, Quotation.[QuotationsPanelsCost-1].PanelCost, ROUND((Quotation.[QuotationsPanelsCost-1].PanelCost * 1) / (1 - Quotation.QuotationsPanels.PanelProfit / 100), 2) AS PanelPrice, " +
                      "ROUND(ROUND((Quotation.[QuotationsPanelsCost-1].PanelCost * 1) / (1 - Quotation.QuotationsPanels.PanelProfit / 100), 2) * Quotation.QuotationsPanels.PanelQty, 2) AS PanelsPrice " +
                      "FROM Quotation.[QuotationsPanelsCost-1] INNER JOIN " +
                      "Quotation.QuotationsPanels ON Quotation.[QuotationsPanelsCost-1].PanelID = Quotation.QuotationsPanels.PanelID";

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


