using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Store
{
    [Table("[Store].[TransferInvoices]")]
    public class TransferInvoice : Base
    {
        private int _Number;
        private int _Month;
        private int _Year;
        private DateTime _Date;
        private string _Code;

        [Key]
        public int ID { get; set; }
        public int JobOrderID { get; set; }
        public int Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }
        public int Month
        {
            get => _Month;
            set => SetValue(ref _Month, value);
        }
        public int Year
        {
            get => _Year;
            set => SetValue(ref _Year, value);
        }
        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
    }
}
