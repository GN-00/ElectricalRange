using ProjectsNow.Data.Finance;
using ProjectsNow.DataInput;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.Finance
{
    public partial class ProformaInvoiceForm : UserControl
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public double VATPercentage { get; set; }
        public string VATPercentageArabic { get; set; }
        public string TextAmount { get; set; }
        public string TextAmountArabic { get; set; }

        public ProformaInvoice InvoiceData { get; set; }
        public List<ProformaInvoicePanel> PanelsData { get; set; }

        public Visibility DescriptionVisibilty { get; set; } = Visibility.Collapsed;
        public ProformaInvoiceForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

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

            if (PageNumber == TotalPages)
            {
                if (InvoiceData.Description != "-")
                {
                    TableArea.RowDefinitions[20].Height = new GridLength(25);
                    AmountArea.Visibility = Visibility.Visible;
                }

                TotalArea.Visibility = SignatureArea.Visibility = Visibility.Visible;
                TableArea.RowDefinitions[19].Height = new GridLength(25);

                InvoiceData.GrossPrice = Math.Truncate(InvoiceData.GrossPrice * 100) / 100d;
                InvoiceData.VATValue = Math.Truncate(InvoiceData.VATValue * 100) / 100d;

                if (InvoiceData.Amount != 0)
                {
                    VATPercentageArabic = VATPercentage.ToString("N0").ToArabicNumbers();

                    TextAmount = Input.NumberToSRWords((decimal)InvoiceData.Amount);
                    ToWord toWord = new((decimal)InvoiceData.Amount, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
                    TextAmountArabic = toWord.ConvertToArabic();
                }
            }

            DataContext = new
            {
                PanelsData,
                InvoiceData,
                VATPercentage,
                VATPercentageArabic,
                PageNumber,
                TotalPages,
                TextAmount,
                TextAmountArabic,
            };
        }
    }
}
