using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Store
{
    [Table("[Store].[InvoicesAttachments]")]
    public class InvoiceAttachment : AttachmentBase
    {
        public int InvoiceId { get; set; }
    }
}
