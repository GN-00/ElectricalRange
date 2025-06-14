using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Application
{
    [Table("[Application].[CompaniesAttachments]")]
    internal class CompanyAttachment : AttachmentBase
    {
        public int CompanyId { get; set; }
    }
}
