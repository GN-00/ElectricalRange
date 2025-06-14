using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;

using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.JobOrdersViews
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

        public PanelViewModel(JobOrder order, JPanel panel)
        {
            UserData = Navigation.UserData;
            PanelData = panel;
            JobOrderData = order;
            //NewData.Update(PanelData);

            GetData();

            IsGeneral = true;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public User UserData { get; }
        public JPanel PanelData { get; }
        public JPanelDetails NewData { get; set; }
        public JobOrder JobOrderData { get; }

        public bool IsEditable
        {
            get
            {
                if (!UserData.ModifyJobOrders)
                    return false;

                if (NewData.Status == Statuses.Production.ToString())
                    return false;
                else if (NewData.Status == Statuses.Closed.ToString())
                    return false;
                else if (NewData.Status == Statuses.Delivered.ToString())
                    return false;
                else if (NewData.Status == Statuses.Hold.ToString())
                    return false;
                else if (NewData.Status == Statuses.Canceled.ToString())
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
            string query = $"Select * From [JobOrder].[Panels] " +
                           $"Where PanelID = {PanelData.PanelID} ";
            NewData = connection.QueryFirstOrDefault<JPanelDetails>(query);

            Enclosures = connection.Query<string>("Select EnclosureType From [JobOrder].[Panels] Where EnclosureType Is Not Null Group By EnclosureType");
            EnclosureMetalTypes = connection.Query<string>("Select EnclosureMetalType From [JobOrder].[Panels] Where EnclosureMetalType Is Not Null Group By EnclosureMetalType ");
            EnclosureColors = connection.Query<string>("Select EnclosureColor From [JobOrder].[Panels] Where EnclosureColor Is Not Null Group By EnclosureColor");
            EnclosureIPs = connection.Query<string>("Select EnclosureIP From [JobOrder].[Panels] Where EnclosureIP Is Not Null Group By EnclosureIP");
            EnclosureForms = connection.Query<string>("Select EnclosureForm From [JobOrder].[Panels] Where EnclosureForm Is Not Null Group By EnclosureForm ");
            EnclosureFunctionals = connection.Query<string>("Select EnclosureFunctional From [JobOrder].[Panels] Where EnclosureFunctional Is Not Null Group By EnclosureFunctional");
            EnclosureDoors = connection.Query<string>("Select EnclosureDoor From [JobOrder].[Panels] Where EnclosureDoor Is Not Null Group By EnclosureDoor");
            Sources = connection.Query<string>("Select Source From [JobOrder].[Panels] Where Source Is Not Null Group By Source");
        }

        private void Save()
        {
            string query;
            JPanel checkName;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [JobOrder].[Panels] " +
                        $"Where PanelName = @PanelName " +
                        $"And PanelID <> {NewData.PanelID} " +
                        $"And JobOrderID = '{NewData.JobOrderID}'";

                checkName = connection.QueryFirstOrDefault<JPanel>(query, NewData);
            }

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
            if (string.IsNullOrWhiteSpace(NewData.PanelType)) { message += $"\n Panel Type."; isReady = false; }

            if (isReady == true)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Update(NewData);

                    query = $"Select * From [JobOrder].[Panels(View)] " +
                            $"Where PanelID = {PanelData.PanelID} ";
                    var updateData = connection.QueryFirstOrDefault<JPanel>(query);

                    PanelData.Update(updateData);
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
            if (!UserData.ModifyJobOrders)
                return false;

            if (NewData.Status == Statuses.Production.ToString())
                return false;
            else if (NewData.Status == Statuses.Closed.ToString())
                return false;
            else if (NewData.Status == Statuses.Delivered.ToString())
                return false;
            else if (NewData.Status == Statuses.Hold.ToString())
                return false;
            else if (NewData.Status == Statuses.Canceled.ToString())
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