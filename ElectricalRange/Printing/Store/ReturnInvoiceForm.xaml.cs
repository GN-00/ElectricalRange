using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Store;
using ProjectsNow.DataInput;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.Store
{
    public partial class ReturnInvoiceForm : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal VATPercentage { get; set; }
        public List<Item> ItemsData { get; set; }
        public ReturnInvoice InvoiceData { get; set; }

        public ReturnInvoiceForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

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
                InvoiceTable.RowDefinitions[i].Height = new GridLength(35);
            }

            string ArabicVAT = VATPercentage.ToString().ToArabicNumbers();
            string textPrice = null;
            string textPriceArabic = null;

            if (TotalCost != 0)
            {
                InvoiceTable.RowDefinitions[15].Height = new GridLength(20);
                for (int i = 1; i <= 6; i++)
                {
                    ((Grid)FindName($"Total{i}")).Visibility = Visibility.Visible;
                }

                textPrice = Input.NumberToSRWords(TotalPrice);
                ToWord toWord = new(TotalPrice, new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
                textPriceArabic = toWord.ConvertToArabic();
            }

            DataContext = new { ItemsData, InvoiceData, TotalCost, VATPercentage, TotalVAT, TotalPrice, textPrice, textPriceArabic, ArabicVAT, Page, Pages };
        }
    }
}
