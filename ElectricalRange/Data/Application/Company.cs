using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Application
{
    [Table("[Application].[Companies]")]
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public int? WatermarkId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public long VATNumber { get; set; }
        public string Code { get; set; }
    }
}
