using Dapper;

using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class PurchaseOrderController
    {
        public static ObservableCollection<PurchaseOrder> GetPurchaseOrders(SqlConnection connection, int jobOrderID)
        {
            string query = $"Select * From [JobOrder].[PurchaseOrders]" +
                           $"Where JobOrderID = {jobOrderID} ";

            ObservableCollection<PurchaseOrder> records = new(connection.Query<PurchaseOrder>(query));
            return records;
        }
    }
}
