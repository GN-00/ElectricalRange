using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201193 : SqlUpdate
    {
        public U202201193()
        {
            Key = nameof(U202201193);
            Command = "ALTER VIEW [Store].[Items(InOrder)] AS " +
                      "SELECT Purchase.Orders.JobOrderID, Purchase.Transactions.Category, " +
                      "Purchase.Transactions.Code, SUM(Purchase.Transactions.Qty) AS InOrderQty " +
                      "FROM Purchase.Orders LEFT OUTER JOIN " +
                      "Purchase.Transactions ON Purchase.Orders.ID = Purchase.Transactions.PurchaseOrderID " +
                      "WHERE (Purchase.Transactions.Reference IS NULL) AND(Purchase.Orders.Revised = 0) " +
                      "GROUP BY Purchase.Orders.JobOrderID, Purchase.Transactions.Category, Purchase.Transactions.Code";

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


