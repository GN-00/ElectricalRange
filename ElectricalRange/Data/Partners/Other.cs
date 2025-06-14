using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

namespace ProjectsNow.Data.Partners
{
    [Table("[Partner].[Others]")]
    public class Other : Base, IAccess, IPartner
    {
        private string _Name;
        private string _VATNumber;

        [Key]
        public int Id { get; set; }
        public string Name
        {
            get => _Name;
            set => SetValue(ref _Name, value);
        }

        [Write(false)]
        public string Group
        {
            get => Name.Substring(0, 1).ToUpper();
        }

        public string VATNumber
        {
            get => _VATNumber;
            set => SetValue(ref _VATNumber, value);
        }
    }

    //public static class OtherController
    //{
    //    public static bool AbilityToDelete(this Other other, SqlConnection connection)
    //    {
    //        string query = $"Select OtherId " +
    //                       $"From [Accountant].[TransactionsLevel1] " +
    //                       $"Where OtherId = {other.Id} ";
    //        Other result = connection.QueryFirstOrDefault<Other>(query);

    //        return result == null;
    //    }
    //}
}
