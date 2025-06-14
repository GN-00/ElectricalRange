using System.Windows;
using ProjectsNow.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using ProjectsNow.Data.Application;

namespace ProjectsNow.Printing
{
    public partial class RequestforShopDrawingApprovalControl : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public List<APanel> PanelsData { get; set; }
        public ApprovalRequestInformation RequestInformation { get; set; }

        public RequestforShopDrawingApprovalControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            BackgroundImage.Source = AppData.CompanyWatermark;

            double rowHeight = 22.6771653543307;
            foreach (RowDefinition row in DeliveryTable.RowDefinitions)
            {
                row.Height = new GridLength(0);
            }

            DeliveryTable.RowDefinitions[0].Height = new GridLength(rowHeight);

            for (int i = 1; i <= PanelsData.Count; i++)
            {
                DeliveryTable.RowDefinitions[i].Height = new GridLength(rowHeight);
            }

            DataContext = new { PanelsData, RequestInformation, Page, Pages };
        }
    }
}
