using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[InspectionsAttachments]")]
    public class InspectionRequestAttachment : AttachmentBase
    {
        public string InspectionNumber { get; set; }
    }
}
