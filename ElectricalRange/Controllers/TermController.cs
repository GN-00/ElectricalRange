using Dapper.Contrib.Extensions;

using ProjectsNow.Data.Quotations;
using ProjectsNow.Enums;

using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class TermController
    {
        public static void DefaultTerms(SqlConnection connection, int quotationID)
        {
            List<Term> list = new()
            {
                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 1,
                    Condition = "This offer consists of LV Switchgear, according to the attached Bill of Prices and the technical part that complement one another.",
                    ConditionType = ConditionTypes.ScopeOfSupply.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 2,
                    Condition = "Our offer is limited to the details mentioned; any changes which could be requested by the purchaser will be subject to further offer.",
                    ConditionType = ConditionTypes.ScopeOfSupply.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 1,
                    Condition = "In Saudi riyals currency of account and payment.",
                    ConditionType = ConditionTypes.TotalPrice.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 2,
                    Condition = "Firm within the validity and delivery periods as defined hereunder.",
                    ConditionType = ConditionTypes.TotalPrice.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 1,
                    Condition = "70 % of the total price along with the order as down payment by cheque, cash or bank transfer.",
                    ConditionType = ConditionTypes.PaymentConditions.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 2,
                    Condition = "30 % of the total price before delivery by cheque, cash or bank transfer.",
                    ConditionType = ConditionTypes.PaymentConditions.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 1,
                    Condition = "This offer is valid for (1) week from offer date.",
                    ConditionType = ConditionTypes.ValidityPeriod.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 2,
                    Condition = "Should extension of validity be required, please contact us.",
                    ConditionType = ConditionTypes.ValidityPeriod.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 1,
                    Condition = "Will be submitted upon the period which will be mentioned in our order acknowledgment after receiving down payment.",
                    ConditionType = ConditionTypes.ShopDrawingSubmittals.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 1,
                    Condition = "Ex-Store, Jeddah.",
                    ConditionType = ConditionTypes.Delivery.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 2,
                    Condition = "TBA, from the date of: [P.O or contract], down payment & shop drawing approval.",
                    ConditionType = ConditionTypes.Delivery.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 1,
                    Condition = "The Guarantee period will be 12 months from the date of delivery, for material defect and Malfunction except damage unintentionally done by customer. ",
                    ConditionType = ConditionTypes.Guarantee.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 1,
                    Condition = "Enclosure sizes and dimensions may be subject to change during shop drawings preparation depending on the final design.",
                    ConditionType = ConditionTypes.Remarks.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },

                new Term()
                {
                    QuotationID = quotationID,
                    Sort = 2,
                    Condition = "ER-PCAPS has the right to change the prices in case of partial order.",
                    ConditionType = ConditionTypes.Remarks.ToString(),
                    IsUsed = true,
                    IsDefault = true
                },
            };

            _ = connection.Insert(list);
        }
    }
}
