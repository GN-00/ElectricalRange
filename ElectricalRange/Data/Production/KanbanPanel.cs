
using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Production
{
    [Table("[Production].[KanbanPanels]")]
    public class KanbanPanel
    {
        [Key]
        public int Id { get; set; }
        public int PanelId { get; set; }
        public int JobOrderId { get; set; }
        public string Status { get; set; } // ToDo, InProgress, QualityCheck, ReadyForDelivery, Completed
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public int UpdatedBy { get; set; }
        public string Notes { get; set; }
        public int Priority { get; set; } = 1; // 1-Low, 2-Medium, 3-High, 4-Critical
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public decimal? ProgressPercentage { get; set; }
        
        // Navigation properties
        [Write(false)]
        public ProductionPanel Panel { get; set; }
    }
}
