using ProjectsNow.Data.Application;
using ProjectsNow.Data.Customers;
using ProjectsNow.Data.Store;
using ProjectsNow.DataInput;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.Store
{
    public partial class SalesInvoiceForm : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal VATPercentage { get; set; }
        public List<SalesItem> ItemsData { get; set; }
        public SalesInvoice InvoiceData { get; set; }

        public SalesInvoiceForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            BackgroundImage.Source = AppData.CompanyWatermark;

            for (int i = ItemsData.Count + 1; i <= 14; i++)
            {
                InvoiceTable.RowDefinitions[i].Height = new GridLength(0);
            }

            InvoiceTable.RowDefinitions[0].Height = new GridLength(50);

            for (int i = 1; i <= 6; i++)
            {
                ((Grid)FindName($"Total{i}")).Visibility = Visibility.Collapsed;
            }

            for (int i = 1; i <= ItemsData.Count; i++)
            {
                InvoiceTable.RowDefinitions[i].MinHeight = 35;
            }

            string ArabicVAT = VATPercentage.ToString("N0").ToArabicNumbers();
            string textPrice = null;
            string textPriceArabic = null;

            if (TotalCost != 0)
            {
                InvoiceTable.RowDefinitions[15].Height = new GridLength(20);
                for (int i = 1; i <= 6; i++)
                {
                    ((Grid)FindName($"Total{i}")).Visibility = Visibility.Visible;
                }

                textPrice = Input.NumberToSRWords(decimal.Parse(TotalPrice.ToString("N2")));
                ToWord toWord = new(decimal.Parse(TotalPrice.ToString("N2")), new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
                textPriceArabic = toWord.ConvertToArabic();
            }

            var total = Math.Truncate(TotalPrice * 100) / 100m;
            var vat = Math.Truncate(TotalVAT * 100) / 100m;

            QR.Encoder encoder = new()
            {
                SellerName = AppData.CompanyData.FullName,
                VATNumber = AppData.CompanyData.VATNumber.ToString(),
                Date = InvoiceData.Date,
                Total = total.ToString(),
                VATValue = vat.ToString(),
            };
            QR.Source = encoder.GetQRImage();
            DataContext = new { ItemsData, InvoiceData, TotalCost, VATPercentage, TotalVAT, TotalPrice, textPrice, textPriceArabic, ArabicVAT, Page, Pages };
        }
    }
}
