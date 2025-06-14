using Dapper.Contrib.Extensions;
using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Windows.MessageWindows;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class DownloadPanelViewModel : ViewModelBase
    {
        //Download From Library
        public DownloadPanelViewModel(Quotation quotation, QPanel panel, ObservableCollection<QPanel> panels)
        {
            QuotationData = quotation;
            PanelData = panel;
            PanelsData = panels;
            NewData.Update(PanelData);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }
        public Quotation QuotationData { get; set; }
        public QPanel PanelData { get; set; }
        public QPanel NewData { get; set; } = new QPanel();
        public ObservableCollection<QPanel> PanelsData { get; set; }


        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            QPanel checkName = PanelsData.Where(p => p.PanelName == NewData.PanelName).FirstOrDefault();

            if (checkName != null)
            {
                _ = MessageWindow.Show("Name Error", $"Panel name is already exist!\nPanel SN ({checkName.PanelSN})", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }

            NewData.PanelSN = PanelsData.Max(p => p.PanelSN) + 1;
            NewData.QuotationID = QuotationData.QuotationID;

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Insert(NewData);

                List<QItem> items = QItemController.PanelRecalculateItems(connection, PanelData.PanelID);

                if (items.Count != 0)
                {
                    string insert = $"Insert Into [Quotation].[QuotationsPanelsItems] " +
                                    $"(PanelID, Article1, Article2, Category, Code, Description, Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort) " +
                                    $"Values " +
                                    $"({NewData.PanelID}, @Article1, @Article2, @Category, @Code, @Description, @Unit, @ItemQty, @Brand, @Remarks, @ReferenceCost, @ReferenceDiscount, @ItemTable, @ItemType, @ItemSort)";
                    _ = connection.Execute(insert, items);
                }
            }

            PanelsData.Add(NewData);

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (NewData == null)
                return false;

            if (string.IsNullOrWhiteSpace(NewData.PanelName))
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