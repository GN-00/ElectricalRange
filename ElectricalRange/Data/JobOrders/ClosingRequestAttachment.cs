using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[ClosingAttachments]")]
    public class ClosingRequestAttachment : AttachmentBase
    {
        public string ClosingNumber { get; set; }
    }
}
