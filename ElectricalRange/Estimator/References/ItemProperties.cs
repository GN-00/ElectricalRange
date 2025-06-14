using Dapper.Contrib.Extensions;

namespace ProjectsNow.Estimator.References
{
    [Table("[Estimator].[ItemsProperties]")]
    public class ItemProperties : PropertiesSorts
    {
        [Key]
        public int Id { get; set; }
        public string GroupId { get; set; }
        public int Sort { get; set; }
        public string ItemType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }

        public override string ToString()
        {
            return $"{Description}";
        }
    }
}
