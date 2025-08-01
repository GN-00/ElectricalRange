using System;

namespace ProjectsNow.Data.PurchaseOrder
{
    public class Revision
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Code { get; set; }
        public DateTime? Date { get; set; }
    }
}
