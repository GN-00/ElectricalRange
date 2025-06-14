using Dapper;

using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class ModificationPanelController
    {
        public static ObservableCollection<ModificationPanel> GetPanels(SqlConnection connection, int jobOrderID)
        {
            string query = $"Select * From [JobOrder].[Panels(View)] Where JobOrderID = {jobOrderID}";
            ObservableCollection<ModificationPanel> records = new(connection.Query<ModificationPanel>(query));
            return records;
        }
    }
}
