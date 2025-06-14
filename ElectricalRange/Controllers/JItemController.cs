using Dapper;

using ProjectsNow.Data.JobOrders;
using ProjectsNow.Enums;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class JItemController
    {
        public static ObservableCollection<JItem> PanelDetails(SqlConnection connection, int panelID)
        {
            string query = $"Select PanelID, Category, Code, Description, " +
                           $"Unit, ItemQty, Brand, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort " +
                           $"From [JobOrder].[PanelsItemsView] " +
                           $"Where PanelID = {panelID} And ItemTable = '{Tables.Details}'" +
                           $"Order By ItemSort";

            System.Collections.Generic.IEnumerable<JItem> items = connection.Query<JItem>(query);
            ObservableCollection<JItem> records = new(items);
            return records;
        }
        public static ObservableCollection<JItem> PanelEnclosure(SqlConnection connection, int panelID)
        {
            string query = $"Select PanelID, Category, Code, Description, " +
                           $"Unit, ItemQty, Brand, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort " +
                           $"From [JobOrder].[PanelsItemsView] " +
                           $"Where PanelID = {panelID} And ItemTable = '{Tables.Enclosure}'" +
                           $"Order By ItemSort";

            System.Collections.Generic.IEnumerable<JItem> items = connection.Query<JItem>(query);
            ObservableCollection<JItem> records = new(items);
            return records;
        }
    }
}
