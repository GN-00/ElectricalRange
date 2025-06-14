using Dapper;

using ProjectsNow.Data.References;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class ReferenceController
    {
        public static ObservableCollection<Reference> GetReferences(SqlConnection connection)
        {
            string query = $"Select * From [Reference].[References(View)] " +
                           $"Where Hide = 0 " +
                           $"Order By Code";
            ObservableCollection<Reference> records = new(connection.Query<Reference>(query));

            return records;
        }

        public static ObservableCollection<string> GetArticle1(SqlConnection connection)
        {
            ObservableCollection<string> records =
                new(connection.Query<string>("Select Article From [Quotation].[Articles] Order By Sort"));

            return records;
        }

        public static ObservableCollection<string> GetArticle2(SqlConnection connection)
        {
            ObservableCollection<string> records =
               new(connection.Query<string>("Select Article2 From [Quotation].[QuotationsPanelsItems] Group By Article2 Order By Article2"));

            return records;
        }

        internal static ObservableCollection<string> GetRemarks(SqlConnection connection)
        {
            return new ObservableCollection<string>()
            {
                "Icu = 6kA",
                "Icu = 10kA",
                "Icu = 15kA",
                "Icu = 18kA",
                "Icu = 20kA",
                "Icu = 25kA",
                "Icu = 36kA",
                "Icu = 40kA",
                "Icu = 50kA",
                "Icu = 65kA",
                "Icu = 70kA",
                "Icu = 85kA",
                "Icu = 100kA",
            };
        }
    }
}
