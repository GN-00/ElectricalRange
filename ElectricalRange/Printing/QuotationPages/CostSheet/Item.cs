namespace ProjectsNow.Printing.QuotationPages.CostSheet
{
    public class Item
    {
        public int QuotationID { get; set; }
        public int Sort { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal PublicPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal Total { get; set; }
        public string Brand { get; set; }
        public string ItemTable { get; set; }
        public string Article { get; set; }
        public string Type => Article == "COPPER" ? "Copper" : ItemTable == "Enclosure" ? "Enclosure" : Brand == "Schneider" ? "Schneider" : "Other";
    }
}
