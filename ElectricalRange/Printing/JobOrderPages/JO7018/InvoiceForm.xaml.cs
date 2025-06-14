using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.DataInput;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.JobOrderPages.JO7018
{
    public partial class InvoiceForm : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal VATPercentage { get; set; }
        public List<IPanel> PanelsData { get; set; }
        public InvoiceInformation InvoiceInformationData { get; set; }


        public InvoiceForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;


            //for (int i = PanelsData.Count + 1; i <= 14; i++)
            //{
            //    InvoiceTable.RowDefinitions[i].Height = new GridLength(0);
            //}

            InvoiceTable.RowDefinitions[0].Height = new GridLength(50);

            for (int i = 1; i <= 6; i++)
            {
                ((Grid)FindName($"Total{i}")).Visibility = Visibility.Collapsed;
            }

            //for (int i = 1; i <= PanelsData.Count; i++)
            //{
            //    InvoiceTable.RowDefinitions[i].MinHeight = 35;
            //}

            string ArabicVAT = VATPercentage.ToString("N0").ToArabicNumbers();
            string textPrice = null;
            string textPriceArabic = null;

            if (TotalCost != 0)
            {
                InvoiceTable.RowDefinitions[2].Height = new GridLength(20);
                for (int i = 1; i <= 6; i++)
                {
                    ((Grid)FindName($"Total{i}")).Visibility = Visibility.Visible;
                }

                textPrice = Input.NumberToSRWords(TotalPrice);
                ToWord toWord = new(TotalPrice, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
                textPriceArabic = toWord.ConvertToArabic();
            }

            QR.Encoder encoder = new()
            {
                SellerName = Database.CompanyName,
                VATNumber = InvoiceInformationData.CompanyVAT.ToString(),
                Date = InvoiceInformationData.Date,
                Total = TotalPrice.ToString("N2"),
                VATValue = TotalVAT.ToString("N2"),
            };
            QR.Source = encoder.GetQRImage();
            DataContext = new { PanelsData, InvoiceInformationData, TotalCost, VATPercentage, TotalVAT, TotalPrice, textPrice, textPriceArabic, ArabicVAT, Page, Pages };
        }
    }
}
