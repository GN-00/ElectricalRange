using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class SpecialPanelViewModel : ViewModelBase
    {
        public SpecialPanelViewModel(Quotation quotation, QPanel panel, ObservableCollection<QPanel> panels)
        {
            UserData = Navigation.UserData;
            QuotationData = quotation;
            PanelData = panel;
            PanelsData = panels;
            NewData.Update(PanelData);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public User UserData { get; }
        public bool IsEditable
        {
            get
            {
                if (!UserData.ModifyQuotations)
                    return false;

                if (UserData.EmployeeId != QuotationData.EstimationID)
                    return false;

                if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                    return false;

                return true;
            }
        }
        public Quotation QuotationData { get; }

        public QPanel NewData { get; } = new QPanel();
        public QPanel PanelData { get; }
        public ObservableCollection<QPanel> PanelsData { get; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

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

            if (isReady == true)
            {
                NewData.QuotationID = QuotationData.QuotationID;
                NewData.PanelType = null;
                NewData.IsSpecial = true;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    if (NewData.PanelID != 0) //Edit
                    {
                        _ = connection.Update(NewData);
                        PanelData.Update(NewData);
                    }
                    else //Add
                    {
                        PanelsData.Add(NewData);
                        _ = connection.Insert(NewData);
                    }

                    QItem newItemData = new()
                    {
                        PanelID = NewData.PanelID,
                        Code = "SPECIAL ITEM",
                        Description = NewData.PanelNameInfo,
                        ItemQty = 1,
                        ItemCost = NewData.PanelCost,
                        ItemDiscount = 0,
                        ItemSort = 1,
                        ItemTable = "Details",
                        ItemType = "NewItem",
                        Article1 = "SPECIAL",
                        Article2 = null,
                        Unit = "Lot",
                    };

                    _ = connection.Execute($"Delete From [Quotation].[QuotationsPanelsItems] Where PanelID = {NewData.PanelID}");
                    _ = connection.Insert(newItemData);
                }

                Navigation.ClosePopup();
            }
            else
            {
                _ = MessageView.Show("Missing Data", message, MessageViewButton.OK, MessageViewImage.Warning);
            }
        }
        private bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(NewData.PanelName))
                return false;

            if (NewData.PanelQty <= 0)
                return false;

            if (NewData.PanelCost < 0)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void Cancel()
        {
            Navigation.ClosePopup();
        }
        private bool CanCancel()
        {
            return true;
        }
    }
}