using Dapper;

using ProjectsNow.Data;
using ProjectsNow.Data.Users;

using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class UserController
    {
        public static User CheckUserInquiryID(SqlConnection connection, int inquiryID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where InquiryID = {inquiryID}");
            return record;
        }
        public static User CheckUserQuotationID(SqlConnection connection, int quotationID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where QuotationID = {quotationID}");
            return record;
        }
        public static User CheckUserJobOrderID(SqlConnection connection, int jobOrderID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where JobOrderID = {jobOrderID}");
            return record;
        }
        //public static User CheckUserCustomerID(SqlConnection connection, int customerID)
        //{
        //    User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where CustomerID = {customerID}");
        //    return record;
        //}
        //public static User CheckUserContactID(SqlConnection connection, int contactID)
        //{
        //    User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where ContactID = {contactID}");
        //    return record;
        //}
        public static User CheckUserConsultantID(SqlConnection connection, int consultantID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where ConsultantID = {consultantID}");
            return record;
        }
        //public static User CheckUserSupplierID(SqlConnection connection, int supplierID)
        //{
        //    User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where SupplierID = {supplierID}");
        //    return record;
        //}
        //public static User CheckUserAcknowledgementID(SqlConnection connection, int acknowledgementID)
        //{
        //    User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where AcknowledgementID = {acknowledgementID}");
        //    return record;
        //}
        public static User CheckUserAccountID(SqlConnection connection, int accountID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where AccountId = {accountID}");
            return record;
        }
        public static User CheckUserAccountantOrderId(SqlConnection connection, int jobOrderFinanceID)
        {
            User record = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where AccountantOrderId = {jobOrderFinanceID}");
            return record;
        }



        public static void UpdateInquiryID(SqlConnection connection, User user)
        {
            if (user.InquiryId == null)
            {
                _ = connection.Execute($"Update [User].[Users] Set InquiryID = NULL Where Id = {user.Id}");
            }
            else
            {
                _ = connection.Execute($"Update [User].[Users] Set InquiryID = {user.InquiryId} Where Id = {user.Id}");
            }
        }
        public static void UpdateQuotationID(SqlConnection connection, User user)
        {
            if (user.QuotationId == null)
            {
                _ = connection.Execute($"Update [User].[Users] Set QuotationID = NULL Where Id = {user.Id}");
            }
            else
            {
                _ = connection.Execute($"Update [User].[Users] Set QuotationID = {user.QuotationId} Where Id = {user.Id}");
            }
        }
        public static void UpdateJobOrderID(SqlConnection connection, User user)
        {
            if (user.JobOrderId == null)
            {
                _ = connection.Execute($"Update [User].[Users] Set JobOrderID = NULL Where Id = {user.Id}");
            }
            else
            {
                _ = connection.Execute($"Update [User].[Users] Set JobOrderID = {user.JobOrderId} Where Id = {user.Id}");
            }
        }
        public static void UpdateCustomerID(SqlConnection connection, User user)
        {
            if (user.CustomerId == null)
            {
                _ = connection.Execute($"Update [User].[Users] Set CustomerID = NULL Where Id = {user.Id}");
            }
            else
            {
                _ = connection.Execute($"Update [User].[Users] Set CustomerID = {user.CustomerId} Where Id = {user.Id}");
            }
        }

        public static void UpdateConsultantID(SqlConnection connection, User user)
        {
            if (user.ConsultantId == null)
            {
                _ = connection.Execute($"Update [User].[Users] Set ConsultantID = NULL Where Id = {user.Id}");
            }
            else
            {
                _ = connection.Execute($"Update [User].[Users] Set ConsultantID = {user.ConsultantId} Where Id = {user.Id}");
            }
        }
        //public static void UpdateSupplierID(SqlConnection connection, User user)
        //{
        //    if (user.SupplierId == null)
        //    {
        //        _ = connection.Execute($"Update [User].[Users] Set SupplierID = NULL Where Id = {user.Id}");
        //    }
        //    else
        //    {
        //        _ = connection.Execute($"Update [User].[Users] Set SupplierID = {user.SupplierId} Where Id = {user.Id}");
        //    }
        //}
        //public static void UpdateAcknowledgementID(SqlConnection connection, User user)
        //{
        //    if (user.SupplierId == null)
        //    {
        //        _ = connection.Execute($"Update [User].[Users] Set AcknowledgementID = NULL Where Id = {user.Id}");
        //    }
        //    else
        //    {
        //        _ = connection.Execute($"Update [User].[Users] Set AcknowledgementID = {user.AcknowledgementID} Where Id = {user.Id}");
        //    }
        //}
        public static void UpdateAccountID(SqlConnection connection, User user)
        {
            if (user.SupplierId == null)
            {
                _ = connection.Execute($"Update [User].[Users] Set AccountID = NULL Where Id = {user.Id}");
            }
            else
            {
                _ = connection.Execute($"Update [User].[Users] Set AccountID = {user.AccountId} Where Id = {user.Id}");
            }
        }
        public static void UpdateJobOrderFinanceID(SqlConnection connection, User user)
        {
            if (user.SupplierId == null)
            {
                _ = connection.Execute($"Update [User].[Users] Set AccountantOrderId = NULL Where Id = {user.Id}");
            }
            else
            {
                _ = connection.Execute($"Update [User].[Users] Set AccountantOrderId = {user.AccountantOrderId} Where Id = {user.Id}");
            }
        }

        public static void ResetIDs(SqlConnection connection, int userId)
        {
            _ = connection.Execute($"Update [User].[Users] Set " +
                                   $"UserId = NULL, " +
                                   $"InquiryId = NULL, " +
                                   $"QuotationId = NULL, " +
                                   $"JobOrderId = NULL, " +
                                   $"ConsultantId = NULL, " +
                                   $"CustomerId = NULL, " +
                                   $"SupplierId = NULL, " +
                                   $"AccountId = NULL, " +
                                   $"AccountantOrderId = NULL, " +
                                   $"ReferenceId = NULL " +
                                   $"Where Id = {userId}");
        }
    }
}
