using System;

namespace ProjectsNow.Data.Finance
{
    public class CustomerReceipt
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public string JobOrderCode { get; set; }
        public string Customer { get; set; }
        public long CustomerVAT { get; set; }
        public long CompanyVAT { get; set; } = Database.CompanyVAT;
        public string Project { get; set; }
        public string Account { get; set; }
        public string ReceiptType 
        { 
            get
            {
                if (Account.ToUpper() == "CASH")
                {
                    return "Cash";
                }
                else
                {
                    return $"Transfer\n{Account}";
                }
            }
            
        }
        public string TextTotal => DataInput.Input.NumberToSRWords(Total);
    }
}
