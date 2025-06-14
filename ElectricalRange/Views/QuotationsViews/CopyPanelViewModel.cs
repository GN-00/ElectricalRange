using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class CopyPanelViewModel : ViewModelBase
    {
        public CopyPanelViewModel(QPanel panel, ObservableCollection<QPanel> panels)
        {
            PanelData = panel;
            PanelsData = panels;
            NewData.Update(PanelData);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public QPanel NewData { get; set; } = new QPanel();
        public QPanel PanelData { get; set; }
        public ObservableCollection<QPanel> PanelsData { get; private set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            QPanel checkName = PanelsData.Where(p => p.PanelName == NewData.PanelName).FirstOrDefault();

            if (checkName != null)
            {
                _ = MessageView.Show("Name Error", $"Panel name is already exist!\nPanel SN ({checkName.PanelSN})", MessageViewButton.OK, MessageViewImage.Warning);
                
                return;
            }

            NewData.PanelSN = PanelsData.Max(p => p.PanelSN) + 1;

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Insert(NewData);

                List<QItem> items = QItemController.PanelItems(connection, PanelData.PanelID);

                if (items.Count != 0)
                {
                    string insert = $"Insert Into [Quotation].[QuotationsPanelsItems] " +
                                    $"(PanelID, Article1, Article2, Category, Code, Description, Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort) " +
                                    $"Values " +
                                    $"({NewData.PanelID}, @Article1, @Article2, @Category, @Code, @Description, @Unit, @ItemQty, @Brand, @Remarks, @ItemCost, @ItemDiscount, @ItemTable, @ItemType, @ItemSort)";
                                    
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