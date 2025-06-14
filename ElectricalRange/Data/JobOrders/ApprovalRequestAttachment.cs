using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{

    [Table("[JobOrder].[ApprovalsAttachments]")]
    public class ApprovalRequestAttachment : AttachmentBase
    {
        public string ApprovalNumber { get; set; }
    }
}
