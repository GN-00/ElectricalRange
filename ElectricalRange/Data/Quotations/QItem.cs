using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Quotations
{
    [Table("[Quotation].[QuotationsPanelsItems]")]
    public class QItem : Base
    {
        public override string ToString()
        {
            return Code?.ToString();
        }
        
        private string _Article1;
        private string _Article2;
        private string _Category;
        private string _Code;
        private string _Description;
        private decimal _ItemQty = 1;
        private string _Brand;
        private string _Remarks;
        private decimal _ItemCost;
        private decimal _ItemDiscount;
        private string _ItemTable;
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
        public string Unit { get; set; } = "No";
        public decimal ItemQty
        {
            get => _ItemQty;
            set
            {
                if (SetValue(ref _ItemQty, value))
                {
                    OnPropertyChanged(nameof(ItemTotalCost));
                    OnPropertyChanged(nameof(ItemTotalPrice));
                }
            }
        }
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
        public decimal ItemCost
        {
            get => _ItemCost;
            set
            {
                if (value != _ItemCost)
                {
                    if (value < 0)
                    {
                        SetValue(ref _ItemCost, 0);
                    }
                    else
                    {
                        SetValue(ref _ItemCost, value);
                    }

                    OnPropertyChanged(nameof(ItemPrice));
                    OnPropertyChanged(nameof(ItemTotalCost));
                    OnPropertyChanged(nameof(ItemTotalPrice));
                }
            }
        }
        public decimal ItemDiscount
        {
            get => _ItemDiscount;
            set
            {
                if (value != _ItemDiscount)
                {
                    if (value > 100)
                    {
                        SetValue(ref _ItemDiscount, 0);
                    }
                    else
                    {
                        SetValue(ref _ItemDiscount, value);
                    }

                    OnPropertyChanged(nameof(ItemTotalPrice));
                }
            }
        }

        [Write(false)]
        public decimal ItemPrice => ItemCost * (1 - (ItemDiscount / 100m));
        [Write(false)]
        public decimal ItemTotalCost => ItemCost * ItemQty;
        [Write(false)]
        public decimal ItemTotalPrice => ItemCost * ItemQty * (1 - (ItemDiscount / 100m));
        public string ItemTable
        {
            get => _ItemTable;
            set => SetValue(ref _ItemTable, value);
        }
        public string ItemType
        {
            get => _ItemType;
            set => SetValue(ref _ItemType, value);
        }
        public int ItemSort { get; set; }
        public string SelectionGroup { get; set; }
        [Write(false)]
        public decimal ReferenceCost { get; set; }
        [Write(false)]
        public decimal ReferenceDiscount { get; set; }
    }
}
