using Dapper.Contrib.Extensions;


namespace ProjectsNow.Data.Production
{
    [Table("[Production].[MaterialsRequests]")]
    public class MaterialsRequest : Base
    {
        [Key]
        public int Id { get; set; }
        public int JobOrderId { get; set; }
        public int PanelId { get; set; }
        public int RequestId { get; set; }
        public DateTime Date { get; set; }

        [Write(false)]
        public string JobOrderCode { get; set; }

        [Write(false)]
        public string Customer { get; set; }

        [Write(false)]
        public string PanelName { get; set; }

        [Write(false)]
        public string RequestCode => $"Part-{RequestId}";
        [Write(false)]
        public int Page { get; set; }
        [Write(false)]
        public int Pages { get; set; }
        //[Write(false)]
        //public ProductionPanel PanelData { get; set; }
        [Write(false)]
        public List<Item> Items { get; set; }
    }
}
