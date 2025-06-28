using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Printing;
using ProjectsNow.Printing.ProductionPages;
using ProjectsNow.Views;
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
    }
}
