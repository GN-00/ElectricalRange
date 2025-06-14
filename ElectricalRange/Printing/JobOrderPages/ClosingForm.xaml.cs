using ProjectsNow.Data.JobOrders;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.JobOrderPages
{
    public partial class ClosingForm : UserControl
    {
        public double Page { get; set; }
        public double Pages { get; set; }
        public List<ClosingPanel> PanelsData { get; set; }
        public ClosingRequestInformation RequestData { get; set; }

        public ClosingForm()
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

            for (int i = 0; i < PanelsData.Count; i++)
            {
                PanelsTable.RowDefinitions[i + 1].Height = new GridLength(25);
            }

            DataContext = new { RequestData, PanelsData, Page, Pages };
        }
    }
}
