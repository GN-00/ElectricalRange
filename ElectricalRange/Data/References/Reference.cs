using Dapper.Contrib.Extensions;

using ProjectsNow.Enums;

namespace ProjectsNow.Data.References
{
    [Table("[Reference].[References]")]
    public class Reference : Base, IAccess
    {
        private string _SearchKeys = "";
        private string _Category;
        private string _Code;
        private string _SupplierCode;
        private string _Description;
        private string _Brand;
        private string _Unit = "No";
        private decimal _Cost;
        private decimal _Discount;
        private decimal _Stock;

        [Key]
        public int ReferenceID { get; set; }

        [Write(false)]
        public int Id => ReferenceID;

        public string SearchKeys
        {
            get => _SearchKeys;
            set => SetValue(ref _SearchKeys, value);
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

        public string SupplierCode
        {
            get => _SupplierCode;
            set => SetValue(ref _SupplierCode, value);
        }
        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }
        public string Article1 { get; set; }
        public string Article2 { get; set; }
        public string Brand
        {
            get => _Brand;
            set => SetValue(ref _Brand, value);
        }
        public string Remarks { get; set; }
        public string Unit
        {
            get => _Unit;
            set => SetValue(ref _Unit, value);
        }
        public decimal Cost
        {
            get => _Cost;
            set => SetValue(ref _Cost, value);
        }
        public decimal Discount
        {
            get => _Discount;
            set => SetValue(ref _Discount, value);
        }

        [Write(false)]
        public decimal Stock
        {
            get => _Stock;
            set => SetValue(ref _Stock, value);
        }

        [Write(false)]
        public string ItemType { get; set; } = ItemTypes.Standard.ToString();
        public string Type { get; set; }
        public bool EditableCost { get; set; }

        [Write(false)]
        public decimal ListPrice => Cost * (1 - Discount / 100);

        [Write(false)]
        public decimal NewCost
        {
            get => _Cost;
            set => SetValue(ref _Cost, value);
        }
    }
}