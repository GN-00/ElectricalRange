using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201197 : SqlUpdate
    {
        public U202201197()
        {
            Key = nameof(U202201197);
            Command = "ALTER VIEW [Quotation].[Panels(View)] AS " +
                      "SELECT Quotation.QuotationsPanelsCost.PanelCost, Quotation.QuotationsPanels.PanelID, Quotation.QuotationsPanels.RevisePanelID, Quotation.QuotationsPanels.QuotationID, Quotation.QuotationsPanels.PurchaseOrdersNumber,  " +
                      "Quotation.QuotationsPanels.PanelSN, Quotation.QuotationsPanels.PanelName, Quotation.QuotationsPanels.PanelQty, Quotation.QuotationsPanels.PanelType, Quotation.QuotationsPanels.PanelProfit,  " +
                      "Quotation.QuotationsPanels.EnclosureName, Quotation.QuotationsPanels.EnclosureType, Quotation.QuotationsPanels.EnclosureLocation, Quotation.QuotationsPanels.EnclosureInstallation,  " +
                      "Quotation.QuotationsPanels.EnclosureHeight, Quotation.QuotationsPanels.EnclosureWidth, Quotation.QuotationsPanels.EnclosureDepth, Quotation.QuotationsPanels.EnclosureIP, Quotation.QuotationsPanels.EnclosureColor,  " +
                      "Quotation.QuotationsPanels.EnclosureMetalType, Quotation.QuotationsPanels.EnclosureForm, Quotation.QuotationsPanels.EnclosureDoor, Quotation.QuotationsPanels.EnclosureFunctional, Quotation.QuotationsPanels.Source,  " +
                      "Quotation.QuotationsPanels.Icu, Quotation.QuotationsPanels.Frequency, Quotation.QuotationsPanels.PowerSupplyOperation, Quotation.QuotationsPanels.EarthingSystem, Quotation.QuotationsPanels.DirectCurrent,  " +
                      "Quotation.QuotationsPanels.Busbar, Quotation.QuotationsPanels.BusbarHorizontal, Quotation.QuotationsPanels.BusbarVertical, Quotation.QuotationsPanels.NeutralSize, Quotation.QuotationsPanels.EarthSize,  " +
                      "Quotation.QuotationsPanels.SignallingVoltage, Quotation.QuotationsPanels.SignallingSource, Quotation.QuotationsPanels.ControlVoltage, Quotation.QuotationsPanels.ControlSource, Quotation.QuotationsPanels.SensorsVoltage,  " +
                      "Quotation.QuotationsPanels.SensorsSource, Quotation.QuotationsPanels.LightingVoltage, Quotation.QuotationsPanels.LightingSource, Quotation.QuotationsPanels.ClimateType, Quotation.QuotationsPanels.AtmosphereType,  " +
                      "Quotation.QuotationsPanels.PollutionRisks, Quotation.QuotationsPanels.AmbientTemperature, Quotation.QuotationsPanels.RelativeHumidity, Quotation.QuotationsPanels.SeaLevel, Quotation.QuotationsPanels.IndicationType,  " +
                      "Quotation.QuotationsPanels.IndicationSize, Quotation.QuotationsPanels.IndicationOther, Quotation.QuotationsPanels.MeterRelay, Quotation.QuotationsPanels.IncomingFixed, Quotation.QuotationsPanels.IncomingPlugIn,  " +
                      "Quotation.QuotationsPanels.IncomingDrawout, Quotation.QuotationsPanels.IncomingMotorized, Quotation.QuotationsPanels.IncomingBehindDoor, Quotation.QuotationsPanels.IncomingThroughPlate,  " +
                      "Quotation.QuotationsPanels.IncomingInterLock, Quotation.QuotationsPanels.IncomingPadlocking, Quotation.QuotationsPanels.IncomingShutter, Quotation.QuotationsPanels.IncomingThroughDoor,  " +
                      "Quotation.QuotationsPanels.IncomingThroughCover, Quotation.QuotationsPanels.IncomingDirect, Quotation.QuotationsPanels.IncomingTerminalBlocks, Quotation.QuotationsPanels.IncomingBusbarLinks,  " +
                      "Quotation.QuotationsPanels.IncomingFront, Quotation.QuotationsPanels.IncomingRear, Quotation.QuotationsPanels.IncomingLeftRight, Quotation.QuotationsPanels.IncomingTopCables,  " +
                      "Quotation.QuotationsPanels.IncomingTopBusduct, Quotation.QuotationsPanels.IncomingBottomCables, Quotation.QuotationsPanels.IncomingScrews, Quotation.QuotationsPanels.IncomingGlandPlate,  " +
                      "Quotation.QuotationsPanels.IncomingCableGland, Quotation.QuotationsPanels.IncomingShrouding, Quotation.QuotationsPanels.OutgoingFixed, Quotation.QuotationsPanels.OutgoingPlugIn,  " +
                      "Quotation.QuotationsPanels.OutgoingDrawout, Quotation.QuotationsPanels.OutgoingMotorized, Quotation.QuotationsPanels.OutgoingBehindDoor, Quotation.QuotationsPanels.OutgoingThroughPlate,  " +
                      "Quotation.QuotationsPanels.OutgoingInterLock, Quotation.QuotationsPanels.OutgoingPadlocking, Quotation.QuotationsPanels.OutgoingShutter, Quotation.QuotationsPanels.OutgoingThroughDoor,  " +
                      "Quotation.QuotationsPanels.OutgoingThroughCover, Quotation.QuotationsPanels.OutgoingDirect, Quotation.QuotationsPanels.OutgoingTerminalBlocks, Quotation.QuotationsPanels.OutgoingBusbarLinks,  " +
                      "Quotation.QuotationsPanels.OutgoingFront, Quotation.QuotationsPanels.OutgoingRear, Quotation.QuotationsPanels.OutgoingLeftRight, Quotation.QuotationsPanels.OutgoingTopCables,  " +
                      "Quotation.QuotationsPanels.OutgoingTopBusduct, Quotation.QuotationsPanels.OutgoingBottomCables, Quotation.QuotationsPanels.OutgoingScrews, Quotation.QuotationsPanels.OutgoingGlandPlate,  " +
                      "Quotation.QuotationsPanels.OutgoingCableGland, Quotation.QuotationsPanels.OutgoingShrouding, Quotation.QuotationsPanels.PushButtonON, Quotation.QuotationsPanels.PushButtonOFF,  " +
                      "Quotation.QuotationsPanels.PushButtonReset, Quotation.QuotationsPanels.SignallingON, Quotation.QuotationsPanels.SignallingOFF, Quotation.QuotationsPanels.SignallingTrip, Quotation.QuotationsPanels.ExternalLabelType,  " +
                      "Quotation.QuotationsPanels.ExternalLabelFixeing, Quotation.QuotationsPanels.ExternalLabelLanguage, Quotation.QuotationsPanels.InternalLabelType, Quotation.QuotationsPanels.InternalLabelFixeing,  " +
                      "Quotation.QuotationsPanels.InternalLabelLanguage, Quotation.QuotationsPanels.EquipmentLabelType, Quotation.QuotationsPanels.EquipmentLabelFixeing, Quotation.QuotationsPanels.EquipmentLabelLanguage,  " +
                      "Quotation.QuotationsPanels.LabelBackground, Quotation.QuotationsPanels.LabelFont, Quotation.QuotationsPanels.AuxiliaryVoltageSection, Quotation.QuotationsPanels.AuxiliaryVoltageColor,  " +
                      "Quotation.QuotationsPanels.AuxiliaryVoltageType, Quotation.QuotationsPanels.AuxiliaryCurrentSection, Quotation.QuotationsPanels.AuxiliaryCurrentColor, Quotation.QuotationsPanels.AuxiliaryCurrentType,  " +
                      "Quotation.QuotationsPanels.ApparatusDefind, Quotation.QuotationsPanels.Weight, Quotation.QuotationsPanels.ForInformation, Quotation.QuotationsPanels.ForProduction, Quotation.QuotationsPanels.ForApproval,  " +
                      "Quotation.QuotationsPanels.AsManufactured, Quotation.QuotationsPanels.Remarks, Quotation.QuotationsPanels.IsSpecial " +
                      "FROM Quotation.QuotationsPanels LEFT OUTER JOIN " +
                      "Quotation.QuotationsPanelsCost ON Quotation.QuotationsPanels.PanelID = Quotation.QuotationsPanelsCost.PanelID";

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


