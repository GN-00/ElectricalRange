using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[JobOrders(Canceled)]")]
    public class CancelJobOrder
    {
        public int Id { get; set; }
        public int JobOrderId { get; set; }
        public DateTime Date { get; set; }
    }
}
