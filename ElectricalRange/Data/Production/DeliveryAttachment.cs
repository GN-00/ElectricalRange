using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Production
{
    [Table("[Production].[DeliveriesAttachments]")]

    public class DeliveryAttachment : AttachmentBase
    {
        public string DeliveryNumber { get; set; }
    }
}