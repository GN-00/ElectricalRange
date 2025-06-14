using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[Acknowledgment]")]
    public class Acknowledgment : Base
    {
        [Key]
        public int ID { get; set; }
        public int JobOrderID { get; set; }

        private bool _PaymentToggle1 = true;
        public bool PaymentToggle1
        {
            get => _PaymentToggle1;
            set => SetValue(ref _PaymentToggle1, value);
        }

        private bool _PaymentToggle2 = false;
        public bool PaymentToggle2
        {
            get => _PaymentToggle2;
            set => SetValue(ref _PaymentToggle2, value);
        }

        private decimal? _DownPayment = 50;
        public decimal? DownPayment
        {
            get => _DownPayment;
            set => SetValue(ref _DownPayment, value);
        }
        private bool _InAdvanceToggle = false;
        public bool InAdvanceToggle
        {
            get => _InAdvanceToggle;
            set => SetValue(ref _InAdvanceToggle, value);
        }

        private decimal? _BeforeDelivery = 50;
        public decimal? BeforeDelivery
        {
            get => _BeforeDelivery;
            set => SetValue(ref _BeforeDelivery, value);
        }

        private decimal? _AfterDelivery;
        public decimal? AfterDelivery
        {
            get => _AfterDelivery;
            set => SetValue(ref _AfterDelivery, value);
        }

        private decimal? _Testing;
        public decimal? Testing
        {
            get => _Testing;
            set => SetValue(ref _Testing, value);
        }

        private string _PaymentOther;
        public string PaymentOther
        {
            get => _PaymentOther;
            set => SetValue(ref _PaymentOther, value);
        }

        private bool _DrawingToggle1 = false;
        public bool DrawingToggle1
        {
            get => _DrawingToggle1;
            set => SetValue(ref _DrawingToggle1, value);
        }

        private bool _DrawingToggle2 = false;
        public bool DrawingToggle2
        {
            get => _DrawingToggle2;
            set => SetValue(ref _DrawingToggle2, value);
        }

        private bool _DrawingToggle3 = true;
        public bool DrawingToggle3
        {
            get => _DrawingToggle3;
            set => SetValue(ref _DrawingToggle3, value);
        }

        private bool _DrawingToggle4 = false;
        public bool DrawingToggle4
        {
            get => _DrawingToggle4;
            set => SetValue(ref _DrawingToggle4, value);
        }

        private bool _DrawingToggle5 = false;
        public bool DrawingToggle5
        {
            get => _DrawingToggle5;
            set => SetValue(ref _DrawingToggle5, value);
        }

        private DateTime? _DrawingDate;
        public DateTime? DrawingDate
        {
            get => _DrawingDate;
            set => SetValue(ref _DrawingDate, value);
        }

        private int? _DrawingPeriod = 1;
        public int? DrawingPeriod
        {
            get => _DrawingPeriod;
            set => SetValue(ref _DrawingPeriod, value);
        }

        private string _DrawingUnit1 = "Week";
        public string DrawingUnit1
        {
            get => _DrawingUnit1;
            set => SetValue(ref _DrawingUnit1, value);
        }

        private string _DrawingCondition1 = "Advance Payment";
        public string DrawingCondition1
        {
            get => _DrawingCondition1;
            set => SetValue(ref _DrawingCondition1, value);
        }

        private int? _DrawingStartPeriod;
        public int? DrawingStartPeriod
        {
            get => _DrawingStartPeriod;
            set => SetValue(ref _DrawingStartPeriod, value);
        }

        private int? _DrawingEndPeriod;
        public int? DrawingEndPeriod
        {
            get => _DrawingEndPeriod;
            set => SetValue(ref _DrawingEndPeriod, value);
        }

        private string _DrawingUnit2;
        public string DrawingUnit2
        {
            get => _DrawingUnit2;
            set => SetValue(ref _DrawingUnit2, value);
        }

        private string _DrawingCondition2;
        public string DrawingCondition2
        {
            get => _DrawingCondition2;
            set => SetValue(ref _DrawingCondition2, value);
        }

        private string _DrawingOther;
        public string DrawingOther
        {
            get => _DrawingOther;
            set => SetValue(ref _DrawingOther, value);
        }

        private bool _DeliveryToggle1 = false;
        public bool DeliveryToggle1
        {
            get => _DeliveryToggle1;
            set => SetValue(ref _DeliveryToggle1, value);
        }

        private bool _DeliveryToggle2 = false;
        public bool DeliveryToggle2
        {
            get => _DeliveryToggle2;
            set => SetValue(ref _DeliveryToggle2, value);
        }

        private bool _DeliveryToggle3 = true;
        public bool DeliveryToggle3
        {
            get => _DeliveryToggle3;
            set => SetValue(ref _DeliveryToggle3, value);
        }

        private bool _DeliveryToggle4 = false;
        public bool DeliveryToggle4
        {
            get => _DeliveryToggle4;
            set => SetValue(ref _DeliveryToggle4, value);
        }

        private bool _DeliveryToggle5 = false;
        public bool DeliveryToggle5
        {
            get => _DeliveryToggle5;
            set => SetValue(ref _DeliveryToggle5, value);
        }

        private DateTime? _DeliveryDate;
        public DateTime? DeliveryDate
        {
            get => _DeliveryDate;
            set => SetValue(ref _DeliveryDate, value);
        }

        private int? _DeliveryPeriod = 1;
        public int? DeliveryPeriod
        {
            get => _DeliveryPeriod;
            set => SetValue(ref _DeliveryPeriod, value);
        }

        private string _DeliveryUnit1 = "Week";
        public string DeliveryUnit1
        {
            get => _DeliveryUnit1;
            set => SetValue(ref _DeliveryUnit1, value);
        }

        private string _DeliveryCondition1 = "Advance Payment & Drawing Approval";
        public string DeliveryCondition1
        {
            get => _DeliveryCondition1;
            set => SetValue(ref _DeliveryCondition1, value);
        }

        private int? _DeliveryStartPeriod;
        public int? DeliveryStartPeriod
        {
            get => _DeliveryStartPeriod;
            set => SetValue(ref _DeliveryStartPeriod, value);
        }

        private int? _DeliveryEndPeriod;
        public int? DeliveryEndPeriod
        {
            get => _DeliveryEndPeriod;
            set => SetValue(ref _DeliveryEndPeriod, value);
        }

        private string _DeliveryUnit2;
        public string DeliveryUnit2
        {
            get => _DeliveryUnit2;
            set => SetValue(ref _DeliveryUnit2, value);
        }

        private string _DeliveryCondition2;
        public string DeliveryCondition2
        {
            get => _DeliveryCondition2;
            set => SetValue(ref _DeliveryCondition2, value);
        }

        private string _DeliveryOther;
        public string DeliveryOther
        {
            get => _DeliveryOther;
            set => SetValue(ref _DeliveryOther, value);
        }

        private string _DeliveryPlace = "Ex-Factory";
        public string DeliveryPlace
        {
            get => _DeliveryPlace;
            set => SetValue(ref _DeliveryPlace, value);
        }

        private int? _WarrantyPeriod = 1;
        public int? WarrantyPeriod
        {
            get => _WarrantyPeriod;
            set => SetValue(ref _WarrantyPeriod, value);
        }

        private string _WarrantyUnit = "Year";
        public string WarrantyUnit
        {
            get => _WarrantyUnit;
            set => SetValue(ref _WarrantyUnit, value);
        }

        private string _WarrantyCondition = "Delivery";
        public string WarrantyCondition
        {
            get => _WarrantyCondition;
            set => SetValue(ref _WarrantyCondition, value);
        }

        private bool _CancelationToggle = false;
        public bool CancelationToggle
        {
            get => _CancelationToggle;
            set => SetValue(ref _CancelationToggle, value);
        }

        private decimal _Cancellation1 = 0;
        public decimal Cancellation1
        {
            get => _Cancellation1;
            set => SetValue(ref _Cancellation1, value);
        }
        private decimal _Cancellation2 = 20;
        public decimal Cancellation2
        {
            get => _Cancellation2;
            set => SetValue(ref _Cancellation2, value);
        }
        private decimal _Cancellation3 = 50;
        public decimal Cancellation3
        {
            get => _Cancellation3;
            set => SetValue(ref _Cancellation3, value);
        }
        private decimal _Cancellation4 = 100;
        public decimal Cancellation4
        {
            get => _Cancellation4;
            set => SetValue(ref _Cancellation4, value);
        }
    }
}
