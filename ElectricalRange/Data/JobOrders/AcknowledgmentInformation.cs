using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.JobOrders
{
    public class AcknowledgmentInformation
    {
        public int JobOrderID { get; set; }
        public string Code { get; set; }
        public string PurchaseOrder { get; set; }
        public int CodeNumber { get; set; }
        public int CodeYear { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string ProjectName { get; set; }
        public string ContactName { get; set; }

        public bool PaymentToggle1 { get; set; }
        public bool PaymentToggle2 { get; set; }
        public bool InAdvanceToggle { get; set; }
        public decimal? DownPayment { get; set; }
        public decimal? BeforeDelivery { get; set; }
        public decimal? AfterDelivery { get; set; }
        public decimal? Testing { get; set; }
        public string PaymentOther { get; set; }

        public bool DrawingToggle1 { get; set; }
        public bool DrawingToggle2 { get; set; }
        public bool DrawingToggle3 { get; set; }
        public bool DrawingToggle4 { get; set; }
        public bool DrawingToggle5 { get; set; }

        public DateTime? DrawingDate { get; set; }

        public int? DrawingPeriod { get; set; }
        public string DrawingUnit1 { get; set; }
        public string DrawingCondition1 { get; set; }
        public int? DrawingStartPeriod { get; set; }
        public int? DrawingEndPeriod { get; set; }
        public string DrawingUnit2 { get; set; }
        public string DrawingCondition2 { get; set; }
        public string DrawingOther { get; set; }

        public bool DeliveryToggle1 { get; set; }
        public bool DeliveryToggle2 { get; set; }
        public bool DeliveryToggle3 { get; set; }
        public bool DeliveryToggle4 { get; set; }
        public bool DeliveryToggle5 { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public int? DeliveryPeriod { get; set; }
        public string DeliveryUnit1 { get; set; }
        public string DeliveryCondition1 { get; set; }
        public int? DeliveryStartPeriod { get; set; }
        public int? DeliveryEndPeriod { get; set; }
        public string DeliveryUnit2 { get; set; }

        public string DeliveryCondition2 { get; set; }
        public string DeliveryOther { get; set; }
        public string DeliveryPlace { get; set; }
        public int? WarrantyPeriod { get; set; }
        public string WarrantyUnit { get; set; }

        public string WarrantyCondition { get; set; }
        public bool CancelationToggle { get; set; }
        public decimal Cancellation1 { get; set; }
        public decimal Cancellation2 { get; set; }
        public decimal Cancellation3 { get; set; }
        public decimal Cancellation4 { get; set; }

        public decimal QuotationCost { get; set; }
        public decimal VAT { get; set; }
        public decimal Discount { get; set; }

        public string Payment
        {
            get
            {
                if (PaymentToggle1)
                {
                    string payment = "";
                    if (InAdvanceToggle)
                    {
                        return "100% In Advance";
                    }

                    if (DownPayment > 0)
                    {
                        payment += $"{DownPayment}% Down Payment - ";
                    }

                    if (BeforeDelivery > 0)
                    {
                        payment += $"{BeforeDelivery}% Before Delivery - ";
                    }

                    if (Testing > 0)
                    {
                        payment = payment.Substring(0, payment.Length - 3) + "\n";
                    }

                    if (AfterDelivery > 0)
                    {
                        payment += $"{AfterDelivery}% After Delivery - ";
                    }

                    if (Testing > 0)
                    {
                        payment += $"{Testing}% Afte Testing & Commissioning - ";
                    }

                    return payment.Substring(0, payment.Length - 3) + ".";
                }
                else if (PaymentToggle2)
                {
                    return PaymentOther + ".";
                }
                else
                {
                    return null;
                }
            }
        }
        public string Drawing
        {
            get
            {
                if (DrawingToggle1)
                {
                    if (DrawingDate == null)
                    {
                        return null;
                    }
                    else
                    {
                        return ((DateTime)DrawingDate).ToString("dd/MM/yyyy") + ".";
                    }
                }

                else if (DrawingToggle2)
                {
                    return "As Soon As Possible.";
                }
                else if (DrawingToggle3)
                {
                    return $"{DrawingPeriod} {DrawingUnit1} after {DrawingCondition1}. ";
                }
                else if (DrawingToggle4)
                {
                    return $"{DrawingStartPeriod} - {DrawingEndPeriod} {DrawingUnit2} after {DrawingCondition2}. ";
                }
                else if (DrawingToggle5)
                {
                    return $"{DrawingOther}. ";
                }
                else
                {
                    return null;
                }
            }
        }
        public string Delivery
        {
            get
            {
                if (DeliveryToggle1)
                {
                    if (DeliveryDate == null)
                    {
                        return null;
                    }
                    else
                    {
                        return ((DateTime)DeliveryDate).ToString("dd/MM/yyyy") + ".";
                    }
                }

                else if (DeliveryToggle2)
                {
                    return "As Soon As Possible.";
                }
                else if (DeliveryToggle3)
                {
                    return $"{DeliveryPeriod} {DeliveryUnit1} after {DeliveryCondition1}. ";
                }
                else if (DeliveryToggle4)
                {
                    return $"{DeliveryStartPeriod} - {DeliveryEndPeriod} {DeliveryUnit2} after {DeliveryCondition2}. ";
                }
                else if (DeliveryToggle5)
                {
                    return $"{DeliveryOther}. ";
                }
                else
                {
                    return null;
                }
            }
        }
        public string Warranty => $"{WarrantyPeriod} {WarrantyUnit} after {WarrantyCondition}.";

        public string OrderAcknowledgementCode
        {
            get
            {
                if (CodeNumber != 0)
                {
                    if (CodeNumber > 99)
                    {
                        return $"ER-{(CodeYear - Database.CompanyCreationYear) * 1000 + CodeNumber}/{CodeYear}";
                    }
                    else
                    {
                        return $"ER-{(CodeYear - Database.CompanyCreationYear) * 100 + CodeNumber}/{CodeYear}";
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        [Write(false)]
        public decimal VATPercentage => VAT * 100m;
        [Write(false)]
        public decimal QuotationEstimatedPrice { get; set; }
        [Write(false)]
        public decimal QuotationVATValue { get; set; }
        [Write(false)]
        public decimal QuotationFinalPrice { get; set; }
        [Write(false)]
        public string TextQuotationFinalPrice => DataInput.Input.NumberToSRWords(QuotationFinalPrice);

    }
}
