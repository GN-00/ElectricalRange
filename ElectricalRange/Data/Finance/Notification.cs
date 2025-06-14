using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    [Table("[Accountant].[Notifications]")]
    public class Notification : Base
    {
        private string _Code;
        private DateTime _Date = DateTime.Now.Date;
        private double? _Amount;

        [Key]
        public int Id { get; set; }
        public int JobOrderId { get; set; }
        public int Number { get; set; }
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }

        public double? Amount
        {
            get => _Amount;
            set => SetValue(ref _Amount, value);
        }

        [Write(false)]
        public string QuotationCode { get; set; }

        [Write(false)]
        public string JobOrderCode { get; set; }

        [Write(false)]
        public string Customer { get; set; }

        [Write(false)]
        public string Attention { get; set; }

        [Write(false)]
        public DateTime PurchaseOrderDate { get; set; }

        [Write(false)]
        public string PurchaseOrderNumber { get; set; }


        [Write(false)]
        public double Paid { get; set; } 

        [Write(false)]
        public double FinalPrice { get; set; } 

        [Write(false)]
        public double Balance => FinalPrice - Paid;

        [Write(false)]
        public double Payment => Amount.GetValueOrDefault();

        [Write(false)]
        public string PaymentText => DataInput.Input.NumberToSRWords((decimal)Payment);

    }
}
