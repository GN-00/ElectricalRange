using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Production
{
    [Table("[Production].[PanelsItems(Used)]")]
    public class UsedItem
    {
        [Key]
        public int Id { get; set; }
        public int JobOrderId { get; set; }
        public int PanelId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public double Qty { get; set; }
        public DateTime Date { get; set; }
    }
}
