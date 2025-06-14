using Dapper;

using ProjectsNow.Data.Inquiries;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class SalesmanController
    {
        public static ObservableCollection<Salesman> GetSalesmen(SqlConnection connection)
        {
            string query = $"Select * " +
                           $"From [User].[Salesmen] " +
                           $"Order By Name";

            ObservableCollection<Salesman> salesmen =
                new(connection.Query<Salesman>(query)) { new Salesman() };

            return salesmen;
        }
    }
}