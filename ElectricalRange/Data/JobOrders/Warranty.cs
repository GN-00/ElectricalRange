using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[Warranties]")]
    public class Warranty : Base
    {
        private string _Code;
        private DateTime _Date;
        private bool _Installation;
        private bool _Delivery;
        private bool _Service;
        private bool _Other;
        private string _OtherInfo;
        private int _TotalUnits;
        private string _TotalUnitsText;
        private string _DeliveryCodeLinked;
        private int _IssuedById;
        private string _IssuedBy;
        private string _Mobile;

        [Key]
        public int Id { get; set; }
        public int JobOrderId { get; set; }
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }

        public int Number { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }

        [Write(false)]
        public string DateInfo => Date.ToString("dd-MM-yyyy");

        [Write(false)]
        public string JobOrderCode { get; set; }

        [Write(false)]
        public string Customer { get; set; }

        [Write(false)]
        public string Location { get; set; }

        [Write(false)]
        public string Contact { get; set; }

        [Write(false)]
        public string Project { get; set; }

        public bool Installation
        {
            get => _Installation;
            set => SetValue(ref _Installation, value);
        }

        public bool Delivery
        {
            get => _Delivery;
            set => SetValue(ref _Delivery, value);
        }

        public bool Service
        {
            get => _Service;
            set => SetValue(ref _Service, value);
        }

        public bool Other
        {
            get => _Other;
            set => SetValue(ref _Other, value);
        }

        public string OtherInfo
        {
            get => _OtherInfo;
            set => SetValue(ref _OtherInfo, value);
        }

        [Write(false)]
        public string StartFrom
        {
            get
            {
                if (Installation)
                    return nameof(Installation);
                else if (Delivery)
                    return nameof(Delivery);
                else if (Service)
                    return nameof(Service);
                else
                    return OtherInfo;
            }
        }

        [Write(false)]
        public int TotalUnits
        {
            get => _TotalUnits;
            set => SetValue(ref _TotalUnits, value);
        }


        [Write(false)]
        public string TotalUnitsInfo1
        {
            get
            {
                if (TotalUnits == 1)
                    return "panel";
                else
                    return "panels";
            }
        }

        [Write(false)]
        public string TotalUnitsInfo2
        {
            get
            {
                if (TotalUnits == 1)
                    return "/unit";
                else
                    return "/units";
            }
        }


        [Write(false)]
        public string TotalUnitsInfo3
        {
            get
            {
                if (TotalUnits == 1)
                    return "is";
                else
                    return "are";
            }
        }

        [Write(false)]
        public string TotalUnitsText
        {
            get => _TotalUnitsText;
            set => SetValue(ref _TotalUnitsText, value);
        }

        public string DeliveryCodeLinked
        {
            get => _DeliveryCodeLinked;
            set => SetValue(ref _DeliveryCodeLinked, value);
        }

        public int IssuedById
        {
            get => _IssuedById;
            set => SetValue(ref _IssuedById, value);
        }

        [Write(false)]
        public string IssuedBy
        {
            get => _IssuedBy;
            set => SetValue(ref _IssuedBy, value);
        }

        [Write(false)]
        public string Mobile
        {
            get => _Mobile;
            set => SetValue(ref _Mobile, value);
        }

        public int Duration { get; set; } = 1;
        public string DurationUnit { get; set; } = "Year";

        [Write(false)]
        public string DurationUnitInfo
        {
            get
            {
                if (DurationUnit == "Year")
                {
                    if (Duration == 1)
                        return "Year";
                    else
                        return "Years";
                }
                else if (DurationUnit == "Month")
                {
                    if (Duration == 1)
                        return "Month";
                    else
                        return "Monthes";
                }
                else
                {
                    if (Duration == 1)
                        return "Day";
                    else
                        return "Days";
                }
            }
        }

        [Write(false)]
        public string DurationInfo => $"{Duration} {DurationUnitInfo}";

        [Write(false)]
        public string DurationText => DataInput.Input.NumberToWords(Duration);

        [Write(false)]
        public DateTime LastDelivery { get; set; }

        [Write(false)]
        public DateTime EndDate
        {
            get
            {
                if (DurationUnit == "Day")
                    return LastDelivery.AddDays(Duration);
                else if (DurationUnit == "Month")
                    return LastDelivery.AddMonths(Duration);
                else
                    return LastDelivery.AddYears(Duration);
            }
        }
    }
}
