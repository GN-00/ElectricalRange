using Dapper.Contrib.Extensions;

using ProjectsNow.Data.Application;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Enums;

using System;
using System.Reflection;

namespace ProjectsNow.Data.Quotations
{
    [Table("[Quotation].[Quotations]")]
    public class Quotation : Base, IAccess
    {
        private DateTime? _SubmitDate;
        private string _QuotationStatus = Statuses.Running.ToString();
        private decimal _Discount;
        private decimal _QuotationCost;
        private decimal _QuotationPrice;
        private decimal _QuotationEstimatedPrice;
        private decimal _QuotationFinalPrice;
        private decimal _QuotationDiscountValue;
        private decimal _QuotationVATValue;
        private string _RegisterCode;

        private string _PowerVoltage = "400/230V";
        private string _Phase = "3P + N";
        private string _Frequency = "60Hz";
        private string _NetworkSystem = "AC";
        private string _ControlVoltage = "230V AC";
        private string _TinPlating = "Bare Copper";
        private string _NeutralSize = "Full of Phase";
        private string _EarthSize = "Half of Neutral";
        private string _EarthingSystem = "TNS";

        private string _ProjectName;
        private DateTime _RegisterDate;
        private DateTime _DuoDate;
        private string _Priority = "Normal";
        private string _ProjectStatus;
        private string _Classification;
        private decimal? _Value;
        private string _Location;
        private string _Scope;
        private string _CustomerName;
        private string _EstimationName;
        private string _EstimationCode;
        private string _SalesmanName;
        private string _SalesmanCode;
        private string _BillOfPriceNote;
        private string _BillOfPriceNoteLanguage;

        [Key]
        public int QuotationID { get; set; }

        [Write(false)]
        public int Id => QuotationID;

        [Write(false)]
        public int? JobOrderId { get; set; }

        public string QuotationCode { get; set; }
        public int QuotationNumber { get; set; }
        public int QuotationRevise { get; set; } = 0;
        public DateTime? QuotationReviseDate { get; set; }
        public int QuotationYear { get; set; } = DateTime.Now.Year;
        public int QuotationMonth { get; set; } = DateTime.Now.Month;
        public string QuotationStatus
        {
            get => _QuotationStatus;
            set => SetValue(ref _QuotationStatus, value);
        }
        public DateTime? SubmitDate
        {
            get => _SubmitDate;
            set => SetValue(ref _SubmitDate, value);
        }
        public string PowerVoltage
        {
            get => _PowerVoltage;
            set => SetValue(ref _PowerVoltage, value);
        }
        public string Phase
        {
            get => _Phase;
            set => SetValue(ref _Phase, value);
        }
        public string Frequency
        {
            get => _Frequency;
            set => SetValue(ref _Frequency, value);
        }
        public string NetworkSystem
        {
            get => _NetworkSystem;
            set => SetValue(ref _NetworkSystem, value);
        }
        public string ControlVoltage
        {
            get => _ControlVoltage;
            set => SetValue(ref _ControlVoltage, value);
        }
        public string TinPlating
        {
            get => _TinPlating;
            set => SetValue(ref _TinPlating, value);
        }
        public string NeutralSize
        {
            get => _NeutralSize;
            set => SetValue(ref _NeutralSize, value);
        }
        public string EarthSize
        {
            get => _EarthSize;
            set => SetValue(ref _EarthSize, value);
        }
        public string EarthingSystem
        {
            get => _EarthingSystem;
            set => SetValue(ref _EarthingSystem, value);
        }
        public decimal Discount
        {
            get => _Discount;
            set => SetValue(ref _Discount, value);
        }
        public decimal VAT { get; set; } = AppData.VAT / 100m;

        [Write(false)]
        public decimal VATPercentage => VAT * 100;

        [Write(false)]
        public decimal QuotationCost
        {
            get => _QuotationCost;
            set => SetValue(ref _QuotationCost, value);
        }

        [Write(false)]
        public decimal QuotationPrice
        {
            get => _QuotationPrice;
            set => SetValue(ref _QuotationPrice, value);
        }

        [Write(false)]
        public decimal QuotationEstimatedPrice
        {
            get => _QuotationEstimatedPrice;
            set => SetValue(ref _QuotationEstimatedPrice, value);
        }

        [Write(false)]
        public decimal QuotationFinalPrice
        {
            get => _QuotationFinalPrice;
            set => SetValue(ref _QuotationFinalPrice, value);
        }

