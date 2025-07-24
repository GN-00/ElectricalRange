namespace ProjectsNow.Data.Production
{
    public class CheckItem
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public double Qty { get; set; }
        public double Design { get; set; }
        public double Missing => Qty - Design;
    }
}
