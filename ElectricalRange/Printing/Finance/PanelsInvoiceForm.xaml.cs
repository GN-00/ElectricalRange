using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.DataInput;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.Finance
{

    public partial class PanelsInvoiceForm : UserControl
    {
        public bool TableTotal { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public double NetPrice { get; set; }
        public double VATValue { get; set; }
        public double GrossPrice { get; set; }
        public double VATPercentage { get; set; }
        public string VATPercentageArabic { get; set; }
        public string TextGrossPrice { get; set; }
        public string TextGrossPriceArabic { get; set; }

        public Invoice InvoiceData { get; set; }
        public List<InvoiceItem> PanelsData { get; set; }

        public PanelsInvoiceForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;
            //BackgroundImage.Source = AppData.CompanyWatermark;

            if (InvoiceData.IsReturn)
            {
                FormName.Text = "RETURN VAT INVOICE";
                FormArabicName.Text = "مرتجع فاتورة ضريبية";
            }

            if (PanelsData.Count == 0)
                TableArea.RowDefinitions[0].Height = new GridLength(1);
            else
                TableArea.RowDefinitions[0].Height = new GridLength(50);

            int maxRows = 19;
            for (int i = 1; i < maxRows; i++)
            {
                if (i <= PanelsData.Count)
                    TableArea.RowDefinitions[i].Height = new GridLength();
                else
                    TableArea.RowDefinitions[i].Height = new GridLength(0);
            }

            if (PageNumber == 1)
                DetailsArea.Visibility = Visibility.Visible;

            if (TableTotal)
            {
                TableArea.RowDefinitions[19].Height = new GridLength(25);
            }

            if (PageNumber == TotalPages)
            {
                TotalArea.Visibility = Visibility.Visible;

                GrossPrice = Math.Truncate(GrossPrice * 100) / 100d;
                VATValue = Math.Truncate(VATValue * 100) / 100d;

                if (GrossPrice != 0)
                {
                    VATPercentageArabic = VATPercentage.ToString("N0").ToArabicNumbers();

                    TextGrossPrice = Input.NumberToSRWords((decimal)GrossPrice);
                    ToWord toWord = new((decimal)GrossPrice, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
                    TextGrossPriceArabic = toWord.ConvertToArabic();

                    QR.Encoder encoder = new()
                    {
                        SellerName = Database.CompanyName,
                        VATNumber = InvoiceData.CompanyVAT.ToString(),
                        Date = InvoiceData.Date,
                        Total = GrossPrice.ToString(),
                        VATValue = VATValue.ToString(),
                    };

                    QR.Source = encoder.GetQRImage();
                }
            }

            DataContext = new
            {
                PanelsData,
                InvoiceData,
                NetPrice,
                VATPercentage,
                VATPercentageArabic,
                VATValue,
                GrossPrice,
                TextGrossPrice,
                TextGrossPriceArabic,
                PageNumber,
                TotalPages
            };
        }
    }
}
