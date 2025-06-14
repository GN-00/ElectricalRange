using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201192 : SqlUpdate
    {
        public U202201192()
        {
            Key = nameof(U202201192);
            Command = "ALTER VIEW [Purchase].[OrdersView] AS " +
                      "SELECT Purchase.Orders.ID, Purchase.Orders.JobOrderID, Purchase.Orders.SupplierID, Purchase.Orders.Number, Purchase.Orders.Code, Purchase.Orders.Date,  " +
                      "Store.Suppliers.Name AS SupplierName, Store.Suppliers.Code AS SupplierCode, Store.SuppliersContacts.Name AS SupplierAttention,  " +
                      "Purchase.Orders.QuotationCode, Purchase.Orders.SupplierAttentionID, Purchase.Orders.DeliverToPlace, Purchase.Orders.DeliverToPerson,  " +
                      "Purchase.Orders.DeliveryAddress, Purchase.Orders.Payment, Purchase.Orders.VAT, Purchase.Orders.QuotationRequestId, Purchase.Orders.Revise,  " +
                      "Purchase.Orders.ReviseDate, Purchase.Orders.OriginalOrderID, Purchase.Orders.Revised " +
                      "FROM Store.SuppliersContacts RIGHT OUTER JOIN " +
                      "Purchase.Orders ON Store.SuppliersContacts.ID = Purchase.Orders.SupplierAttentionID LEFT OUTER JOIN " +
                      "Store.Suppliers ON Purchase.Orders.SupplierID = Store.Suppliers.ID " +
                      "WHERE (Purchase.Orders.Revised = 0); ";

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


