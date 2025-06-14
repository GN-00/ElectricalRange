using Dapper;

using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class QuotationOptionController
    {
        public static ObservableCollection<QuotationOption> GetOptions(SqlConnection connection, int quotationID)
        {
            string query = $"Select * From [Quotation].[QuotationsOptions] " +
                           $"Where QuotationID = {quotationID} ";

            ObservableCollection<QuotationOption> records = new(connection.Query<QuotationOption>(query));
            return records;
        }
    }
}
