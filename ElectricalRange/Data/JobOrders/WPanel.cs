using Dapper.Contrib.Extensions;

using System.IO;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[WarrantiesPanels]")]
    public class WPanel : Base
    {
        [Key]
        public int Id { get; set; }
        public int WarrantyId { get; set; }
        public int PanelId { get; set; }

        [Write(false)]
        public int JobOrderId { get; set; }

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
        public string Enclosure { get; set; }
    }
}
