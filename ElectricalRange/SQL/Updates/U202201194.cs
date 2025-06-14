using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201194 : SqlUpdate
    {
        public U202201194()
        {
            Key = nameof(U202201194);
            Command = "ALTER VIEW [Purchase].[TransactionsView] AS " +
                      "SELECT Purchase.Transactions.ID, Purchase.Orders.JobOrderID, " +
                      "Purchase.Transactions.PurchaseOrderID, Purchase.Transactions.Category, Purchase.Transactions.Code,  " +
                      "Purchase.Transactions.Description, Purchase.Transactions.Unit, Purchase.Transactions.Qty, " +
                      "Purchase.Transactions.Cost, Purchase.Transactions.Reference,  " +
                      "Purchase.Orders.Revised " +
                      "FROM Purchase.Transactions LEFT OUTER JOIN " +
                      "Purchase.Orders ON Purchase.Transactions.PurchaseOrderID = Purchase.Orders.ID " +
                      "WHERE(Purchase.Orders.Revised = 0)";

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


