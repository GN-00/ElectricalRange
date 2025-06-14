using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[PurchaseOrdersAttachments]")]
    internal class PurchaseOrderAttachment : AttachmentBase
    {
        public int PurchaseOrderId { get; set; }
    }
}
