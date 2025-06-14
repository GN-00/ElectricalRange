using Dapper;

using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Views;

using System;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Services
{
    public static class SalesInvoicesServices
    {
        public static void GetCode(Invoice invoice)
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select IsNull(Max(Number), 0) From [Finance].[CustomersInvoices] " +
                    $"Where Year = {DateTime.Now.Year} " +
                    $"And Type = 'Items'";

            invoice.Number = connection.QueryFirstOrDefault<int>(query) + 1;
            invoice.Month = DateTime.Now.Month;
            invoice.Year = DateTime.Now.Year;
            invoice.Code = $"M-{DateTime.Now.Year:0000}{DateTime.Now.Month:00}{invoice.Number:0000}";
        }

        public static void PrintInvoice(string invoiceCode, IView checkPoint)
        {
            JobOrdersInvoicesServices.PrintInvoice(invoiceCode, checkPoint);
        }
    }
}
