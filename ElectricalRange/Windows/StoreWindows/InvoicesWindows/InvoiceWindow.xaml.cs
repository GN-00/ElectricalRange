using Dapper.Contrib.Extensions;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Store;
using ProjectsNow.Enums;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows
{
    public partial class InvoiceWindow : Window
    {
        public Actions ActionData { get; set; }
        public SupplierInvoice SupplierInvoiceData { get; set; }
        public ObservableCollection<SupplierInvoice> SupplierInvoicesData { get; set; }

        private SupplierInvoice supplierInvoice;
        public InvoiceWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            supplierInvoice = new SupplierInvoice();
            supplierInvoice.Update(SupplierInvoiceData);

            InvoiceDate.SelectedDate = supplierInvoice.Date;
            DataContext = supplierInvoice;
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool isReady = true;
            string message = "Please Enter:";

            if (string.IsNullOrWhiteSpace(supplierInvoice.Number)) { message += $"\n  Number."; isReady = false; }

            if (!isReady)
            {
                _ = MessageWindow.Show("Error", message, MessageWindowButton.OK, MessageWindowImage.Information);
                return;
            }

            if (ActionData == Actions.Edit)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Update(supplierInvoice);
                }
                SupplierInvoiceData.Update(supplierInvoice);
            }
            else
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(supplierInvoice);
                }
                SupplierInvoicesData.Add(supplierInvoice);
            }

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;

            if (date == null)
            {
                picker.SelectedDate = supplierInvoice.Date = DateTime.Now;
            }
            else
            {
                picker.SelectedDate = supplierInvoice.Date = date.Value;
            }
        }
    }
}
