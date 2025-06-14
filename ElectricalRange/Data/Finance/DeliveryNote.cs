using System;

namespace ProjectsNow.Data.Finance
{
    public class DeliveryNote : Base
    {
        public int JobOrderId { get; set; }
        public string Code { get; set; }
        public string JobOrderCode { get; set; }
        public DateTime Date { get; set; }
        public string DateInfo => Date.ToString("dd-MM-yyyy");
        public int Panels { get; set; }
        public string Customer { get; set; }
        public string Project { get; set; }
        public int Number { get; set; }
        public int Year { get; set; }

    }
}
