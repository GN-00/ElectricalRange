using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Printing;
using ProjectsNow.Printing.ProductionPages;
using ProjectsNow.Views;
using ProjectsNow.Windows.MessageWindows;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Windows;

namespace ProjectsNow.Services
{
    public static class ProductionServices
    {
        public static void PrintCloseRequest(CloseRequest request, IView view)
        {
            string query;
            List<ProductionPanel> panels;
            CloseRequestInfo requestInfo;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Production].[CloseRequestsInfo] " +
                    $"Where JobOrderId = {request.JobOrderId} " +
                    $"And Number = '{request.Number}'";
            requestInfo = connection.QueryFirstOrDefault<CloseRequestInfo>(query);

            query = $"Select * From [Production].[Panels(Closed)] " +
                    $"Where JobOrderId = {request.JobOrderId} " +
                    $"And Reference = '{request.Number}' " +
                    $"Order By SN";
            panels = [.. connection.Query<ProductionPanel>(query)];

            double pagesNumber = panels.Count / 10d;
            if (pagesNumber - Math.Truncate(pagesNumber) != 0)
                pagesNumber = Math.Truncate(pagesNumber) + 1;

            foreach (var panel in panels)
                panel.SN = panels.IndexOf(panel) + 1;

            if (pagesNumber != 0)
            {
                requestInfo.Pages = (int)pagesNumber;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    requestInfo.Page = i;
                    requestInfo.Panels = [.. panels.Where(p => p.SN > ((i - 1) * 10) && p.SN <= (i * 10))];
                    CloseForm requestForm = new(requestInfo);

                    elements.Add(requestForm);
                }

                Print.PrintPreview(elements, $"P.I.F {requestInfo.JobOrderCode}-{requestInfo.Number}", view);
            }
            else
            {
                _ = MessageView.Show("Items",
                                     "There is no panels!!",
                                     MessageViewButton.OK,
                                     MessageViewImage.Warning);
            }
        }

        public static List<Item> AddItems(ProductionPanel panel) 
        {
            Navigation.OpenLoading(Visibility.Visible, "Working....");

            OpenFileDialog path = new() { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm" };
            _ = path.ShowDialog();

            string filePath = $@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={path.FileName};" +
                              $@"Extended Properties='Excel 8.0;HDR=Yes;'";

            try
            {
                DataTable excelData = new();
                using (OleDbConnection connection = new(filePath))
                {
                    connection.Open();
                    OleDbDataAdapter oleAdpt = new("select Code, Description, Qty from [Sheet1$]", connection); //here we read data from sheet1  
                    _ = oleAdpt.Fill(excelData);
                }

                if (excelData.Rows.Count == 0)
                {
                    _ = MessageWindow.Show("Data Error", "No data!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    Navigation.CloseLoading();
                    return null;
                }

                List<Item> excelList = new();
                for (int i = 0; i < excelData.Rows.Count; i++)
                {
                    Item excelRow = new();
                    excelRow.PanelId = panel.PanelId;
                    excelRow.JobOrderId = panel.JobOrderId;
                    excelRow.Code = excelData.Rows[i]["Code"].ToString();
                    excelRow.Description = excelData.Rows[i]["Description"].ToString();
                    excelRow.Qty = Convert.ToDouble(excelData.Rows[i]["Qty"]);
                    excelRow.Type = "Base";
                    excelList.Add(excelRow);
                }

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"DELETE FROM [Production].[PanelsItems] WHERE PanelId = {panel.PanelId} AND JobOrderId = {panel.JobOrderId}";
                    _ = connection.Execute(query);
                    _ = connection.Insert(excelList);
                }

                panel.Items = excelList.Sum(x => x.Qty);

                _ = MessageWindow.Show("Data", $"Items updated!", MessageWindowButton.OK, MessageWindowImage.Information);
                Navigation.CloseLoading();
                return excelList;

            }
            catch (Exception exception)
            {
                _ = MessageWindow.Show("Error", exception.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
                Navigation.CloseLoading();
                return null;
            }
        }
    }
}
