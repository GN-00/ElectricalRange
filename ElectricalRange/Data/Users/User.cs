using Dapper.Contrib.Extensions;

using System.Windows;

namespace ProjectsNow.Data.Users
{
    [Table("[User].[Users]")]
    public class User : Base, IAccess
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }

        [Write(false)]
        public bool IsEstimation { get; set; }
        [Write(false)]
        public bool IsSalesman { get; set; }

        public bool Administrator { get; set; }

        #region Attachments
        public int? WatermarkId { get; set; }
        #endregion

        #region Access
        public int? UserId { get; set; } 
        public int? InquiryId { get; set; }
        public int? QuotationId { get; set; }
        public int? JobOrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? ConsultantId { get; set; }
        public int? SupplierId { get; set; }
        public int? OtherId { get; set; }
        public int? AccountId { get; set; }
        public int? AccountantOrderId { get; set; }
        public int? ReferenceId { get; set; }

        #endregion

        #region Tendaring
        public bool AccessTendaring { get; set; }
        public bool AccessInquiries { get; set; }
        public bool ModifyInquiries { get; set; }

        public bool AccessQuote { get; set; }
        public bool AccessQuotations { get; set; }
        public bool AccessAllQuotations { get; set; }
        public bool ModifyQuotations { get; set; }
        public decimal QuotationsDiscountValue { get; set; }
        #endregion 

        #region Partners

        public bool AccessPartners { get; set; }

        #region Customers
        public bool AccessCustomers { get; set; }
        public bool ModifyCustomers { get; set; }

        public bool AccessCustomersContacts { get; set; }
        public bool ModifyCustomersContacts { get; set; }

        public bool AccessConsultants { get; set; }
        public bool ModifyConsultants { get; set; }

        public bool AccessOthers { get; set; }
        public bool ModifyOthers { get; set; }

        #endregion

        #region Suppliers
        public bool AccessSuppliers { get; set; }
        public bool ModifySuppliers { get; set; }

        public bool AccessSuppliersContacts { get; set; }
        public bool ModifySuppliersContacts { get; set; }
        #endregion

        #endregion

        #region Projects
        public bool AccessProjects { get; set; }
        public bool AccessNewJobOrder { get; set; }
        public bool AccessJobOrders { get; set; }
        public bool ModifyJobOrders { get; set; }
        public bool JobOrdersCanCancel { get; set; }

        public bool AccessQuality { get; set; }
        public bool ModifyQuality { get; set; }
        #endregion

        #region QuotationsStatus
        public bool AccessQuotationsStatus { get; set; }
        public bool ManageQuotationsUpdates { get; set; }

        #endregion

        #region Items
        public bool AccessItems { get; set; }
        public bool AccessReferences { get; set; }
        public bool ReferencesDiscount { get; set; }
        public bool AccessStore { get; set; }
        public bool AccessSalesInvoices { get; set; }
        public bool ModifySalesInvoices { get; set; }
        #endregion

        #region Finance
        public bool AccessFinance { get; set; }

        public bool AccessCompanyAccount { get; set; }
        public bool ModifyCompanyAccount { get; set; }

        public bool AccessReceipts { get; set; }
        public bool ModifyReceipts { get; set; }

        public bool AccessPayments { get; set; }
        public bool ModifyPayments { get; set; }

        public bool AccessCustomersAccounts { get; set; }
        public bool ModifyCustomersAccounts { get; set; }

        public bool AccessSuppliersAccounts { get; set; }
        public bool ModifySuppliersAccounts { get; set; }


        public bool AccessNotifyReceipts { get; set; }
        public bool ModifyNotifyReceipts { get; set; }

        public bool AccessJobordersFinance { get; set; }
        public bool ModifyJobOrdersInvoices { get; set; }

        public bool AccessStatements { get; set; }
        public bool AccessSuppliersInvoices { get; set; }
        public bool AccessTransportation { get; set; }
        public bool AccessExpense { get; set; }


        #endregion

        #region Accountant
        public bool AccessAccounts { get; set; }
        public bool AccessAccountantJobOrders { get; set; }

        #endregion
    }
}