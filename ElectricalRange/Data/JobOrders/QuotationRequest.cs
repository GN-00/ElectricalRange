using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[QuotationsRequests]")]
    public class QuotationRequest : Base
    {
        [Key]
        public int Id { get; set; }
        public int JobOrderID { get; set; }

        private int _Number;
        public int Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }

        private DateTime _Date;
        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }
    }
}
