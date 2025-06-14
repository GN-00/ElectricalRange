using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.DataInput;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.JobOrderPages.JO7059
{
    public partial class InvoiceForm : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public double TotalCost { get; set; }
        public double TotalVAT { get; set; }
        public double TotalPrice { get; set; }
        public double VATPercentage { get; set; }
        public List<InvoiceItem> PanelsData { get; set; }
        public Invoice InvoiceData { get; set; }


        public InvoiceForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            BackgroundImage.Source = AppData.CompanyWatermark;

            for (int i = PanelsData.Count + 1; i <= 14; i++)
            {
                InvoiceTable.RowDefinitions[i].Height = new GridLength(0);
            }

            if (PanelsData.Count == 0)
                InvoiceTable.RowDefinitions[0].Height = new GridLength(1);
            else
                InvoiceTable.RowDefinitions[0].Height = new GridLength(50);

            for (int i = 1; i <= 6; i++)
            {
                ((Grid)FindName($"Total{i}")).Visibility = Visibility.Collapsed;
            }

            for (int i = 1; i <= PanelsData.Count; i++)
            {
                InvoiceTable.RowDefinitions[i].MinHeight = 35;
            }

            string ArabicVAT = VATPercentage.ToString("N0").ToArabicNumbers();
            string textPrice = null;
            string textPriceArabic = null;

            if (TotalCost != 0)
            {
                TotalPrice = Math.Truncate(TotalPrice * 100) / 100d;
                TotalVAT = Math.Truncate(TotalVAT * 100) / 100d;

                InvoiceTable.RowDefinitions[15].Height = new GridLength(20);
                InvoiceTable.RowDefinitions[16].Height = new GridLength(20);
                InvoiceTable.RowDefinitions[17].Height = new GridLength(20);
                for (int i = 1; i <= 6; i++)
                {
                    ((Grid)FindName($"Total{i}")).Visibility = Visibility.Visible;
                }

                textPrice = Input.NumberToSRWords((decimal)TotalPrice);
                ToWord toWord = new((decimal)TotalPrice, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
                textPriceArabic = toWord.ConvertToArabic();

                QR.Encoder encoder = new()
                {
                    SellerName = Database.CompanyName,
                    VATNumber = InvoiceData.CompanyVAT.ToString(),
                    Date = InvoiceData.Date,
                    Total = TotalPrice.ToString(),
                    VATValue = TotalVAT.ToString(),
                };
                QR.Source = encoder.GetQRImage();
            }

            DataContext = new { PanelsData, InvoiceData, TotalCost, VATPercentage, TotalVAT, TotalPrice, textPrice, textPriceArabic, ArabicVAT, Page, Pages };
        }
    }
}
