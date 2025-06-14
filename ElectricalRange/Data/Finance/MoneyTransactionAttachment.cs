using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[TransactionsAttachments]")]
    internal class TransactionAttachment : AttachmentBase
    {
        public int TransactionId { get; set; }
    }
}
