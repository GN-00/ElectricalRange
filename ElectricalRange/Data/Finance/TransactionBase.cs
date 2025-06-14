using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Finance
{
    public class TransactionBase : Base
    {
        public int? AttachmentId { get; set; }

        [Write(false)]
        public string Name { get; set; }

        [Write(false)]
        public string VATNumber { get; set; }

        [Write(false)]
        public string CompanyVAT => Application.AppData.CompanyData.VATNumber.ToString();
    }
}
