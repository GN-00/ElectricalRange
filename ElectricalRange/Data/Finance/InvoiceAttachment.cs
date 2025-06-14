using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Finance
{

    [Table("[Finance].[CustomersInvoicesAttachments]")]
    internal class InvoiceAttachment : AttachmentBase
    {
        public int InvoiceId { get; set; }
    }
}
