using System.Collections.ObjectModel;
using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.Data.Customers;

namespace ProjectsNow.Controllers
{
    public static class ConsultantController
    {
        public static ObservableCollection<Consultant> GetConsultants(SqlConnection connection)
        {
            string query = "Select * From [Customer].[Consultants] " +
                           $"Order By ConsultantName";
            ObservableCollection<Consultant> records = new(connection.Query<Consultant>(query));
            return records;
        }

        public static bool AbilityToDelete(this Consultant consultant, SqlConnection connection)
        {
            string query = $"Select ConsultantID " +
                           $"From [Inquiries] " +
                           $"Where ConsultantID = {consultant.ConsultantID} ";
            System.Collections.Generic.List<Consultant> records = connection.Query<Consultant>(query).ToList();
            return records.Count == 0;
        }
    }
}
