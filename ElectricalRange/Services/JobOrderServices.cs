using Dapper;

using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Printing;
using ProjectsNow.Printing.JobOrderPages;
using ProjectsNow.Views;
using ProjectsNow.Views.JobOrdersViews;

using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ProjectsNow.Services
{
    public static class JobOrderServices
    {
        public static void Approvals(JobOrder order, IView view)
        {
            Navigation.To(new ApprovalsView(order), view);
        }
        public static void PrintApprovals(int orderId, ApprovalRequest request, IView view)
        {

            MessageBoxResult result =
                MessageView.Show("Printing",
                                 "Print with watermark?",
                                 MessageViewButton.YesNo,
                                 MessageViewImage.Question);

            string query;
            List<APanel> panels;
            ApprovalRequestInformation requestInfromation;
            List<int> requestsNumbers = new();
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [JobOrder].[ApprovalRequestsInformations] " +
                        $"Where JobOrderID = {orderId} " +
                        $"And RequestNumber = '{request.Number}'";
                requestInfromation = connection.QueryFirstOrDefault<ApprovalRequestInformation>(query);

                query = $"Select * From [JobOrder].[Panels(ApprovalRequestDetails)] " +
                        $"Where JobOrderID = {orderId} " +
                        $"And RequestNumber = '{request.Number}' " +
                        $"Order By PanelSN";
                panels = connection.Query<APanel>(query).ToList();
            }
            requestInfromation.DrawingsNo = panels.Count;

            double pagesNumber = panels.Count / 10d;
            if (pagesNumber - Math.Truncate(pagesNumber) != 0)
            {
                pagesNumber = Math.Truncate(pagesNumber) + 1;
            }

            if (pagesNumber != 0)
            {
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    RequestforShopDrawingApprovalControl requestforShopDrawingApproval = new()
                    {
                        Page = i,
                        Pages = Convert.ToInt32(pagesNumber),
                        RequestInformation = requestInfromation,
                        PanelsData = panels.Where(p => p.PanelSN > ((i - 1) * 10) && p.PanelSN <= (i * 10)).ToList()
                    };
                    if (result == MessageBoxResult.Yes)
                    {
                        requestforShopDrawingApproval.BackgroundImage.Visibility = Visibility.Visible;
                    }

                    elements.Add(requestforShopDrawingApproval);
                }

                Print.PrintPreview(elements, $"Shop Drawing Approval Request {request.Number}", view);
            }
            else
            {
                _ = MessageView.Show("Items",
                                     "There is no panels!!",
                                     MessageViewButton.OK,
                                     MessageViewImage.Warning);
            }
        }

        public static void Production(JobOrder order, IView view)
        {
            Navigation.To(new JobFileRequestsView(order), view);
        }
        internal static void PrintJobFileRequest(int orderId, JobFileRequest request, IView view)
        {
            string query;
            List<ProductionPanel> panels;
            JobFileRequestInformation requestInfromation;
            List<int> requestsNumbers = new();
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [JobOrder].[JobFileRequestsInformations] " +
                        $"Where JobOrderID = {orderId} " +
                        $"And RequestCode = '{request.Number}'";
                requestInfromation = connection.QueryFirstOrDefault<JobFileRequestInformation>(query);

                query = $"Select * From [JobOrder].[Panels(JobFileRequestDetails)] " +
                        $"Where JobOrderID = {orderId} " +
                        $"And RequestNumber = '{request.Number}' " +
                        $"Order By PanelSN";
                panels = connection.Query<ProductionPanel>(query).ToList();
            }

            double pagesNumber = panels.Count / 10d;
            if (pagesNumber - Math.Truncate(pagesNumber) != 0)
            {
                pagesNumber = Math.Truncate(pagesNumber) + 1;
            }

            int order = 1;
            foreach (ProductionPanel panel in panels)
            {
                panel.PanelOrder = order++;
            }

            if (pagesNumber != 0)
            {
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    JobFile requestForm = new()
                    {
                        Page = i,
                        Pages = Convert.ToInt32(pagesNumber),
                        RequestData = requestInfromation,
                        PanelsData = panels.Where(p => p.PanelOrder > ((i - 1) * 10) && p.PanelOrder <= (i * 10)).ToList()
                    };

                    elements.Add(requestForm);
                }

                Print.PrintPreview(elements, $"Shop Drawing Approval Request {request.Number}", view);
            }
            else
            {
                _ = MessageView.Show("Items",
                                     "There is no panels!!",
                                     MessageViewButton.OK,
                                     MessageViewImage.Warning);
            }
        }

        public static void Inspection(JobOrder order, IView view)
        {
            Navigation.To(new InspectionView(order), view);
        }
        internal static void PrintInspectionRequest(int orderId, InspectionRequest request, IView view)
        {
            string query;
            List<InspectionPanel> panels;
            InspectionRequestInformation requestInfromation;
            List<int> requestsNumbers = new();
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [JobOrder].[InspectionRequestsInformations] " +
                        $"Where JobOrderID = {orderId} " +
                        $"And RequestCode = '{request.Number}'";
                requestInfromation = connection.QueryFirstOrDefault<InspectionRequestInformation>(query);

                query = $"Select * From [JobOrder].[Panels(InspectionRequestDetails)] " +
                        $"Where JobOrderID = {orderId} " +
                        $"And RequestNumber = '{request.Number}' " +
                        $"Order By PanelSN";
                panels = connection.Query<InspectionPanel>(query).ToList();
            }

            double pagesNumber = panels.Count / 10d;
            if (pagesNumber - Math.Truncate(pagesNumber) != 0)
            {
                pagesNumber = Math.Truncate(pagesNumber) + 1;
            }

            if (pagesNumber != 0)
            {
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    NotificationForInspection requestForm = new()
                    {
                        Page = i,
                        Pages = Convert.ToInt32(pagesNumber),
                        RequestData = requestInfromation,
                        PanelsData = panels.Where(p => p.PanelSN > ((i - 1) * 10) && p.PanelSN <= (i * 10)).ToList()
                    };

                    elements.Add(requestForm);
                }

                Print.PrintPreview(elements, $"Notification For Inspection {request.Number}", view);
            }
            else
            {
                _ = MessageView.Show("Items",
                                     "There is no panels!!",
                                     MessageViewButton.OK,
                                     MessageViewImage.Warning);
            }
        }

        public static void Closing(JobOrder order, IView view)
        {
            Navigation.To(new ClosingRequestsView(order), view);
        }
        internal static void PrintClosingRequest(int orderId, ClosingRequest request, IView view)
        {
            string query;
            List<ClosingPanel> panels;
            ClosingRequestInformation requestInfromation;
            List<int> requestsNumbers = new();
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [JobOrder].[ClosingRequestsInformations] " +
                        $"Where JobOrderID = {orderId} " +
                        $"And RequestCode = '{request.Number}'";
                requestInfromation = connection.QueryFirstOrDefault<ClosingRequestInformation>(query);

                query = $"Select * From [JobOrder].[Panels(ClosingRequestDetails)] " +
                        $"Where JobOrderID = {orderId} " +
                        $"And RequestNumber = '{request.Number}' " +
                        $"Order By PanelSN";
                panels = connection.Query<ClosingPanel>(query).ToList();
            }

            double pagesNumber = panels.Count / 10d;
            if (pagesNumber - Math.Truncate(pagesNumber) != 0)
            {
                pagesNumber = Math.Truncate(pagesNumber) + 1;
            }

            if (pagesNumber != 0)
            {
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    ClosingForm requestForm = new()
                    {
                        Page = i,
                        Pages = Convert.ToInt32(pagesNumber),
                        RequestData = requestInfromation,
                        PanelsData = panels.Where(p => p.PanelSN > ((i - 1) * 10) && p.PanelSN <= (i * 10)).ToList()
                    };

                    elements.Add(requestForm);
                }

                Print.PrintPreview(elements, $"Shop Drawing Approval Request {request.Number}", view);
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
