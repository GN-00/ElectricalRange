using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Finance;
using ProjectsNow.DataInput;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.Finance
{
    public partial class ProformaInvoiceForm1 : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal VATPercentage { get; set; }
        public List<IPanel> PanelsData { get; set; }
        public ProformaInvoiceInformation InvoiceInformationData { get; set; }

        public ProformaInvoiceForm1()
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

            InvoiceTable.RowDefinitions[0].Height = new GridLength(50);

            for (int i = 1; i <= 7; i++)
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
                InvoiceTable.RowDefinitions[15].Height = new GridLength(20);
                InvoiceTable.RowDefinitions[16].Height = new GridLength(20);
                for (int i = 1; i <= 7; i++)
                {
                    ((Grid)FindName($"Total{i}")).Visibility = Visibility.Visible;
                }

                textPrice = Input.NumberToSRWords(InvoiceInformationData.DownPaymentAmount);
                ToWord toWord = new(InvoiceInformationData.DownPaymentAmount, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
                textPriceArabic = toWord.ConvertToArabic();
            }

            DataContext = new
            {
                PanelsData,
                InvoiceInformationData,
                TotalCost,
                VATPercentage,
                TotalVAT,
                TotalPrice,
                textPrice,
                textPriceArabic,
                ArabicVAT,
                Page,
                Pages
            };
        }
    }
}