        [Write(false)]
        public decimal QuotationDiscountValue
        {
            get => _QuotationDiscountValue;
            set => SetValue(ref _QuotationDiscountValue, value);
        }

        [Write(false)]
        public decimal QuotationVATValue
        {
            get => _QuotationVATValue;
            set => SetValue(ref _QuotationVATValue, value);
        }

        public int InquiryID { get; set; }

        [Write(false)]
        public int CustomerID { get; set; }

        [Write(false)]
        public int ConsultantID { get; set; }

        [Write(false)]
        public int EstimationID { get; set; }

        [Write(false)]
        public int SalesmanID { get; set; }

        [Write(false)]
        public string RegisterCode
        {
            get => _RegisterCode;
            set => SetValue(ref _RegisterCode, value);
        }

        [Write(false)]
        public string ProjectName
        {
            get => _ProjectName;
            set => SetValue(ref _ProjectName, value);
        }

        [Write(false)]
        public DateTime RegisterDate
        {
            get => _RegisterDate;
            set => SetValue(ref _RegisterDate, value);
        }

        [Write(false)]
        public string RegisterDateInfo
        {
            get => _RegisterDate.ToString("dd-MM-yyyy");
        }

        [Write(false)]
        public DateTime DuoDate
        {
            get => _DuoDate;
            set => SetValue(ref _DuoDate, value);
        }

        [Write(false)]
        public string DuoDateInfo
        {
            get => _DuoDate.ToString("dd-MM-yyyy");
        }

        [Write(false)]
        public string Priority
        {
            get => _Priority;
            set => SetValue(ref _Priority, value);
        }

        [Write(false)]
        public int RegisterNumber { get; set; }

        [Write(false)]
        public int RegisterMonth { get; set; }

        [Write(false)]
        public int RegisterYear { get; set; }

        [Write(false)]
        public string DeliveryCondition { get; set; }

        [Write(false)]
        public string ProjectStatus
        {
            get => _ProjectStatus;
            set => SetValue(ref _ProjectStatus, value);
        }

        [Write(false)]
        public string Classification
        {
            get => _Classification;
            set => SetValue(ref _Classification, value);
        }

        [Write(false)]
        public decimal? Value
        {
            get => _Value;
            set => SetValue(ref _Value, value);
        }

        [Write(false)]
        public string Location
        {
            get => _Location;
            set => SetValue(ref _Location, value);
        }

        [Write(false)]
        public string Scope
        {
            get => _Scope;
            set => SetValue(ref _Scope, value);
        }

        [Write(false)]
        public string CustomerName
        {
            get => _CustomerName;
            set => SetValue(ref _CustomerName, value);
        }

        [Write(false)]
        public string EstimationName
        {
            get => _EstimationName;
            set => SetValue(ref _EstimationName, value);
        }

        [Write(false)]
        public string EstimationCode
        {
            get => _EstimationCode;
            set => SetValue(ref _EstimationCode, value);
        }

        [Write(false)]
        public string SalesmanName
        {
            get => _SalesmanName;
            set => SetValue(ref _SalesmanName, value);
        }

        public string BillOfPriceNote
        {
            get => _BillOfPriceNote;
            set => SetValue(ref _BillOfPriceNote, value);
        }

        [Write(false)]
        public string SalesmanCode
        {
            get => _SalesmanCode;
            set => SetValue(ref _SalesmanCode, value);
        }

        public string BillOfPriceNoteLanguage
        {
            get => _BillOfPriceNoteLanguage;
            set => SetValue(ref _BillOfPriceNoteLanguage, value);
        }

        [Write(false)]
        public string ContactName { get; set; }

        [Write(false)]
        public string ContactNumber { get; set; }

        [Write(false)]
        public string OptionCode { get; set; }

        [Write(false)]
        public string OptionName { get; set; }

        public Quotation() { }
        public Quotation(Inquiry inquiry)
        {
            foreach (PropertyInfo property in typeof(Inquiry).GetProperties())
            {
                if (property.SetMethod != null)
                {
                    if (GetType().GetProperty(property.Name) != null)
                    {
                        GetType().GetProperty(property.Name).SetValue(this, property.GetValue(inquiry));
                    }
                }
            }
        }
    }

    public class QuotationsYear
    {
        public int Year { get; set; }
    }
}
