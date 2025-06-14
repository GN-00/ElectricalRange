namespace ProjectsNow.Data.Finance
{
    public class JobOrder : Base, IAccess
    {
        private decimal _Paid;

        public int ID { get; set; }
        public int CustomerID { get; set; }
        public string Code { get; set; }
        public int CodeNumber { get; set; }
        public int Year { get; set; }
        public string QuotationCode { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNameArabic { get; set; }
        public string Contact { get; set; }
        public decimal QuotationPrice { get; set; }
        public decimal QuotationEstimatedPrice { get; set; }
        public decimal QuotationFinalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal QuotationDiscountValue { get; set; }
        public decimal QuotationVATValue { get; set; }
        public decimal VAT { get; set; }
        public decimal VATPercentage => VAT * 100;

        public decimal Amount => QuotationFinalPrice;
        public decimal Paid
        {
            get => _Paid;
            set => SetValue(ref _Paid, value)
                  .UpdateProperties(this, nameof(Balance), nameof(BalanceView), nameof(BalanceStatus));
        }
        public decimal Balance => QuotationFinalPrice - Paid;
        public decimal BalanceView
        {
            get
            {
                if (-1 <= Balance && Balance <= 1)
                {
                    return 0;
                }
                else
                {
                    return Balance;
                }
            }
        }

        public decimal BalanceStatus
        {
            get
            {
                if (BalanceView > 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}
