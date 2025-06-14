using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[ProductionAttachments]")]
    public class ProductionAttachment : AttachmentBase
    {
        public string ProductionNumber { get; set; }
    }
}
