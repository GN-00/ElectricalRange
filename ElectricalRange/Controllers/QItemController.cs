using Dapper;

using ProjectsNow.Data.Quotations;

using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Controllers
{
    public static class QItemController
    {
        public static List<QItem> PanelItems(SqlConnection connection, int panelID)
        {
            string query = $"Select * From [Quotation].[QuotationsPanelsItems] " +
                           $"Where PanelID = {panelID} " +
                           $"Order By ItemSort";

            List<QItem> records = connection.Query<QItem>(query).ToList();
            return records;
        }

        public static List<QItem> QuotationRecalculateItems(SqlConnection connection, int quotationID)
        {
            string query = $"Select * " +
                           $"From [Quotation].[RecalculateItems] " +
                           $"Where QuotationID = {quotationID} " +
                           $"Order By ItemSort";

            List<QItem> records = connection.Query<QItem>(query).ToList();
            return records;
        }

        public static List<QItem> PanelRecalculateItems(SqlConnection connection, int panelID)
        {
            string query = $"Select * " +
                           $"From [Quotation].[RecalculateItems] " +
                           $"Where PanelID = {panelID} " +
                           $"Order By ItemSort";

            List<QItem> records = connection.Query<QItem>(query).ToList();
            return records;
        }

    }
}
