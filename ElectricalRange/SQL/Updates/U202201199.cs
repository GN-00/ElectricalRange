using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201199 : SqlUpdate
    {
        public U202201199()
        {
            Key = nameof(U202201199);
            Command = "CREATE VIEW [JobOrder].[Panels(Cost)] AS " +
                      "SELECT PanelID, ROUND(Price * (1 - Discount / 100), 2) AS PanelPrice, ROUND(Price * (1 - Discount / 100) * VAT, 2) AS VATVlue, ROUND(Price * (1 - Discount / 100), 2) +ROUND(Price * (1 - Discount / 100) * VAT, 2) AS PanelFinalPrice " +
                      "FROM JobOrder.[Panels(Cost)-1] ";

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


