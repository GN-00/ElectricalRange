using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Production
{
    [Table("[Production].[PanelsItems]")]
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public int JobOrderId { get; set; }
        public int PanelId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public double Qty { get; set; }
        public string Type { get; set; }

        [Write(false)]
        public double ReceivedQty { get; set; }

        [Write(false)]
        public double Percentage  => (ReceivedQty / Qty) * 100;

    }
}
