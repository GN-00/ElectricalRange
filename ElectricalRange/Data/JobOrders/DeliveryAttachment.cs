using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[DeliveriesAttachments]")]
    internal class DeliveryAttachment : AttachmentBase
    {
        public string DeliveryNumber { get; set; }
    }
}
