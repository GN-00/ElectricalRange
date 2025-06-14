using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201203 : SqlUpdate
    {
        public U202201203()
        {
            Key = nameof(U202201203);
            Command = "Update [Quotation].[Quotations] Set " +
                      "[Discount] = 11.8056903 " +
                      "Where [QuotationID] = 3130";

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


