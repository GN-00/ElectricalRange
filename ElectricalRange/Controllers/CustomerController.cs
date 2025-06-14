using Dapper;

using ProjectsNow.Data.Customers;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Controllers
{
    public static class CustomerController
    {
        public static ObservableCollection<Customer> GetCustomers(SqlConnection connection)
        {
            string query = $"Select * " +
                           $"From [Customer].[Customers(View)]" +
                           $"Order By CustomerName";
            ObservableCollection<Customer> customers = new(connection.Query<Customer>(query));
            return customers;
        }

        public static bool AbilityToDelete(this Customer customer, SqlConnection connection)
        {
            string query = $"Select CustomerID " +
                           $"From [Inquiry].[Inquiries] " +
                           $"Where CustomerID = {customer.CustomerID} ";
            System.Collections.Generic.List<Customer> customers = connection.Query<Customer>(query).ToList();
            return customers.Count == 0;
        }
    }
}
