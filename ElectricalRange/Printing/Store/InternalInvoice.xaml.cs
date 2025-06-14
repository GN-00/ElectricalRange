using ProjectsNow.Data.JobOrders;
using ProjectsNow.DataInput;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.Store
{
    public partial class InternalInvoice : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal VATPercentage { get; set; }
        public List<Item> ItemsData { get; set; }
        public InvoiceInformation InvoiceData { get; set; }

        public InternalInvoice()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            foreach (RowDefinition row in InvoiceTable.RowDefinitions)
            {
                row.Height = new GridLength(0);
            }

            InvoiceTable.RowDefinitions[0].Height = new GridLength(50);

            for (int i = 1; i <= 6; i++)
            {
                ((Grid)FindName($"Total{i}")).Visibility = Visibility.Collapsed;
            }

            for (int i = 1; i <= ItemsData.Count; i++)
            {
                InvoiceTable.RowDefinitions[i].Height = new GridLength(40);
            }

            string textPrice = null;
            string textPriceArabic = null;
            if (TotalCost != 0)
            {
                InvoiceTable.RowDefinitions[9].Height = new GridLength(20);
                for (int i = 1; i <= 6; i++)
                {
                    ((Grid)FindName($"Total{i}")).Visibility = Visibility.Visible;
                }

                textPrice = $"Only {Input.NumberToWords((int)Math.Ceiling(TotalPrice))} Saudi Riyals ";
                ToWord toWord = new(Convert.ToDecimal(((int)Math.Ceiling(TotalPrice)).ToString()), new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));

                textPriceArabic = toWord.ConvertToArabic();
            }

            TotalPrice = Math.Ceiling(TotalPrice);
            DataContext = new { ItemsData, InvoiceData, TotalCost, VATPercentage, TotalVAT, TotalPrice, textPrice, textPriceArabic, Page, Pages };
        }
    }
}
