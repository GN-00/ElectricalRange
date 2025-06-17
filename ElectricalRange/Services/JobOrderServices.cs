using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.AttachedProperties;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Printing;
using ProjectsNow.Printing.JobOrderPages;
using ProjectsNow.Views;
using ProjectsNow.Views.JobOrdersViews;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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


        private static Grid Table { get; set; }
        private static Border NameBorder { get; set; }
        private static TextBlock EnglishName { get; set; }
        static void UpdateUI()
        {
            List<UIElement> elements = new()
            {
                EnglishName,
                NameBorder,
                Table,
            };

            foreach (UIElement element in elements)
            {
                if (element == null)
                    continue;

                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                element.Arrange(new Rect(element.DesiredSize));
            }
        }
        private static void NewTable()
        {
            Table = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Table.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
            Table.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
        }
        private static void NewCell(ProductionPanel panel)
        {
            EnglishName = new TextBlock()
            {
                Text = panel.PanelName,
                FontFamily = new FontFamily("Times New Roman"),
                //FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            NameBorder = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = EnglishName,
            };

            Grid.SetRow(NameBorder, Table.RowDefinitions.Count);
            Table.RowDefinitions.Add(new RowDefinition() { MinHeight = 42 });
            Table.Children.Add(NameBorder);
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


            int page = 1;
            double maxHeight = 650;
            List<List<ProductionPanel>> pagePanels = new() { new List<ProductionPanel>() };

            int pagesNumber;

            NewTable();
            foreach (ProductionPanel panel in panels)
            {
                NewCell(panel);
                UpdateUI();

                var lines = EnglishName.GetLines().ToList();
                bool isMultiLine = lines.Count > 1;

Panel:
                int nameLines = lines.Count;
                if (nameLines >= 1 && isMultiLine)
                {
                    EnglishName.Text = "";
                    UpdateUI();

                    while (Table.ActualHeight + 25 < maxHeight)
                    {
                        if (lines.Count == 0)
                            break;

                        if (EnglishName.Text == "")
                            EnglishName.Text += lines[0];
                        else
                            EnglishName.Text += $"\n{lines[0]}";

                        UpdateUI();
                        lines.Remove(lines[0]);
                    }
                }


                if (Table.ActualHeight > maxHeight)
                {
                    ProductionPanel newPanel = new();
                    newPanel.Update(panel);

                    if (!isMultiLine)
                    {
                        pagePanels[page - 1].Add(newPanel);

                        page++;
                        pagePanels.Add(new List<ProductionPanel>());
                        NewTable();
                        NewCell(panel);
                    }
                    else
                    {
                        newPanel.PanelName = EnglishName.Text;
                        pagePanels[page - 1].Add(newPanel);

                        page++;
                        pagePanels.Add(new List<ProductionPanel>());
                        NewTable();
                        NewCell(panel);

                        if (lines.Count != 0)
                        {
                            goto Panel;
                        }
                    }
                }
                else
                {
                    ProductionPanel newPanel = new();
                    newPanel.Update(panel);
                    pagePanels[page - 1].Add(newPanel);

                    if (isMultiLine)
                    {
                        newPanel.PanelName = EnglishName.Text;
                    }
                }

                if (pagePanels[page - 1].Count > 20)
                {
                    page++;
                    pagePanels.Add(new List<ProductionPanel>());
                    NewTable();
                }

            }

            if (panels.Count != 0)
            {
                pagesNumber = pagePanels.Count;
                List<FrameworkElement> elements = new();
                for (int i = 1; i <= pagesNumber; i++)
                {
                    JobFile requestForm = new()
                    {
                        Page = i,
                        Pages = Convert.ToInt32(pagesNumber),
                        RequestData = requestInfromation,
                        PanelsData = pagePanels[i - 1]
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
