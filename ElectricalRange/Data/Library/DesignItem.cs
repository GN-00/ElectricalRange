using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Library
{
    [Table("[Estimator].[DesignItems]")]
    public class DesignItem
    {
        [Key]
        public int Id { get; set; }
        public int SelectionId { get; set; }
        public int PanelId { get; set; }
        public int SN { get; set; }

        [Write(false)]
        public string Group => $"{LabelName}{LabelNumber}";
        public string LabelName { get; set; }
        public int LabelNumber { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public double Qty { get; set; }
        public string ItemTable { get; set; }
        public string Note { get; set; }


        [Write(false)]
        public string ItemType { get; set; }

        [Write(false)]
        public int Sort { get; set; }



        [Write(false)]
        public static string Table => "[Estimator].[DesignItems]";
        public override string ToString()
        {
            return Description;
        }
    }
}
