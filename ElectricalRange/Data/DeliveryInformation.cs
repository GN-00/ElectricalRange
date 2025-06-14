using System;

namespace ProjectsNow.Data
{
    public class DeliveryInfromation
    {
        public DateTime Date { get; set; }
        public string JobOrderCode { get; set; }
        public string DeliveryNumber { get; set; }
        public int ShipmentNo { get; set; }
        public string CustomerName { get; set; }
        public string Attention { get; set; }
        public string ProjectName { get; set; }
        public string PurchaseOrderNumber { get; set; }

    }
}
