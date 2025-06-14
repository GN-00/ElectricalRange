using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Users
{
    [Table("[User].[Attachments]")]
    internal class UserAttachment : AttachmentBase
    {
        public int UserId { get; set; }
    }
}
