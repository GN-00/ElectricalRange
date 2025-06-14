using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Store
{
    [Table("[Store].[ReturnInvoices]")]
    public class ReturnInvoice : Base
    {
        [Key]
        public int ID { get; set; }
        public int OriginalInvoiceID { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }

        [Write(false)]
        public string Code { get; set; }
        [Write(false)]
        public int Month { get; set; }
        [Write(false)]
        public int Year { get; set; }

        [Write(false)]
        public long CompanyVAT { get; set; } = Database.CompanyVAT;

        [Write(false)]
        public string JobOrderCode { get; set; }

        [Write(false)]
        public string ProjectName { get; set; }

        [Write(false)]
        public string OriginalInvoiceCode { get; set; }

        [Write(false)]
        public string SupplierName { get; set; }

        [Write(false)]
        public string SupplierAddress { get; set; }

        [Write(false)]
        public string Attention { get; set; }

        [Write(false)]
        public string SupplierVAT { get; set; }

        [Write(false)]
        public double ReturnValue { get; set; }
    }
}
