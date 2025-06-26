namespace ProjectsNow.Data.Production
{
    public class CloseRequestInfo
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public string JobOrderCode { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public List<ProductionPanel> Panels { get; set; }

    }
}
