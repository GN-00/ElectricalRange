using Dapper.Contrib.Extensions;

using ProjectsNow.Data.Application;

namespace ProjectsNow.Data
{
    [Table("[Purchase].[Transactions]")]
    public class CompanyPOTransaction : Base
    {
        private string _Category;
        private string _Code;
        private string _Description;
        private string _Unit;
        private decimal _Qty;
        private decimal _ReceivedQty;
        private decimal _Cost;
        private decimal _VAT = AppData.VAT;

        [Key]
        public int ID { get; set; }

        [Write(false)]
        public int JobOrderID { get; set; }

        [Write(false)]
        public int SN { get; set; }
        public int PurchaseOrderID { get; set; }
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
        public string Unit
        {
            get => _Unit;
            set => SetValue(ref _Unit, value);
        }
        public decimal Qty
        {
            get => _Qty;
            set
            {
                if (value <= 0)
                {
                    value = 1;
                }

                if (SetValue(ref _Qty, value))
                {
                    OnPropertyChanged(nameof(TotalCost));
                }
            }
        }

        public decimal VAT
        {
            get => _VAT;
            set => SetValue(ref _VAT, value);
        }

        [Write(false)]
        public decimal ReceivedQty
        {
            get => _ReceivedQty;
            set => SetValue(ref _ReceivedQty, value)
                  .UpdateProperties(this, nameof(FinalQty), nameof(Received));
        }

        [Write(false)]
        public decimal FinalQty => Qty - ReceivedQty;

        public decimal Cost
        {
            get => _Cost;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (SetValue(ref _Cost, value))
                {
                    OnPropertyChanged(nameof(TotalCost));
                }
            }
        }
        [Write(false)]
        public decimal TotalCost => _Qty * _Cost;

        [Write(false)]
        public bool Received => Qty == ReceivedQty;

        [Write(false)]
        public string Type { get; set; } = "Purchase Order";
    }
}
