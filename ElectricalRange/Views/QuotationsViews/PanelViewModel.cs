using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class PanelViewModel : ViewModelBase
    {
        private string _Title;

        private bool _IsGeneral;
        private bool _IsEnclosure;
        private bool _IsPower;
        private bool _IsAuxiliary;
        private bool _IsInstruments;
        private bool _IsApparatus;
        private bool _IsConnections;
        private bool _IsCircuitMarking;
        private bool _IsClimaticConditions;
        private bool _IsParticularNote;

        public PanelViewModel(Quotation quotation, QPanel panel, ObservableCollection<QPanel> panels)
        {
            UserData = Navigation.UserData;
            PanelData = panel;
            PanelsData = panels;
            QuotationData = quotation;
            NewData.Update(PanelData);

            GetData();

            IsGeneral = true;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public User UserData { get; }
        public QPanel PanelData { get; }
        public QPanel NewData { get; } = new QPanel();
        public ObservableCollection<QPanel> PanelsData { get; }
        public Quotation QuotationData { get; }

        public bool IsEditable
        {
            get
            {
                if (UserData.EmployeeId != QuotationData.EstimationID)
                    return false;

                if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                    return false;

                return true;
            }
        }

        public string Title
        {
            get => _Title;
            set => SetValue(ref _Title, value);
        }


        public bool IsGeneral
        {
            get => _IsGeneral;
            set
            {
                if (SetValue(ref _IsGeneral, value))
                {
                    if (value)
                    {
                        Title = "General Information";
                    }
                }
            }
        }
        public bool IsEnclosure
        {
            get => _IsEnclosure;
            set
            {
                if (SetValue(ref _IsEnclosure, value))
                {
                    if (value)
                    {
                        Title = "Enclosure Information";
                    }
                }
            }
        }
        public bool IsPower
        {
            get => _IsPower;
            set
            {
                if (SetValue(ref _IsPower, value))
                {
                    if (value)
                    {
                        Title = "Power & Busbar";
                    }
                }
            }
        }
        public bool IsAuxiliary
        {
            get => _IsAuxiliary;
            set
            {
                if (SetValue(ref _IsAuxiliary, value))
                {
                    if (value)
                    {
                        Title = "Auxiliary";
                    }
                }
            }
        }
        public bool IsInstruments
        {
            get => _IsInstruments;
            set
            {
                if (SetValue(ref _IsInstruments, value))
                {
                    if (value)
                    {
                        Title = "Instruments";
                    }
                }
            }
        }
        public bool IsApparatus
        {
            get => _IsApparatus;
            set
            {
                if (SetValue(ref _IsApparatus, value))
                {
                    if (value)
                    {
                        Title = "Apparatus";
                    }
                }
            }
        }
        public bool IsConnections
        {
            get => _IsConnections;
            set
            {
                if (SetValue(ref _IsConnections, value))
                {
                    if (value)
                    {
                        Title = "Connections";
                    }
                }
            }
        }
        public bool IsCircuitMarking
        {
            get => _IsCircuitMarking;
            set
            {
                if (SetValue(ref _IsCircuitMarking, value))
                {
                    if (value)
                    {
                        Title = "Circuit Marking";
                    }
                }
            }
        }
        public bool IsClimaticConditions
        {
            get => _IsClimaticConditions;
            set
            {
                if (SetValue(ref _IsClimaticConditions, value))
                {
                    if (value)
                    {
                        Title = "Climatic Conditions";
                    }
                }
            }
        }
        public bool IsParticularNote
        {
            get => _IsParticularNote;
            set
            {
                if (SetValue(ref _IsParticularNote, value))
                {
                    if (value)
                    {
                        Title = "Particular Note";
                    }
                }
            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public IEnumerable<string> Enclosures { get; private set; }
        public IEnumerable<string> EnclosureMetalTypes { get; private set; }
        public IEnumerable<string> EnclosureColors { get; private set; }
        public IEnumerable<string> EnclosureIPs { get; private set; }
        public IEnumerable<string> EnclosureForms { get; private set; }
        public IEnumerable<string> EnclosureFunctionals { get; private set; }
        public IEnumerable<string> EnclosureDoors { get; private set; }
        public IEnumerable<string> Sources { get; private set; }

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            Enclosures = connection.Query<string>("Select EnclosureType From [Quotation].[QuotationsPanels] Where EnclosureType Is Not Null Group By EnclosureType");
            EnclosureMetalTypes = connection.Query<string>("Select EnclosureMetalType From [Quotation].[QuotationsPanels] Where EnclosureMetalType Is Not Null Group By EnclosureMetalType ");
            EnclosureColors = connection.Query<string>("Select EnclosureColor From [Quotation].[QuotationsPanels] Where EnclosureColor Is Not Null Group By EnclosureColor");
            EnclosureIPs = connection.Query<string>("Select EnclosureIP From [Quotation].[QuotationsPanels] Where EnclosureIP Is Not Null Group By EnclosureIP");
            EnclosureForms = connection.Query<string>("Select EnclosureForm From [Quotation].[QuotationsPanels] Where EnclosureForm Is Not Null Group By EnclosureForm ");
            EnclosureFunctionals = connection.Query<string>("Select EnclosureFunctional From [Quotation].[QuotationsPanels] Where EnclosureFunctional Is Not Null Group By EnclosureFunctional");
            EnclosureDoors = connection.Query<string>("Select EnclosureDoor From [Quotation].[QuotationsPanels] Where EnclosureDoor Is Not Null Group By EnclosureDoor");
            Sources = connection.Query<string>("Select Source From [Quotation].[QuotationsPanels] Where Source Is Not Null Group By Source");
        }

        private void Save()
        {
            QPanel checkName;
            checkName = PanelsData.FirstOrDefault(p => p.PanelName == NewData.PanelName && p.PanelID != NewData.PanelID);

            if (checkName != null)
            {
                _ = MessageView.Show($"Name Error",
                                     $"Panel name is already exist!\nPanel SN ({checkName.PanelSN})",
                                     MessageViewButton.OK,
                                     MessageViewImage.Warning);
                return;
            }

            bool isReady = true;
            string message = "Please Enter:";
            if (string.IsNullOrWhiteSpace(NewData.PanelName)) { message += $"\n Panel Name."; isReady = false; }
            if (NewData.PanelQty == 0) { message += $"\n Panel Qty."; isReady = false; }
            if (string.IsNullOrWhiteSpace(NewData.PanelType)) { message += $"\n Panel Type."; isReady = false; }

            if (isReady == true)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    if (NewData.PanelID != 0)
                    {
                        _ = connection.Update(NewData);
                        PanelData.Update(NewData);
                    }
                    else
                    {
                        if (NewData.PanelSN == PanelsData.Count + 1) //Add
                        {
                            PanelsData.Add(NewData);
                        }
                        else //Insert
                        {
                            string query = $"Update [Quotation].[QuotationsPanels] " +
                                           $"Set PanelSN = PanelSN + 1 " +
                                           $"Where PanelSN >= {NewData.PanelSN} " +
                                           $"And QuotationID ={NewData.QuotationID}";

                            _ = connection.Execute(query);

                            foreach (QPanel panel in PanelsData.Where(panel => panel.PanelSN >= NewData.PanelSN))
                            {
                                panel.PanelSN++;
                            }

                            PanelsData.Insert(NewData.PanelSN.Value - 1, NewData);
                        }


                        _ = connection.Insert(NewData);
                    }
                }

                Navigation.Back();
            }
            else
            {
                _ = MessageView.Show("Missing Data", message, MessageViewButton.OK, MessageViewImage.Warning);
            }
        }
        private bool CanSave()
        {
            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void Cancel()
        {
            Navigation.Back();
        }
        private bool CanCancel()
        {
            return true;
        }
    }
}