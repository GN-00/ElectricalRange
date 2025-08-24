using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Library
{
    [Table("[Estimator].[Selections]")]
    public class Selection : PropertiesValues
    {
        [Key]
        public int Id { get; set; }
        public int PanelId { get; set; }
        public string GroupId { get; set; }
        public int Qty { get; set; }

        [Write(false)]
        public static string Table => "[Estimator].[Selections]";
    }
}
