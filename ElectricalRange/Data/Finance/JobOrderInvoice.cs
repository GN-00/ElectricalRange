using System;

namespace ProjectsNow.Data.Finance
{
    public class JobOrderInvoice : Base
    {
        private decimal _VAT;
        private decimal _NetPrice;
        private decimal _VATValue;
        private decimal _GrossPrice;
        private decimal _VATPercentage;

        public int JobOrderID { get; set; }
        public int CustomerID { get; set; }
        public string JobOrderCode { get; set; }
        public string CustomerName { get; set; }
        public string Number { get; set; }
        public int CodeNumber { get; set; }
        public DateTime Date { get; set; }
        public string DateInfo => Date.ToString("dd-MM-yyyy");
        public int Panels { get; set; }
        public decimal VAT
        {
            get => _VAT;
            set => SetValue(ref _VAT, value);
        }
        public decimal VATPercentage
        {
            get => _VATPercentage;
            set => SetValue(ref _VATPercentage, value);
        }
        public decimal NetPrice
        {
            get => _NetPrice;
            set => SetValue(ref _NetPrice, value);
        }
        public decimal VATValue
        {
            get => _VATValue;
            set => SetValue(ref _VATValue, value);
        }
        public decimal GrossPrice
        {
            get => _GrossPrice;
            set => SetValue(ref _GrossPrice, value);
        }
        public decimal Paid { get; set; }
        public decimal Balance
        {
            get
            {
                var value = GrossPrice - Paid;
                if (value > 1)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
        }
        public decimal BalanceInfo
        {
            get
            {
                if (Balance > 1)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
