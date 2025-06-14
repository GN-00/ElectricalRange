using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.JobOrders;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing
{
    public partial class DeliveryForm : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public List<DPanel> PanelsData { get; set; }
        public DeliveryInfromation DeliveryInfromation { get; set; }

        public DeliveryForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            BackgroundImage.Source = AppData.CompanyWatermark;

            for (int i = PanelsData.Count + 1; i <= 16; i++)
            {
                DeliveryTable.RowDefinitions[i].Height = new GridLength(0);
            }

            if (Page == Pages)
                SignatureArea.Visibility = Visibility.Visible;
            else
                SignatureArea.Visibility = Visibility.Collapsed;

            DataContext = new { PanelsData, DeliveryInfromation, Page, Pages };
        }

    }
}
