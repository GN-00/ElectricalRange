using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class QPanelController
    {
        public static ObservableCollection<QPanel> QuotationPanels(SqlConnection connection, int quotationID)
        {
            string query = $"Select * " +
                           $"From [Quotation].[QuotationsPanels(View)] " +
                           $"WHERE QuotationID = {quotationID} Order By PanelSN";

            ObservableCollection<QPanel> panels = new(connection.Query<QPanel>(query));
            return panels;
        }

        public static void UpdateEnclosure(SqlConnection connection, QPanel panelData, Group group, List<List<string>> values, ObservableCollection<QItem> Items)
        {
            if (group.Name == "NSYCRN")
            {
                panelData.EnclosureType = "Universal or Equivalent";
                panelData.EnclosureHeight = decimal.Parse(values[1][1]);
                panelData.EnclosureWidth = decimal.Parse(values[1][2]);
                panelData.EnclosureDepth = decimal.Parse(values[1][3]);

                panelData.EnclosureMetalType = "Steel";
                panelData.EnclosureColor = "7035";
                panelData.EnclosureIP = values[1][4];
                panelData.EnclosureForm = "1";
                panelData.EnclosureLocation = values[1][5] == "With." ? "Outdoor" : "Indoor";
                panelData.EnclosureInstallation = "Wall Mounted";
                panelData.EnclosureFunctional = values[1][6];

                panelData.EnclosureName = $"Universal NSYCRN{panelData.EnclosureHeight}{panelData.Weight}/{panelData.EnclosureDepth} IP{panelData.EnclosureIP} {panelData.EnclosureLocation}";
                _ = connection.Update(panelData);
            }
            else if (group.Name == "NSYSM")
            {
                panelData.EnclosureType = "Universal or Equivalent";
                panelData.EnclosureHeight = decimal.Parse(values[1][1]);
                panelData.EnclosureWidth = decimal.Parse(values[1][2]);
                panelData.EnclosureDepth = decimal.Parse(values[1][3]);

                panelData.EnclosureMetalType = "Steel";
                panelData.EnclosureColor = "7035";
                panelData.EnclosureIP = values[1][4];
                panelData.EnclosureForm = "1";
                panelData.EnclosureLocation = values[1][5] == "With." ? "Outdoor" : "Indoor";
                panelData.EnclosureInstallation = "Floor Standing";
                panelData.EnclosureFunctional = values[1][6];

                panelData.EnclosureName = $"Universal NSYSM{panelData.EnclosureHeight}{panelData.Weight}/{panelData.EnclosureDepth} IP{panelData.EnclosureIP} {panelData.EnclosureLocation}";
                _ = connection.Update(panelData);
            }
            else if (group.Name == "Disbo")
            {
                panelData.EnclosureType = $"Acti9 Disbo {values[1][4]} {values[1][3]}";
                panelData.EnclosureHeight = decimal.Parse(values[2][1]);
                panelData.EnclosureWidth = decimal.Parse(values[2][2]);
                panelData.EnclosureDepth = decimal.Parse(values[2][3]);

                panelData.EnclosureMetalType = "Steel";
                panelData.EnclosureColor = "9002";
                panelData.EnclosureIP = values[2][4];
                panelData.EnclosureForm = "1";
                panelData.EnclosureLocation = "Indoor";
                panelData.EnclosureInstallation = $"{values[1][2]} Ways";
                panelData.EnclosureFunctional = "With";

                panelData.EnclosureName = $"Acti9 Disbo {values[1][2]} Ways {values[1][5]} {values[1][6]}A";
                _ = connection.Update(panelData);
            }
        }

        public static ObservableCollection<QPanel> GetQuotationPanelsWaitingPurcheaseOrder(this SqlConnection connection, int quotationID)
        {
            string query = $"Select * From [Quotation].[Panels(View)]" +
                           $"WHERE QuotationID = {quotationID} And PurchaseOrdersNumber Is Null " +
                           $"Order By PanelSN";

            ObservableCollection<QPanel> records = new(connection.Query<QPanel>(query));
            return records;
        }

        public static void UpdateEnclosure(QPanel panelData, Data.Library.Group group)
        {
            int i;
            int j;
            string value;

            panelData.EnclosureType = group.Description;

            // Get Height
            var checkHeight = group.Categories[0].Properties.FirstOrDefault(x => x.Id == "Height");
            if (checkHeight != null)
            {
                i = checkHeight.i;
                j = checkHeight.j;
                value = (string)group.SelectionData.GetType().GetProperty($"Property{i}{j}").GetValue(group.SelectionData);
                value = value.Replace("H", "");
                panelData.EnclosureHeight = decimal.Parse(value);
            }


            // Get Width
            var checkWidth = group.Categories[0].Properties.FirstOrDefault(x => x.Id == "Width");
            if (checkWidth != null)
            {
                i = checkWidth.i;
                j = checkWidth.j;
                value = (string)group.SelectionData.GetType().GetProperty($"Property{i}{j}").GetValue(group.SelectionData);
                value = value.Replace("W", "");
                panelData.EnclosureWidth = decimal.Parse(value);
            }


            // Get Depth
            var checkDepth = group.Categories[0].Properties.FirstOrDefault(x => x.Id == "Depth");
            if (checkDepth != null)
            {
                i = checkDepth.i;
                j = checkDepth.j;
                value = (string)group.SelectionData.GetType().GetProperty($"Property{i}{j}").GetValue(group.SelectionData);
                value = value.Replace("D", "");
                panelData.EnclosureDepth = decimal.Parse(value);
            }


            // Get IP
            var checkIP = group.Categories[0].Properties.FirstOrDefault(x => x.Id == "IP");
            if (checkIP != null)
            {
                i = checkIP.i;
                j = checkIP.j;
                value = (string)group.SelectionData.GetType().GetProperty($"Property{i}{j}").GetValue(group.SelectionData);
                panelData.EnclosureIP = value;
            }


            //Get Installation
            var checkInstallation = group.Categories[0].Properties.FirstOrDefault(x => x.Id == "Type");
            if (checkInstallation != null)
            {
                i = checkInstallation.i;
                j = checkInstallation.j;
                value = (string)group.SelectionData.GetType().GetProperty($"Property{i}{j}").GetValue(group.SelectionData);
                panelData.EnclosureInstallation = value;
            }

            panelData.EnclosureName = $"{panelData.EnclosureType} {panelData.EnclosureHeight}{panelData.Weight}/{panelData.EnclosureDepth} IP{panelData.EnclosureIP} {panelData.EnclosureLocation}";
            SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(panelData);

        }
    }
}