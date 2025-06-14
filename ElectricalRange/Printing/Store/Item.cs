using System;

namespace ProjectsNow.Printing.Store
{
    public class Item
    {
        public int ID { get; set; }
        public int InvoiceID { get; set; }
        public int SN { get; set; }
        public string Supplier { get; set; }
        public string InvoiceNumber { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Qty { get; set; }
        public decimal Cost { get; set; }
        public decimal TotalCost =>  Qty * Cost;
        public decimal VAT { get; set; }
        public decimal VATValue => VAT / 100m * TotalCost;
        public decimal TotalPrice => Math.Truncate(Qty * Cost * (1 + VAT / 100m) * 100m) / 100m;
    }
}
