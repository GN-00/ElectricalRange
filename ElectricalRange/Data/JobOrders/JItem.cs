using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[PanelsItems]")]
    public class JItem : Base  //Job Order Item
    {
        private string _Article1;
        private string _Article2;
        private string _Category;
        private string _Code;
        private string _Description;
        private string _Brand;
        private string _Remarks;
        private string _ItemType;

        [Key]
        public int ItemID { get; set; }
        public int PanelID { get; set; }
        public string Article1
        {
            get => _Article1;
            set => SetValue(ref _Article1, value);
        }
        public string Article2
        {
            get => _Article2;
            set => SetValue(ref _Article2, value);
        }
        public string Category
        {
            get => _Category;
            set => SetValue(ref _Category, value);
        }
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }
        public string Unit { get; set; }
        public decimal ItemQty { get; set; }
        public string Brand
        {
            get => _Brand;
            set => SetValue(ref _Brand, value);
        }
        public string Remarks
        {
            get => _Remarks;
            set => SetValue(ref _Remarks, value);
        }
        public decimal ItemCost { get; set; }
        public decimal ItemDiscount { get; set; }
        [Write(false)]
        public decimal ItemPrice => ItemCost * (1 - (ItemDiscount / 100m));
        [Write(false)]
        public decimal ItemTotalCost => ItemCost * ItemQty;
        [Write(false)]
        public decimal ItemTotalPrice => ItemCost * ItemQty * (1 - (ItemDiscount / 100m));
        public string ItemTable { get; set; }
        public string ItemType
        {
            get => _ItemType;
            set => SetValue(ref _ItemType, value);
        }
        public int ItemSort { get; set; }
        public string SelectionGroup { get; set; }
    }
}
