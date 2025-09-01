using Dapper.Contrib.Extensions;

using System.IO;

namespace ProjectsNow.Data.Quotations
{

    [Table("[Quotation].[QuotationsPanels]")]
    public class QPanel : Base
    {
        [Key]
        public int PanelID { get; set; }

        [Write(false)]
        public int Id => PanelID;

        public int RevisePanelID { get; set; }
        public int QuotationID { get; set; }
        public int? PurchaseOrdersID { get; set; }

        private int? _PanelSN;
        public int? PanelSN
        {
            get => _PanelSN;
            set => SetValue(ref _PanelSN, value);
        }

        private string _PanelName;
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
                using var reader = new StringReader(PanelName);
                return reader.ReadLine();
            }
        }

        private int _PanelQty = 1;
        public int PanelQty
        {
            get => _PanelQty;
            set
            {
                if (value < 1)
                {
                    return;
                }
                else
                {
                    if (SetValue(ref _PanelQty, value))
                    {
                        OnPropertyChanged(nameof(PanelsCost));
                        OnPropertyChanged(nameof(PanelPrice));
                        OnPropertyChanged(nameof(PanelsPrice));
                    }
                }
            }
        }

        private string _PanelType = "Power Panel";
        public string PanelType
        {
            get => _PanelType;
            set => SetValue(ref _PanelType, value);
        }

        private decimal _PanelProfit;
        public decimal PanelProfit
        {
            get => _PanelProfit;
            set
            {
                if (value != _PanelProfit)
                {
                    if (value < 100)
                    {
                        _PanelProfit = value;

                        OnPropertyChanged();
                        OnPropertyChanged(nameof(PanelsCost));
                        OnPropertyChanged(nameof(PanelPrice));
                        OnPropertyChanged(nameof(PanelsPrice));
                    }
                }
            }
        }

        private string _EnclosureName;
        public string EnclosureName
        {
            get => _EnclosureName;
            set => SetValue(ref _EnclosureName, value);
        }

        private string _EnclosureType;
        public string EnclosureType
        {
            get => _EnclosureType;
            set => SetValue(ref _EnclosureType, value);
        }

        private string _EnclosureInstallation;
        public string EnclosureInstallation
        {
            get => _EnclosureInstallation;
            set => SetValue(ref _EnclosureInstallation, value);
        }

        private decimal? _EnclosureHeight;
        public decimal? EnclosureHeight
        {
            get => _EnclosureHeight;
            set => SetValue(ref _EnclosureHeight, value);
        }

        private decimal? _EnclosureWidth;
        public decimal? EnclosureWidth
        {
            get => _EnclosureWidth;
            set => SetValue(ref _EnclosureWidth, value);
        }

        private decimal? _EnclosureDepth;
        public decimal? EnclosureDepth
        {
            get => _EnclosureDepth;
            set => SetValue(ref _EnclosureDepth, value);
        }

        private string _EnclosureIP;
        public string EnclosureIP
        {
            get => _EnclosureIP;
            set => SetValue(ref _EnclosureIP, value);
        }

        public string EnclosureLocation { get; set; }
        public string EnclosureColor { get; set; }
        public string EnclosureMetalType { get; set; }
        public string EnclosureForm { get; set; }
        public string EnclosureDoor { get; set; }
        public string EnclosureFunctional { get; set; }


        public string Source { get; set; }
        public decimal? Icu { get; set; }
        public string Frequency { get; set; } = "60Hz";
        public string PowerSupplyOperation { get; set; } = "No Parallel Operation";
        public string EarthingSystem { get; set; } = "Earthed (TNS)";
        public string DirectCurrent { get; set; } = "To Earth";

        public string Busbar { get; set; } = "Bare Copper";
        public string BusbarHorizontal { get; set; }
        public string BusbarVertical { get; set; }
        public string NeutralSize { get; set; }
        public string EarthSize { get; set; }

        public string SignallingVoltage { get; set; } = "N/A";
        public string SignallingSource { get; set; } = "N/A";
        public string ControlVoltage { get; set; } = "N/A";
        public string ControlSource { get; set; } = "N/A";
        public string SensorsVoltage { get; set; } = "N/A";
        public string SensorsSource { get; set; } = "N/A";
        public string LightingVoltage { get; set; } = "N/A";
        public string LightingSource { get; set; } = "N/A";

        public string ClimateType { get; set; } = "Normal";
        public string AtmosphereType { get; set; } = "Ordinary";
        public string PollutionRisks { get; set; }
        public decimal? AmbientTemperature { get; set; } = 40;
        public decimal? RelativeHumidity { get; set; } = 50;
        public decimal SeaLevel { get; set; } = 0;

        public string IndicationType { get; set; } = "Without";
        public string IndicationSize { get; set; }
        public string IndicationOther { get; set; }
        public string MeterRelay { get; set; }

        public bool IncomingFixed { get; set; } = false;
        public bool IncomingPlugIn { get; set; } = false;
        public bool IncomingDrawout { get; set; } = false;
        public bool IncomingMotorized { get; set; } = false;
        public bool IncomingBehindDoor { get; set; } = false;
        public bool IncomingThroughPlate { get; set; } = false;
        public bool IncomingInterLock { get; set; } = false;
        public bool IncomingPadlocking { get; set; } = false;
        public bool IncomingShutter { get; set; } = false;
        public bool IncomingThroughDoor { get; set; } = false;
        public bool IncomingThroughCover { get; set; } = false;
        public bool IncomingDirect { get; set; } = false;
        public bool IncomingTerminalBlocks { get; set; } = false;
        public bool IncomingBusbarLinks { get; set; } = false;
        public bool IncomingFront { get; set; } = false;
        public bool IncomingRear { get; set; } = false;
        public bool IncomingLeftRight { get; set; } = false;
        public bool IncomingTopCables { get; set; } = false;
        public bool IncomingTopBusduct { get; set; } = false;
        public bool IncomingBottomCables { get; set; } = false;
        public bool IncomingScrews { get; set; } = false;
        public bool IncomingGlandPlate { get; set; } = false;
        public bool IncomingCableGland { get; set; } = false;
        public bool IncomingShrouding { get; set; } = false;

        public bool OutgoingFixed { get; set; } = false;
        public bool OutgoingPlugIn { get; set; } = false;
        public bool OutgoingDrawout { get; set; } = false;
        public bool OutgoingMotorized { get; set; } = false;
        public bool OutgoingBehindDoor { get; set; } = false;
        public bool OutgoingThroughPlate { get; set; } = false;
        public bool OutgoingInterLock { get; set; } = false;
        public bool OutgoingPadlocking { get; set; } = false;
        public bool OutgoingShutter { get; set; } = false;
        public bool OutgoingThroughDoor { get; set; } = false;
        public bool OutgoingThroughCover { get; set; } = false;
        public bool OutgoingDirect { get; set; } = false;
        public bool OutgoingTerminalBlocks { get; set; } = false;
        public bool OutgoingBusbarLinks { get; set; } = false;
        public bool OutgoingFront { get; set; } = false;
        public bool OutgoingRear { get; set; } = false;
        public bool OutgoingLeftRight { get; set; } = false;
        public bool OutgoingTopCables { get; set; } = false;
        public bool OutgoingTopBusduct { get; set; } = false;
        public bool OutgoingBottomCables { get; set; } = false;
        public bool OutgoingScrews { get; set; } = false;
        public bool OutgoingGlandPlate { get; set; } = false;
        public bool OutgoingCableGland { get; set; } = false;
        public bool OutgoingShrouding { get; set; } = false;

        public string PushButtonON { get; set; } = "Without";
        public string PushButtonOFF { get; set; } = "Without";
        public string PushButtonReset { get; set; } = "Without";
        public string SignallingON { get; set; } = "Without";
        public string SignallingOFF { get; set; } = "Without";
        public string SignallingTrip { get; set; } = "Without";

        public string ExternalLabelType { get; set; } = "Gravely";
        public string ExternalLabelFixeing { get; set; } = "Glued";
        public string ExternalLabelLanguage { get; set; } = "English";
        public string InternalLabelType { get; set; } = "Gravely";
        public string InternalLabelFixeing { get; set; } = "Glued";
        public string InternalLabelLanguage { get; set; } = "English";
        public string EquipmentLabelType { get; set; } = "Sticker";
        public string EquipmentLabelFixeing { get; set; } = "Self Stick";
        public string EquipmentLabelLanguage { get; set; } = "English";
        public string LabelBackground { get; set; } = "Black";
        public string LabelFont { get; set; } = "White";

        public decimal? AuxiliaryVoltageSection { get; set; } = 1.5m;
        public string AuxiliaryVoltageColor { get; set; } = "Black";
        public string AuxiliaryVoltageType { get; set; } = "XLPE";
        public decimal? AuxiliaryCurrentSection { get; set; } = 1.5m;
        public string AuxiliaryCurrentColor { get; set; } = "Black";
        public string AuxiliaryCurrentType { get; set; } = "XLPE";

        public string ApparatusDefind { get; set; } = "Yes";
        public string Weight { get; set; } = "XX";
        public bool ForInformation { get; set; }
        public bool ForProduction { get; set; }
        public bool ForApproval { get; set; }
        public bool AsManufactured { get; set; }
        public string Remarks { get; set; }
        public bool IsSpecial { get; set; }

        private decimal _PanelCost;
        [Write(false)]
        public decimal PanelCost
        {
            get => _PanelCost;
            set
            {
                if (SetValue(ref _PanelCost, value))
                {
                    OnPropertyChanged(nameof(PanelsCost));
                    OnPropertyChanged(nameof(PanelPrice));
                    OnPropertyChanged(nameof(PanelsPrice));
                }
            }
        }

        [Write(false)]
        public decimal PanelsCost => PanelCost * PanelQty;

        [Write(false)]
        public decimal PanelPrice => PanelCost / (1 - PanelProfit / 100m);

        [Write(false)]
        public decimal PanelsPrice => PanelCost * PanelQty / (1 - PanelProfit / 100m);

        [Write(false)]
        public decimal PanelEstimatedPrice { get; set; }

        [Write(false)]
        public decimal PanelsEstimatedPrice { get; set; }

        public override string ToString()
        {
            return PanelSN.ToString();
        }

    }
}
