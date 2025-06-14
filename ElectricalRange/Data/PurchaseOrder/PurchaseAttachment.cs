using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.PurchaseOrder
{
    [Table("[Purchase].[Attachments]")]
    public class PurchaseAttachment : AttachmentBase, IAttachment
    {
        public int PurchaseOrderId { get; set; }
    }
}
