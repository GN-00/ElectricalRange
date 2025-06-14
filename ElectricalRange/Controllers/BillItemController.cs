using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.Data;
using ProjectsNow.Enums;

namespace ProjectsNow.Controllers
{
    internal class BillItemController
    {
        public static List<BillItem> PanelsDetails(SqlConnection connection, string panelsID)
        {
            string query = $"Select * " +
                           $"From [Quotation].[QuotationsPanelsItems] " +
                           $"Where PanelID In ({panelsID}) And ItemTable = '{Tables.Details}'" +
                           $"Order By PanelID, ItemSort";

            List<BillItem> records = connection.Query<BillItem>(query).ToList();
            return records;
        }
    }
}
