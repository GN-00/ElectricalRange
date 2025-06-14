using Dapper.Contrib.Extensions;

using System;
using System.IO;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[PanelsTransactions]")]
    public class TransactionPanel : Base 
    {
        private int? _PanelSN;
        private string _PanelName;
        private DateTime? _Date;
        private DateTime? _DeliveryDate;
        private string _EnclosureType;

        [Key]
        public int TransactionID { get; set; }
        public int JobOrderID { get; set; }
        public int PanelID { get; set; }
        public string Reference { get; set; }

        [Write(false)]
        public string PanelType { get; set; }

        [Write(false)]
        public string PanelTypeArabic { get; set; }

        [Write(false)]
        public string PanelTypeArabicInfo
        {
            get
            {
                return PanelTypeArabic ?? DataInput.Panel.ArabicType(PanelType);
            }
        }

        [Write(false)]
        public int? PanelSN
        {
            get => _PanelSN;
            set => SetValue(ref _PanelSN, value);
        }

        [Write(false)]
        public string PanelName
        {
            get => _PanelName;
            set
            {
                if (SetValue(ref _PanelName, value))
                {
                    OnPropertyChanged(nameof(PanelNameInfo));
                }
            }
        }

        [Write(false)]
        public string PanelNameInfo
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Note))
                {
                    using var reader = new StringReader(PanelName);
                    return $"{reader.ReadLine()} ({Note})";
                }
                else
                {
                    using var reader = new StringReader(PanelName);
                    return $"{reader.ReadLine()}";
                }
            }
        }

        public DateTime? Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }

        public DateTime? DeliveryDate
        {
            get => _DeliveryDate;
            set => SetValue(ref _DeliveryDate, value);
        }

        public int Qty { get; set; }

        [Write(false)]
        public string EnclosureType
        {
            get => _EnclosureType;
            set => SetValue(ref _EnclosureType, value);
        }
        public string Note { get; set; }
        public string Action { get; set; }

        private DateTime? _DateOfCreation;
        
        [Write(false)]
        public DateTime? DateOfCreation
        {
            get => _DateOfCreation;
            set
            {
                if (SetValue(ref _DateOfCreation, value))
                {
                    OnPropertyChanged(nameof(DrawingCode));
                }
            }
        }

        private string _DrawingManualCode;
        [Write(false)]
        public string DrawingManualCode
        {
            get => _DrawingManualCode;
            set
            {
                if (SetValue(ref _DrawingManualCode, value))
                {
                    OnPropertyChanged(nameof(DrawingCode));
                }
            }
        }

        private int? _DrawingNo;
        [Write(false)]
        public int? DrawingNo
        {
            get => _DrawingNo;
            set
            {
                if (SetValue(ref _DrawingNo, value))
                {
                    OnPropertyChanged(nameof(DrawingCode));
                }
            }
        }

        [Write(false)]
        public string DrawingCode
        {
            get
            {
                if (DrawingManualCode != null)
                {
                    return DrawingManualCode;
                }
                else if (DrawingNo != null && DateOfCreation != null)
                {
                    if (PanelType == "Power Panel" || PanelType == "Ready Made Panel" || PanelType == "ECB" || PanelType == "Fuse Panel" || PanelType == "Junction Box")
                    {
                        return $"DP{((DateTime)DateOfCreation).Month:00}-{DrawingNo:000}/{((DateTime)DateOfCreation).Year.ToString().Substring(2, 2)}";
                    }
                    else if (PanelType == "Control Panel" || PanelType == "MCC" || PanelType == "PFC")
                    {
                        return $"CP{((DateTime)DateOfCreation).Month:00}-{DrawingNo:000}/{((DateTime)DateOfCreation).ToString().Substring(2, 2)}";
                    }
                    else if (PanelType == "ATS")
                    {
                        return $"ATS{((DateTime)DateOfCreation).Month:00}-{DrawingNo:000}/{((DateTime)DateOfCreation).ToString().Substring(2, 2)}";
                    }
                    else if (PanelType == "MTS")
                    {
                        return $"MTS{((DateTime)DateOfCreation).Month:00}-{DrawingNo:000}/{((DateTime)DateOfCreation).ToString().Substring(2, 2)}";
                    }
                    else if (PanelType == "Synchronizing Panel")
                    {
                        return $"SYN{((DateTime)DateOfCreation).Month:00}-{DrawingNo:000}/{((DateTime)DateOfCreation).ToString().Substring(2, 2)}";
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        [Write(false)]
        public decimal NetPrice { get; set; }

        [Write(false)]
        public decimal VATValue { get; set; }

        [Write(false)]
        public decimal GrossPrice { get; set; }

        [Write(false)]
        public decimal UnitNetPrice { get; set; }

        [Write(false)]
        public decimal UnitVATValue { get; set; }

        [Write(false)]
        public decimal UnitGrossPrice { get; set; }
    }
}
