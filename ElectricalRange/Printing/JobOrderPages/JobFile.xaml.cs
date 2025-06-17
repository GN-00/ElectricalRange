using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.JobOrderPages
{
    public partial class JobFile : UserControl
    {
        public double Page { get; set; }
        public double Pages { get; set; }
        public List<ProductionPanel> PanelsData { get; set; }
        public JobFileRequestInformation RequestData { get; set; }

        public JobFile()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            if (Page != Pages)
            {
                PagesGrid.Visibility = Visibility.Collapsed;
            }

            if (PanelsData.Count == 0)
                PanelsTable.RowDefinitions[0].Height = new GridLength(1);
            else
                PanelsTable.RowDefinitions[0].Height = new GridLength(30);

            int maxRows = 21;
            for (int i = 1; i < maxRows; i++)
            {
                if (i <= PanelsData.Count)
                    PanelsTable.RowDefinitions[i].Height = new GridLength();
                else
                    PanelsTable.RowDefinitions[i].Height = new GridLength(0);
            }

            DataContext = new { RequestData, PanelsData, Page, Pages };

        }
    }
}
