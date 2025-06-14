using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Customers
{
    [Table("[Customer].[CustomersAttachments]")]
    internal class CustomerAttachment : AttachmentBase
    {
        public int CustomerId { get; set; }
        public string Type { get; set; }
    }
}
