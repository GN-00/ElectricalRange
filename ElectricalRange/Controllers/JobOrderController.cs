using Dapper;

using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Quotations;

using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class JobOrderController
    {
        public static ObservableCollection<Quotation> QuotationsWaitPO(SqlConnection connection, int year)
        {
            string query = $"Select * " +        
                           $"From [JobOrder].[QuotationsWaitPO] " +
                           $"Where QuotationYear = {year} ";

            return new ObservableCollection<Quotation>(connection.Query<Quotation>(query));
        }
        public static ObservableCollection<int> QuotationsWaitingPOYears(SqlConnection connection)
        {
            string query = $"Select " +
                           $"QuotationYear as Year " +
                           $"From [JobOrder].[QuotationsWaitPO] " +
                           $"Group By QuotationYear " +
                           $"Order By QuotationYear Desc";

            return new ObservableCollection<int>(connection.Query<int>(query));
        }

        public static ObservableCollection<Quotation> GetJobOrdersQuotations(SqlConnection connection, int year)
        {
            string query = $"Select * From [JobOrder].[JobOrdersInformation] " +
                           $"Where CodeYear = {year} " +
                           $"Order By CodeYear DESC, CodeNumber DESC";

            return new ObservableCollection<Quotation>(connection.Query<Quotation>(query));
        }
        public static ObservableCollection<int> GetJobOrdersQuotationsYears(SqlConnection connection)
        {
            string query = $"Select " +
                           $"CodeYear as Year " +
                           $"From [JobOrder].[JobOrdersInformation] " +
                           $"Group By CodeYear " +
                           $"Order By CodeYear Desc";

            return new ObservableCollection<int>(connection.Query<int>(query));
        }

        public static ObservableCollection<JobOrder> JobOrders(SqlConnection connection, int year)
        {
            string query = $"Select * From [JobOrder].[JobOrdersInformation] " +
                           $"Where CodeYear = {year} " +
                           $"Order By CodeYear DESC, CodeNumber DESC";

            return new ObservableCollection<JobOrder>(connection.Query<JobOrder>(query));
        }
        public static ObservableCollection<int> JobOrdersYears(SqlConnection connection)
        {
            string query = $"Select " +
                           $"CodeYear as Year " +
                           $"From [JobOrder].[JobOrdersInformation] " +
                           $"Group By CodeYear " +
                           $"Order By CodeYear Desc";

            return new ObservableCollection<int>(connection.Query<int>(query));
        }

        public static JobOrder JobOrder(SqlConnection connection, int jobOrderID)
        {
            string query = $"Select * From [JobOrder].[JobOrdersInformation] " +
                           $"Where ID = {jobOrderID} ";
            JobOrder record = connection.QueryFirstOrDefault<JobOrder>(query);
            return record;
        }

        public static ObservableCollection<JobOrder> GetRunningJobOrders(SqlConnection connection, int year)
        {
            string query = $"Select * From [JobOrder].[JobOrders(Running)] " +
                           $"Where CodeYear = {year}" +
                           $"Order By CodeYear DESC, CodeNumber DESC";

            return new ObservableCollection<JobOrder>(connection.Query<JobOrder>(query));
        }
        public static ObservableCollection<int> GetRunningJobOrdersYears(SqlConnection connection)
        {
            string query = $"Select CodeYear as Year " +
                           $"From [JobOrder].[JobOrders(Running)] " +
                           $"Group By CodeYear " +
                           $"Order By CodeYear Desc";

            return new ObservableCollection<int>(connection.Query<int>(query));
        }

        public static ObservableCollection<JobOrder> GetClosedJobOrders(SqlConnection connection, int year)
        {
            string query = $"Select * From [JobOrder].[JobOrders(Closed)] " +
                           $"Where CodeYear = {year} " +
                           $"Order By CodeYear DESC, CodeNumber DESC";
            System.Collections.Generic.IEnumerable<JobOrder> records = connection.Query<JobOrder>(query);
            return new ObservableCollection<JobOrder>(records);
        }

        public static ObservableCollection<JobOrder> GetCanceledJobOrders(SqlConnection connection, int year)
        {
            string query = $"Select * From [JobOrder].[JobOrders(Canceled)View] " +
                           $"Where CodeYear = {year} " +
                           $"Order By CodeYear DESC, CodeNumber DESC";
            System.Collections.Generic.IEnumerable<JobOrder> records = connection.Query<JobOrder>(query);
            return new ObservableCollection<JobOrder>(records);
        }

        public static ObservableCollection<int> GetClosedJobOrdersYears(SqlConnection connection)
        {
            string query = $"Select CodeYear as Year " +
                           $"From [JobOrder].[JobOrders(Closed)] " +
                           $"Group By CodeYear " +
                           $"Order By CodeYear Desc";

            return new ObservableCollection<int>(connection.Query<int>(query));
        }

        public static ObservableCollection<int> GetCanceledJobOrdersYears(SqlConnection connection)
        {
            string query = $"Select CodeYear as Year " +
                           $"From [JobOrder].[JobOrders(Canceled)View] " +
                           $"Group By CodeYear " +
                           $"Order By CodeYear Desc";

            return new ObservableCollection<int>(connection.Query<int>(query));
        }

        public static int GetCodeNumber(SqlConnection connection)
        {
            string query = $"Select MAX(CodeNumber) as CodeNumber From [JobOrder].[JobOrders] Where CodeYear = {DateTime.Now.Year}";
            int codeNumber = connection.QueryFirstOrDefault<JobOrder>(query).CodeNumber;
            return ++codeNumber;
        }

    }
}