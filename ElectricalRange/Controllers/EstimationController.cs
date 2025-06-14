using Dapper;

using ProjectsNow.Data.Inquiries;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class EstimationController
    {
        public static ObservableCollection<Estimation> GetEstimation(SqlConnection connection)
        {
            string query = $"Select * " +
                           $"From [User].[Estimations] " +
                           $"Order By Name";

            ObservableCollection<Estimation> estimations =
                new(connection.Query<Estimation>(query)) { new Estimation() };
            return estimations;
        }
    }
}
