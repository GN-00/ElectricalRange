using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.Data;

namespace ProjectsNow.Controllers
{
    public class BillPanelController
    {
        public static List<BillPanel> GetBillPanels(SqlConnection connection, int quotationID)
        {
            string query = $"Select * From [Quotation].[BillsPanels] " +
                           $"Where QuotationID = {quotationID} Order By PanelSN";

            List<BillPanel> panels = connection.Query<BillPanel>(query).ToList();
            return panels;
        }

        public static List<BillPanel> GetBillPanels(SqlConnection connection, string panelsIDs)
        {
            string query = $"Select * From [Quotation].[BillsPanels] " +
                           $"Where PanelID In({panelsIDs}) " +
                           $"Order By PanelSN";

            List<BillPanel> panels = connection.Query<BillPanel>(query).ToList();
            return panels;
        }
    }
}
