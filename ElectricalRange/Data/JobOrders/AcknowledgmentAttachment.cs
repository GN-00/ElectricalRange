using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{

    [Table("[JobOrder].[AcknowledgmentsAttachments]")]
    internal class AcknowledgmentAttachment : AttachmentBase
    {
        public int JobOrderID { get; set; }
    }
}
