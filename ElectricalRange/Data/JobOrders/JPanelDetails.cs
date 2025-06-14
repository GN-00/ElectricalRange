using Dapper.Contrib.Extensions;

using ProjectsNow.Attributes;
using ProjectsNow.Enums;

using System;
using System.IO;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[Panels]")]
    public class JPanelDetails : Base
    {
        private int? _PanelSN;
        private string _PanelName;
        private string _PanelType;
        private string _PanelTypeArabic;
        private string _Status = Statuses.New.ToString();
        private DateTime? _DateOfCreation = DateTime.Now;
        private string _EnclosureIP;
        private decimal? _EnclosureWidth;
        private decimal? _EnclosureDepth;
        private decimal? _EnclosureHeight;
        private string _EnclosureInstallation;
        private string _EnclosureType;
        private string _EnclosureName;

        [Key]
        [QPanelProperty]
        public int PanelID { get; set; }
        public int JobOrderID { get; set; }
        [QPanelProperty]
        public int PurchaseOrdersID { get; set; }
        [QPanelProperty]
        public int? PanelSN
        {
            get => _PanelSN;
            set => SetValue(ref _PanelSN, value);
        }
        [QPanelProperty]
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

        [QPanelProperty]
        public string PanelType
        {
            get => _PanelType;
            set
            {
                if (SetValue(ref _PanelType, value))
                {
                    PanelTypeArabic = DataInput.Panel.ArabicType(PanelType);
                }
            }   
        }
        public string PanelTypeArabic
        {
            get => _PanelTypeArabic;
            set => SetValue(ref _PanelTypeArabic, value);
        }

        public string Status
        {
            get => _Status;
            set => SetValue(ref _Status, value);
        }
        public DateTime? DateOfCreation
        {
            get => _DateOfCreation;
            set => SetValue(ref _DateOfCreation, value);
        }
        [QPanelProperty]
        public int PanelQty { get; set; }
        [QPanelProperty]
        public decimal PanelProfit { get; set; }
        [QPanelProperty]
        public string EnclosureName
        {
            get => _EnclosureName;
            set => SetValue(ref _EnclosureName, value);
        }
        [QPanelProperty]
        public string EnclosureType
        {
            get => _EnclosureType;
            set => SetValue(ref _EnclosureType, value);
        }
        [QPanelProperty]
        public string EnclosureInstallation
        {
            get => _EnclosureInstallation;
            set => SetValue(ref _EnclosureInstallation, value);
        }
        [QPanelProperty]
        public decimal? EnclosureHeight
        {
            get => _EnclosureHeight;
            set => SetValue(ref _EnclosureHeight, value);
        }
        [QPanelProperty]
        public decimal? EnclosureWidth
        {
            get => _EnclosureWidth;
            set => SetValue(ref _EnclosureWidth, value);
        }
        [QPanelProperty]
        public decimal? EnclosureDepth
        {
            get => _EnclosureDepth;
            set => SetValue(ref _EnclosureDepth, value);
        }
        [QPanelProperty]
        public string EnclosureIP
        {
            get => _EnclosureIP;
            set => SetValue(ref _EnclosureIP, value);
        }
        [QPanelProperty]
        public string EnclosureLocation { get; set; }
        [QPanelProperty]
        public string EnclosureColor { get; set; }
        [QPanelProperty]
        public string EnclosureMetalType { get; set; }
        [QPanelProperty]
        public string EnclosureForm { get; set; }
        [QPanelProperty]
        public string EnclosureDoor { get; set; }
        [QPanelProperty]
        public string EnclosureFunctional { get; set; }

        [QPanelProperty]
        public string Source { get; set; }
        [QPanelProperty]
        public decimal? Icu { get; set; }
        [QPanelProperty]
        public string Frequency { get; set; }
        [QPanelProperty]
        public string PowerSupplyOperation { get; set; }
        [QPanelProperty]
        public string EarthingSystem { get; set; }
        [QPanelProperty]
        public string DirectCurrent { get; set; }

        [QPanelProperty]
        public string Busbar { get; set; }
        [QPanelProperty]
        public string BusbarHorizontal { get; set; }
        [QPanelProperty]
        public string BusbarVertical { get; set; }
        [QPanelProperty]
        public string NeutralSize { get; set; }
        [QPanelProperty]
        public string EarthSize { get; set; }

        [QPanelProperty]
        public string SignallingVoltage { get; set; }
        [QPanelProperty]
        public string SignallingSource { get; set; }
        [QPanelProperty]
        public string ControlVoltage { get; set; }
        [QPanelProperty]
        public string ControlSource { get; set; }
        [QPanelProperty]
        public string SensorsVoltage { get; set; }
        [QPanelProperty]
        public string SensorsSource { get; set; }
        [QPanelProperty]
        public string LightingVoltage { get; set; }
        [QPanelProperty]
        public string LightingSource { get; set; }

        [QPanelProperty]
        public string ClimateType { get; set; }
        [QPanelProperty]
        public string AtmosphereType { get; set; }
        [QPanelProperty]
        public string PollutionRisks { get; set; }
        [QPanelProperty]
        public decimal? AmbientTemperature { get; set; }
        [QPanelProperty]
        public decimal? RelativeHumidity { get; set; }
        [QPanelProperty]
        public decimal SeaLevel { get; set; }

        [QPanelProperty]
        public string IndicationType { get; set; }
        [QPanelProperty]
        public string IndicationSize { get; set; }
        [QPanelProperty]
        public string IndicationOther { get; set; }
        [QPanelProperty]
        public string MeterRelay { get; set; }

        [QPanelProperty]
        public bool IncomingFixed { get; set; }
        [QPanelProperty]
        public bool IncomingPlugIn { get; set; }
        [QPanelProperty]
        public bool IncomingDrawout { get; set; }
        [QPanelProperty]
        public bool IncomingMotorized { get; set; }
        [QPanelProperty]
        public bool IncomingBehindDoor { get; set; }
        [QPanelProperty]
        public bool IncomingThroughPlate { get; set; }
        [QPanelProperty]
        public bool IncomingInterLock { get; set; }
        [QPanelProperty]
        public bool IncomingPadlocking { get; set; }
        [QPanelProperty]
        public bool IncomingShutter { get; set; }
        [QPanelProperty]
        public bool IncomingThroughDoor { get; set; }
        [QPanelProperty]
        public bool IncomingThroughCover { get; set; }
        [QPanelProperty]
        public bool IncomingDirect { get; set; }
        [QPanelProperty]
        public bool IncomingTerminalBlocks { get; set; }
        [QPanelProperty]
        public bool IncomingBusbarLinks { get; set; }
        [QPanelProperty]
        public bool IncomingFront { get; set; }
        [QPanelProperty]
        public bool IncomingRear { get; set; }
        [QPanelProperty]
        public bool IncomingLeftRight { get; set; }
        [QPanelProperty]
        public bool IncomingTopCables { get; set; }
        [QPanelProperty]
        public bool IncomingTopBusduct { get; set; }
        [QPanelProperty]
        public bool IncomingBottomCables { get; set; }
        [QPanelProperty]
        public bool IncomingScrews { get; set; }
        [QPanelProperty]
        public bool IncomingGlandPlate { get; set; }
        [QPanelProperty]
        public bool IncomingCableGland { get; set; }
        [QPanelProperty]
        public bool IncomingShrouding { get; set; }

        [QPanelProperty]
        public bool OutgoingFixed { get; set; }
        [QPanelProperty]
        public bool OutgoingPlugIn { get; set; }
        [QPanelProperty]
        public bool OutgoingDrawout { get; set; }
        [QPanelProperty]
        public bool OutgoingMotorized { get; set; }
        [QPanelProperty]
        public bool OutgoingBehindDoor { get; set; }
        [QPanelProperty]
        public bool OutgoingThroughPlate { get; set; }
        [QPanelProperty]
        public bool OutgoingInterLock { get; set; }
        [QPanelProperty]
        public bool OutgoingPadlocking { get; set; }
        [QPanelProperty]
        public bool OutgoingShutter { get; set; }
        [QPanelProperty]
        public bool OutgoingThroughDoor { get; set; }
        [QPanelProperty]
        public bool OutgoingThroughCover { get; set; }
        [QPanelProperty]
        public bool OutgoingDirect { get; set; }
        [QPanelProperty]
        public bool OutgoingTerminalBlocks { get; set; }
        [QPanelProperty]
        public bool OutgoingBusbarLinks { get; set; }
        [QPanelProperty]
        public bool OutgoingFront { get; set; }
        [QPanelProperty]
        public bool OutgoingRear { get; set; }
        [QPanelProperty]
        public bool OutgoingLeftRight { get; set; }
        [QPanelProperty]
        public bool OutgoingTopCables { get; set; }
        [QPanelProperty]
        public bool OutgoingTopBusduct { get; set; }
        [QPanelProperty]
        public bool OutgoingBottomCables { get; set; }
        [QPanelProperty]
        public bool OutgoingScrews { get; set; }
        [QPanelProperty]
        public bool OutgoingGlandPlate { get; set; }
        [QPanelProperty]
        public bool OutgoingCableGland { get; set; }
        [QPanelProperty]
        public bool OutgoingShrouding { get; set; }

        [QPanelProperty]
        public string PushButtonON { get; set; }
        [QPanelProperty]
        public string PushButtonOFF { get; set; }
        [QPanelProperty]
        public string PushButtonReset { get; set; }
        [QPanelProperty]
        public string SignallingON { get; set; }
        [QPanelProperty]
        public string SignallingOFF { get; set; }
        [QPanelProperty]
        public string SignallingTrip { get; set; }

        [QPanelProperty]
        public string ExternalLabelType { get; set; }
        [QPanelProperty]
        public string ExternalLabelFixeing { get; set; }
        [QPanelProperty]
        public string ExternalLabelLanguage { get; set; }
        [QPanelProperty]
        public string InternalLabelType { get; set; }
        [QPanelProperty]
        public string InternalLabelFixeing { get; set; }
        [QPanelProperty]
        public string InternalLabelLanguage { get; set; }
        [QPanelProperty]
        public string EquipmentLabelType { get; set; }
        [QPanelProperty]
        public string EquipmentLabelFixeing { get; set; }
        [QPanelProperty]
        public string EquipmentLabelLanguage { get; set; }
        [QPanelProperty]
        public string LabelBackground { get; set; }
        [QPanelProperty]
        public string LabelFont { get; set; }

        [QPanelProperty]
        public decimal? AuxiliaryVoltageSection { get; set; }
        [QPanelProperty]
        public string AuxiliaryVoltageColor { get; set; }
        [QPanelProperty]
        public string AuxiliaryVoltageType { get; set; }
        [QPanelProperty]
        public decimal? AuxiliaryCurrentSection { get; set; }
        [QPanelProperty]
        public string AuxiliaryCurrentColor { get; set; }
        [QPanelProperty]
        public string AuxiliaryCurrentType { get; set; }

        [QPanelProperty]
        public string ApparatusDefind { get; set; }
        [QPanelProperty]
        public string Weight { get; set; }
        [QPanelProperty]
        public bool ForInformation { get; set; }
        [QPanelProperty]
        public bool ForProduction { get; set; }
        [QPanelProperty]
        public bool ForApproval { get; set; }
        [QPanelProperty]
        public bool AsManufactured { get; set; }
        [QPanelProperty]
        public string Remarks { get; set; }
        [QPanelProperty]
        public bool IsSpecial { get; set; }

        [Write(false)]
        public decimal PanelCost { get; set; }
        [Write(false)]
        public decimal PanelsCost { get; set; }
        [Write(false)]
        public decimal PanelPrice { get; set; }
        [Write(false)]
        public decimal PanelsPrice { get; set; }
        [Write(false)]
        public decimal PanelEstimatedPrice { get; set; }
        [Write(false)]
        public decimal PanelsEstimatedPrice { get; set; }
        [Write(false)]
        public decimal PanelVATValue { get; set; }
        [Write(false)]
        public decimal PanelsVATValue { get; set; }
        [Write(false)]
        public decimal PanelFinalPrice { get; set; }
        [Write(false)]
        public decimal PanelsFinalPrice { get; set; }


        private int? _DrawingNo;
        public int? DrawingNo
        {
            get => _DrawingNo;
            set => SetValue(ref _DrawingNo, value);
        }

        private string _DrawingManualCode;
        public string DrawingManualCode
        {
            get => _DrawingManualCode;
            set => SetValue(ref _DrawingManualCode, value);
        }
    }
}
