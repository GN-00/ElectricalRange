using System.Linq;
using System.Windows;
using ProjectsNow.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using ProjectsNow.DataInput;
using ProjectsNow.Data.Application;

namespace ProjectsNow.Printing.Purchase
{
    public partial class PurchaseOrderForm : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public CompanyPO CompanyPOData { get; set; }
        public List<CompanyPOTransaction> Transactions { get; set; }
        public List<CompanyPOTransaction> AllTransactions { get; set; }

        private decimal subtotal;
        private decimal VAT;
        private decimal grandTotal;
        private string grandTotalText;
        public PurchaseOrderForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            if (CompanyPOData.JobOrderCode == null)
                CompanyPOData.JobOrderCode = "Stock";

            for (int i = Transactions.Count + 1; i <= 20; i++)
            {
                ItemsTable.RowDefinitions[i].Height = new GridLength(0);
            }

            for (int i = 0; i < Transactions.Count; i++)
            {
                ItemsTable.RowDefinitions[i + 1].MinHeight = 0.6 * AppData.cm;
            }

            if (Transactions.Count == 0)
                ItemsTable.RowDefinitions[0].Height = new GridLength(1);
            else
                ItemsTable.RowDefinitions[0].Height = new GridLength(0.6 * AppData.cm);

            subtotal = Math.Truncate(AllTransactions.Sum(t => t.Qty * t.Cost) * 100m) /100m;
            VAT = Math.Truncate(subtotal * (CompanyPOData.VAT / 100m) * 100m) / 100m;
            grandTotal = Math.Truncate(subtotal * (1 + (CompanyPOData.VAT / 100m)) * 100m) / 100m;


            if (Page != Pages)
            {
                TotalTable.Visibility = Visibility.Collapsed;
            }
            else
            {
                grandTotalText = Input.NumberToSRWords(grandTotal);
            }

            DataContext = new { Page, Pages, Transactions, CompanyPOData, subtotal, VAT, grandTotal, grandTotalText };
        }
    }
}
