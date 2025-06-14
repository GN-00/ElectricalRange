using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Quotations
{
    [Table("[Quotation].[QuotationsOptions]")]
    public class QuotationOption : Base
    {
        [Key]
        public int ID { get; set; }
        public int QuotationID { get; set; }

        private int _Number;
        public int Number
        {
            get => _Number;
            set
            {
                if (SetValue(ref _Number, value))
                {
                    OnPropertyChanged(nameof(Code));
                }
            }
        }

        [Write(false)]
        public string Code => ((char)(65 + Number)).ToString();

        private string _Name;
        public string Name
        {
            get => _Name;
            set => SetValue(ref _Name, value);
        }


        private decimal _QuotationCost;
        [Write(false)]
        public decimal QuotationCost
        {
            get => _QuotationCost;
            set => SetValue(ref _QuotationCost, value);
        }

        private decimal _QuotationPrice;
        [Write(false)]
        public decimal QuotationPrice
        {
            get => _QuotationPrice;
            set => SetValue(ref _QuotationPrice, value);
        }


        private decimal _QuotationEstimatedPrice;
        [Write(false)]
        public decimal QuotationEstimatedPrice
        {
            get => _QuotationEstimatedPrice;
            set => SetValue(ref _QuotationEstimatedPrice, value);
        }


        private decimal _QuotationFinalPrice;
        [Write(false)]
        public decimal QuotationFinalPrice
        {
            get => _QuotationFinalPrice;
            set => SetValue(ref _QuotationFinalPrice, value);
        }


        private decimal _QuotationDiscountValue;
        [Write(false)]
        public decimal QuotationDiscountValue
        {
            get => _QuotationDiscountValue;
            set => SetValue(ref _QuotationDiscountValue, value);
        }


        private decimal _QuotationVATValue;
        [Write(false)]
        public decimal QuotationVATValue
        {
            get => _QuotationVATValue;
            set => SetValue(ref _QuotationVATValue, value);
        }

    }
}
