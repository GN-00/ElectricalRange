using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201198 : SqlUpdate
    {
        public U202201198()
        {
            Key = nameof(U202201198);
            Command = "CREATE VIEW [JobOrder].[Panels(Cost)-1] AS " +
                      "SELECT Quotation.QuotationsPanels.PanelID, Quotation.QuotationsPanelsCost.PanelPrice AS Price, Quotation.Quotations.VAT, Quotation.Quotations.Discount " +
                      "FROM Quotation.Quotations RIGHT OUTER JOIN " +
                      "Quotation.QuotationsPanels ON Quotation.Quotations.QuotationID = Quotation.QuotationsPanels.QuotationID LEFT OUTER JOIN " +
                      "Quotation.QuotationsPanelsCost ON Quotation.QuotationsPanels.PanelID = Quotation.QuotationsPanelsCost.PanelID"; 

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


