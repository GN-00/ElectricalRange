using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Production
{
    [Table("[JobOrder].[Panels(InProduction)]")]
    public class AddPanel
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int PanelId { get; set; }
        public bool InProduction { get; set; }
        public int Reference { get; set; }
        public DateTime Date { get; set; }
    }
}
