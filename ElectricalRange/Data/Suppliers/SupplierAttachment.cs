using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Suppliers
{
    [Table("[Supplier].[SuppliersAttachments]")]
    internal class SupplierAttachment : AttachmentBase
    {
        public int SupplierId { get; set; }
        public string Type { get; set; }
    }
}
