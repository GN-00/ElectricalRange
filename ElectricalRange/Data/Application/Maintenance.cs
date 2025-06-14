using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Store;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Data.Application
{
    internal static class Maintenance
    {
        public static void QoutationItemsSort()
        {
            string query;
            List<QPanel> panels;
            List<QItem> itemsDetails;
            List<QItem> itemsEnclosure;
            List<QItem> itemsAccessories;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Quotation].[QuotationsPanels] " +
                        $"Order By PanelID";

                panels = connection.Query<QPanel>(query).ToList();
            }

            foreach (QPanel panel in panels)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    query = $"Select * From [Quotation].[QuotationsPanelsItems] " +
                            $"Where ItemTable = 'Details' And PanelID = {panel.PanelID} " +
                            $"Order By ItemSort";
                    itemsDetails = connection.Query<QItem>(query).ToList();

                    query = $"Select * From [Quotation].[QuotationsPanelsItems] " +
                            $"Where ItemTable = 'Enclosure' And PanelID = {panel.PanelID} " +
                            $"Order By ItemSort";
                    itemsEnclosure = connection.Query<QItem>(query).ToList();

                    query = $"Select * From [Quotation].[QuotationsPanelsItems] " +
                            $"Where ItemTable = 'Accessories' And PanelID = {panel.PanelID} " +
                            $"Order By ItemSort";
                    itemsAccessories = connection.Query<QItem>(query).ToList();
                }

                foreach (QItem item in itemsDetails)
                {
                    item.ItemSort = itemsDetails.IndexOf(item);
                }

                foreach (QItem item in itemsEnclosure)
                {
                    item.ItemSort = itemsEnclosure.IndexOf(item);
                }

                foreach (QItem item in itemsAccessories)
                {
                    item.ItemSort = itemsAccessories.IndexOf(item);
                }

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Update(itemsDetails);
                    _ = connection.Update(itemsEnclosure);
                    _ = connection.Update(itemsAccessories);
                }
            }
        }
        public static string QuotationItemsCheckSort()
        {
            string result = "";
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Quotation].[QuotationsPanelsItems] " +
                               $"Where ItemTable = 'Details' " +
                               $"Order By PanelID, ItemSort";

                var items = connection.Query<QItem>(query);

                int oldSN = 0;
                int oldID = 0;
                foreach (QItem item in items)
                {
                    if (item.PanelID != oldID)
                    {
                        oldID = item.PanelID;
                        oldSN = item.ItemSort;

                        if (item.ItemSort != 0 && item.ItemSort != 1)
                            result += $"{item.PanelID}\n";
                    }
                    else
                    {
                        if (item.ItemSort - 1 != oldSN)
                        {
                            result += $"{item.PanelID}\n";
                        }
                    }

                    oldSN = item.ItemSort;
                }
            }

            return result;
        }
        public static string InvoicesChecking()
        {
            string result = "";
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Finance].[JobOrdersInvoices] " +
                               $"Order By Number";

                var items = connection.Query<JobOrderInvoice>(query);

                string oldNumber = "";
                foreach (JobOrderInvoice item in items)
                {
                    if (item.Number == oldNumber)
                    {
                        result += item.JobOrderID.ToString() + ", ";
                    }

                    oldNumber = item.Number;
                }
            }
            return result;
        }

        public class Error
        {
            public string Number { get; set; }
            public double Net { get; set; }
            public double VAT { get; set; }
            public double Gross { get; set; }
        }
        public static void ConvertOldInvoiceData()
        {
            string query;
            Error error = new();
            List<IPanel> panels;
            ObservableCollection<InvoiceItem> items;
            ObservableCollection<JobOrderInvoice> invoices;
            ObservableCollection<Error> errors = new();
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Finance].[JobOrdersInvoices] ";
                invoices = new ObservableCollection<JobOrderInvoice>(connection.Query<JobOrderInvoice>(query));
            }

            foreach (JobOrderInvoice invoice in invoices)
            {
                Finance.Invoice newInvoice = new()
                {
                    CustomerId = invoice.CustomerID,
                    JobOrderId = invoice.JobOrderID,
                    Number = int.Parse(invoice.Number.Substring(6, 3)),
                    Month = int.Parse(invoice.Number.Substring(4, 2)),
                    Year = int.Parse(invoice.Number.Substring(0, 4)),
                    Code = invoice.Number,
                    Date = invoice.Date,
                    NetPrice = (double)invoice.NetPrice,
                    VAT = (double)invoice.VAT,
                    VATPercentage = (double)invoice.VATPercentage,
                    VATValue = (double)invoice.VATValue,
                    GrossPrice = (double)invoice.GrossPrice,
                    Type = "Panels",
                };

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    Invoice check;
                    query = $"Select * from [Finance].[CustomersInvoices] Where Code = '{invoice.Number}'";
                    check = connection.QueryFirstOrDefault<Invoice>(query);

                    if (check != null)
                        continue;

                    _ = connection.Insert(newInvoice);

                    query = $"Select * From [JobOrder].[Panels(InvoiceDetails)] " +
                            $"Where InvoiceNumber = {invoice.Number}";

                    panels = connection.Query<IPanel>(query).ToList();
                }

                items = new ObservableCollection<InvoiceItem>();
                for (int i = 1; i <= panels.Count; i++)
                {
                    var panel = panels[i - 1];

                    panel.PanelSN = i;
                    if (panel.SpecialPrice != 0)
                    {
                        panel.OriginalPrice = panel.PanelEstimatedPrice;

                        panel.PanelsFinalPrice = panel.SpecialPrice;
                        panel.PanelFinalPrice = panel.PanelsFinalPrice / panel.InvoicedQty;

                        panel.PanelsVATValue = panel.SpecialPrice / (1 + panel.VAT) * panel.VAT;
                        panel.PanelVATValue = panel.PanelsVATValue / panel.InvoicedQty;

                        panel.PanelsEstimatedPrice = panel.SpecialPrice / (1 + panel.VAT);
                        panel.PanelEstimatedPrice = panel.PanelsEstimatedPrice / panel.InvoicedQty;
                    }

                    if (panel.SpecialName != null)
                    {
                        panel.PanelName = panel.SpecialName;
                    }

                    if (panel.SpecialArabicType != null)
                    {
                        panel.PanelTypeArabic = panel.SpecialArabicType;
                    }


                    InvoiceItem item = new()
                    {
                        InvoiceId = newInvoice.Id,
                        PanelId = panel.PanelID,
                        SN = panel.PanelSN,
                        Description = panel.PanelName,
                        ArabicDescription = panel.PanelTypeArabicInfo,
                        Qty = panel.InvoicedQty,
                        NetPrice = (double)panel.PanelsEstimatedPrice,
                        UnitNetPrice = (double)panel.PanelEstimatedPrice,
                        VAT = (double)invoice.VAT,
                        VATValue = (double)panel.PanelsVATValue,
                        UnitVATValue = (double)panel.PanelVATValue,
                        GrossPrice = (double)panel.PanelsFinalPrice,
                        UnitGrossPrice = (double)panel.PanelFinalPrice,
                        Code = invoice.JobOrderCode + "-" + panel.PanelSN,
                        UnitOriginalPrice = (double)panel.OriginalPrice,
                    };

                    items.Add(item);
                }


                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(items);
                }

                error = new Error()
                {
                    Number = newInvoice.Code,
                    Net = newInvoice.NetPrice - (double)panels.Sum(x => x.PanelsEstimatedPrice),
                    VAT = newInvoice.VATValue - (double)panels.Sum(x => x.PanelsVATValue),
                    Gross = newInvoice.GrossPrice - (double)panels.Sum(x => x.PanelsFinalPrice),
                };

                if (error.Net >= 0.01 || error.VAT >= 0.01 || error.Gross >= 0.01)
                {
                    errors.Add(error);
                }
            }
        }
        public static void ConvertOldMaterialInvoiceData()
        {
            string query;
            ObservableCollection<InvoiceItem> newItems;
            ObservableCollection<SalesItem> items;
            ObservableCollection<SalesInvoice> invoices;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Store].[SalesInvoices(View)] ";
                invoices = new ObservableCollection<SalesInvoice>(connection.Query<SalesInvoice>(query));
            }

            foreach (SalesInvoice invoice in invoices)
            {
                //M-2022060001
                Finance.Invoice newInvoice = new()
                {
                    CustomerId = invoice.CustomerId,
                    JobOrderId = null,
                    PurchaseOrderNumber = invoice.PurchaseOrderNumber,
                    Address = invoice.Address,
                    ContactId = invoice.ContactId,
                    Number = int.Parse(invoice.Code.Substring(8, 4)),
                    Month = int.Parse(invoice.Code.Substring(6, 2)),
                    Year = int.Parse(invoice.Code.Substring(2, 4)),
                    Code = invoice.Code,
                    Date = invoice.Date,
                    NetPrice = (double)invoice.NetPrice,
                    VAT = (double)invoice.VAT / 100,
                    VATPercentage = (double)invoice.VAT,
                    VATValue = (double)invoice.VATValue,
                    GrossPrice = (double)invoice.GrossPrice,
                    Type = "Items",
                };

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    Invoice check;
                    query = $"Select * from [Finance].[CustomersInvoices] Where Code = '{invoice.Number}'";
                    check = connection.QueryFirstOrDefault<Invoice>(query);

                    if (check != null)
                        continue;

                    _ = connection.Insert(newInvoice);

                    query = $"Select * From [Store].[SalesItems(View)] " +
                            $"Where SalesInvoiceId = {invoice.Id} " +
                            $"Order By Code";
                    items = new ObservableCollection<SalesItem>(connection.Query<SalesItem>(query));
                }

                newItems = new ObservableCollection<InvoiceItem>();
                for (int i = 1; i <= items.Count; i++)
                {
                    var oldItem = items[i - 1];

                    InvoiceItem item = new()
                    {
                        InvoiceId = newInvoice.Id,
                        PanelId = null,
                        ItemId = null,
                        SN = i,
                        Description = oldItem.Description,
                        ArabicDescription = oldItem.ArabicInfo,
                        Qty = (double)oldItem.Qty,
                        NetPrice = (double)oldItem.NetPrice,
                        UnitNetPrice = (double)oldItem.UnitNetPrice,
                        VAT = (double)oldItem.VAT,
                        VATValue = (double)oldItem.VATValue,
                        UnitVATValue = (double)oldItem.VATValue / (double)oldItem.Qty,
                        GrossPrice = (double)oldItem.GrossPrice,
                        UnitGrossPrice = (double)oldItem.GrossPrice / (double)oldItem.Qty,
                        Code = oldItem.Code,
                        Brand = oldItem.Brand,
                        Unit = oldItem.Unit,
                    };

                    newItems.Add(item);
                }


                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(newItems);
                }
            }
        }
        public static void ConvertMoneyTransactionsData()
        {
            string query;
            ObservableCollection<MoneyTransaction> moneyTransactions;
            ObservableCollection<AccountTransaction> transactions = new();
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [Finance].[MoneyTransactions] ";
                moneyTransactions = new ObservableCollection<MoneyTransaction>(connection.Query<MoneyTransaction>(query));
            }

            foreach (MoneyTransaction moneyTransaction in moneyTransactions)
            {
                AccountTransaction newData = new();
                newData.OldId = moneyTransaction.ID;
                newData.Date = moneyTransaction.Date;
                newData.Description = moneyTransaction.Description;
                newData.Amount = (double)moneyTransaction.Amount;
                newData.AccountId = moneyTransaction.AccountID;
                newData.Post = true;
                newData.AttachmentId = moneyTransaction.AttachmentID;

                if (moneyTransaction.Type == "Project")
                {
                    newData.Type = Enums.AccountingTransactions.Receipt.ToString();
                    newData.CustomerId = moneyTransaction.CustomerID;
                }
                else
                {
                    newData.Type = Enums.AccountingTransactions.Payment.ToString();
                    newData.SupplierId = moneyTransaction.SupplierID;
                }

                AccountTransaction check;
                using SqlConnection connection = new(Database.ConnectionString);
                query = $"Select * From [Finance].[MasterTransactions] Where OldId = {moneyTransaction.ID}";
                check = connection.QueryFirstOrDefault<AccountTransaction>(query);

                if (check == null)
                {
                    _ = connection.Insert(newData);
                }
            }
        }

        #region To Delete From DB
        //string query = $"Select * From [JobOrder].[Panels(ProformaInvoiced)] Where JobOrderID  = {JobOrderData.ID} ";
        //panelsTransaction = new ObservableCollection<TransactionPanel>(connection.Query<TransactionPanel>(query));
        //string query = $"Select IsNUll(MAX(InvoiceNumber),0) As InvoiceNumber  From [JobOrder].[ProformaInvoiceNumber] Where Year = {DateTime.Now.Year}";
        //invoiceNumber = connection.QueryFirstOrDefault<int>(query) + 1;
        //        string query;
        //        query = $"Select * From [JobOrder].[ProformaInvoicesInformations] Where InvoiceNumber  = {invoiceData.Number}";
        //        invoiceInformation = connection.QueryFirstOrDefault<Data.Finance.ProformaInvoiceInformation>(query);

        //        query = $"Select * From [JobOrder].[Panels(ProformaInvoiceDetails)] Where InvoiceNumber = {invoiceData.Number}";
        //        panels = connection.Query<IPanel>(query).ToList();
        #endregion

        //public static void ConvertMoneyTransactionsData2()
        //{
        //    string query;
        //    ObservableCollection<MoneyTransaction> moneyTransactions;
        //    ObservableCollection<SubTransaction> transactions = new ObservableCollection<SubTransaction>();
        //    using (SqlConnection connection = new SqlConnection(Database.ConnectionString))
        //    {
        //        query = $"Select * From [Finance].[MoneyTransactions] ";
        //        moneyTransactions = new ObservableCollection<MoneyTransaction>(connection.Query<MoneyTransaction>(query));
        //    }

        //    foreach (MoneyTransaction moneyTransaction in moneyTransactions)
        //    {
        //        SubTransaction newData = new SubTransaction();
        //        newData.OldId = moneyTransaction.ID;
        //        newData.Date = moneyTransaction.Date;
        //        newData.Description = moneyTransaction.Description;
        //        newData.Amount = (double)moneyTransaction.Amount;
        //        newData.AccountId = moneyTransaction.AccountID;
        //        newData.Post = true;
        //        newData.AttachmentId = moneyTransaction.AttachmentID;

        //        if (moneyTransaction.Type == "Project")
        //        {
        //            newData.Type = Enums.AccountingTransactions.Receipt.ToString();
        //            newData.CustomerId = moneyTransaction.CustomerID;
        //        }
        //        else
        //        {
        //            newData.Type = Enums.AccountingTransactions.Payment.ToString();
        //            newData.SupplierId = moneyTransaction.SupplierID;
        //        }

        //        SubTransaction check;
        //        using (SqlConnection connection = new SqlConnection(Database.ConnectionString))
        //        {
        //            query = $"Select * From [Finance].[MasterTransactions] Where OldId = {moneyTransaction.ID}";
        //            check = connection.QueryFirstOrDefault<SubTransaction>(query);

        //            if (check == null)
        //            {
        //                _ = connection.Insert(newData);
        //            }
        //        }
        //    }
        //}
    }
}

