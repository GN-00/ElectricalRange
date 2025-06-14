using Dapper;
using Dapper.Contrib.Extensions;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Data.References
{
    [Table("[Reference].[SearchKeys]")]
    public class SearchKey : Base
    {
        [Key]
        public int Id { get; set; }
        private string _Key;
        public string Key
        {
            get => _Key;
            set => SetValue(ref _Key, value);
        }


        public static ObservableCollection<SearchKey> GetKeys(SqlConnection connection)
        {
            string query = "Select * From [Reference].[SearchKeys] Order By [Key]";
            IEnumerable<SearchKey> list = connection.Query<SearchKey>(query);
            return new ObservableCollection<SearchKey>(list);
        }
    }
}
