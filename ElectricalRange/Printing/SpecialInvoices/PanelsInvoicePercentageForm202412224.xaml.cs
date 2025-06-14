using System.Windows;
using System.Windows.Controls;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Finance;
using ProjectsNow.DataInput;

namespace ProjectsNow.Printing.SpecialInvoices
{
    public partial class PanelsInvoicePercentageForm202412224 : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public double NetPrice { get; set; }
        public double VATValue { get; set; }
        public double GrossPrice { get; set; }
        public double VATPercentage { get; set; }
        public List<InvoiceItem> PanelsData { get; set; }
        public Invoice InvoiceData { get; set; }
        public PanelsInvoicePercentageForm202412224()
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


            for (int i = PanelsData.Count + 1; i <= 16; i++)
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

                InvoiceTable.RowDefinitions[17].Height = new GridLength(20);
                InvoiceTable.RowDefinitions[18].Height = new GridLength(20);
                InvoiceTable.RowDefinitions[19].Height = new GridLength(20);
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
