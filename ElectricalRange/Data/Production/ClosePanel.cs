using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Production
{

    [Table("[Production].[Panels(Closing)]")]
    public class ClosePanel
    {
        [Key]
        public int Id { get; set; }
        public int JobOrderId { get; set; }
        public int PanelId { get; set; }
        public int Number { get; set; }
        public int Qty { get; set; }
        public DateTime Date { get; set; }
    }
}
