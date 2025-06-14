using System;

namespace ProjectsNow.Data.Quotations
{
    public class Revision
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Code { get; set; }
        public DateTime? Date { get; set; }
        public double QuotationEstimatedPrice { get; set; }
    }
}
