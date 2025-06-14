using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.SQL
{
    [Table("[SQL].[Updates]")]
    public class SqlUpdate
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        [Write(false)]
        public string Command { get; set; }
        public bool IsDone { get; set; }
        public DateTime Date { get; set; }
    }
}
