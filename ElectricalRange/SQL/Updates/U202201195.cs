using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201195 : SqlUpdate
    {
        public U202201195()
        {
            Key = nameof(U202201195);
            Command = "ALTER VIEW [Quotation].[QuotationsPanelsCost-1] AS " +
                      "SELECT PanelID, ROUND(SUM((ItemQty * ItemCost) * (1 - ItemDiscount / 100)), 2) AS PanelCost " +
                      "FROM Quotation.QuotationsPanelsItems " +
                      "GROUP BY PanelID"; 

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


