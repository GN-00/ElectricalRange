namespace ProjectsNow.Data
{
    public class BillItem
    {
        public int ItemID { get; set; }
        public int PanelID { get; set; }
        public string Article1 { get; set; }
        public string Article2 { get; set; }
        public string Category { get; set; }
        public string Code { get; set; }
        public string PartNumber => $"{Category}{Code}";
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal ItemQty { get; set; }
        public string Brand { get; set; }
        public string Remarks { get; set; }
        public decimal ItemCost { get; set; }
        public decimal ItemDiscount { get; set; }
        public decimal ItemPrice => ItemCost * (1 - (ItemDiscount / 100m));
        public decimal ItemTotalCost => ItemCost * ItemQty;
        public decimal ItemTotalPrice => ItemCost * ItemQty * (1 - (ItemDiscount / 100m));
        public string ItemTable { get; set; }
        public string ItemType { get; set; }
        public int ItemSort { get; set; }
    }
}
