using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.References
{
    [Table("[Reference].[SuppliersCodes]")]
    public class SupplierReference
    {
        [Key]
        public int Id { get; set; }
        public string SupplierCode { get; set; }
        public string Code { get; set; }
    }
}
