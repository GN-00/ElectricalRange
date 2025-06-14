using Dapper;

using ProjectsNow.Data.JobOrders;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Controllers
{
    public static class ModificationItemController
    {
        public static ObservableCollection<ModificationItem> GetModificationsItems(SqlConnection connection, int jobOrderID)
        {
            string query = $"Select * From [JobOrder].[ModificationsItems] Where JobOrderID = {jobOrderID}";
            ObservableCollection<ModificationItem> records = new(connection.Query<ModificationItem>(query));
            return records;
        }

        public static List<ModificationItem> GetAllItems(SqlConnection connection, int jobOrderID)
        {
            string query = $"Select * From [JobOrder].[PanelsItemsView] Where JobOrderID = {jobOrderID} Order By JobOrderID, PanelID, ItemTable, ItemSort ";
            List<ModificationItem> records = connection.Query<ModificationItem>(query).ToList();
            return records;
        }
    }
}
