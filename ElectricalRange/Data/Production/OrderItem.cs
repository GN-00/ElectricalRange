namespace ProjectsNow.Data.Production
{
    public class OrderItem
    {
        public int JobOrderId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public double Qty { get; set; }
        public double Stock { get; set; }
        public double Missing => Qty - Stock;
        public double Percent => Stock == 0 ? 0 : (Stock / Qty) * 100;
    }
}
