using Dapper.Contrib.Extensions;

using System.IO;

namespace ProjectsNow.Data.Finance
{
    [Table("[Accountant].[NotificationsPanels]")]
    public class ReceiptPanel : Base
    {
        [Key]
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public int PanelId { get; set; }

        [Write(false)]
        public int SN { get; set; }

        [Write(false)]
        public string Name { get; set; }

        [Write(false)]
        public string NameInfo
        {
            get
            {
                using var reader = new StringReader(Name);
                return reader.ReadLine();
            }
        }

        [Write(false)]
        public int Qty { get; set; }

        [Write(false)]
        public int FinalPrice { get; set; }

        [Write(false)]
        public int TotalFinalPrice { get; set; }
    }
}
