using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Finance;
using ProjectsNow.DataInput;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.Finance
{
    public partial class PanelsInvoicePercentageForm : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public double NetPrice { get; set; }
        public double VATValue { get; set; }
        public double GrossPrice { get; set; }
        public double VATPercentage { get; set; }
        public List<InvoiceItem> PanelsData { get; set; }
        public Invoice InvoiceData { get; set; }

        public PanelsInvoicePercentageForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            BackgroundImage.Source = AppData.CompanyWatermark;

            if (InvoiceData.IsReturn)
            {
                FormName.Text = "RETURN VAT INVOICE";
                FormArabicName.Text = "مرتجع فاتورة ضريبية";
            }

            if (Page == 1)
                DetailsArea.Visibility = Visibility.Visible;


            for (int i = PanelsData.Count + 1; i <= 19; i++)
            {
                InvoiceTable.RowDefinitions[i].Height = new GridLength(0);
            }

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

            if (NetPrice != 0)
            {
                var total = Math.Truncate(GrossPrice * 100) / 100d;
                var vat = Math.Truncate(VATValue * 100) / 100d;

                InvoiceTable.RowDefinitions[20].Height = new GridLength(20);
                InvoiceTable.RowDefinitions[21].Height = new GridLength(20);
                InvoiceTable.RowDefinitions[22].Height = new GridLength(20);
                for (int i = 1; i <= 6; i++)
                {
                    ((Grid)FindName($"Total{i}")).Visibility = Visibility.Visible;
                }

                textPrice = Input.NumberToSRWords((decimal)total);
                ToWord toWord = new((decimal)total, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
                textPriceArabic = toWord.ConvertToArabic();


                QR.Encoder encoder = new()
                {
                    SellerName = Database.CompanyName,
                    VATNumber = InvoiceData.CompanyVAT.ToString(),
                    Date = InvoiceData.Date,
                    Total = total.ToString(),
                    VATValue = vat.ToString(),
                };
                QR.Source = encoder.GetQRImage();

                VATValue = vat;
                GrossPrice = total;
            }

            DataContext = new { PanelsData, InvoiceData, NetPrice, VATPercentage, VATValue, GrossPrice, textPrice, textPriceArabic, ArabicVAT, Page, Pages };
        }
    }
}
