using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;

using System;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.FinanceView
{
    internal class PayInvoiceViewModel : ViewModelBase
    {
        private double _Amount;

        public PayInvoiceViewModel(CustomerAccount customer, Invoice invoice)
        {
            Customer = customer;
            InvoiceData = invoice;
            NewData.Update(InvoiceData);

            Amount = NewData.GrossPrice;
            SaveCommand = new RelayCommand(CustomerSave, CanCustomerSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        public double Amount
        {
            get => _Amount;
            set => SetValue(ref _Amount, value);
        }
        public CustomerAccount Customer { get; }
        public Invoice InvoiceData { get; }
        public Invoice NewData { get; } = new Invoice();
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void CustomerSave()
        {
            NewData.Paid = Amount;
            SubTransaction subTransaction = new()
            {
                Date = DateTime.Now,
                Description = $"Invoice {NewData.Code} Receipt",
                JobOrderId = NewData.JobOrderId,
                JobOrderInvoiceId = NewData.Id,
                Amount = Amount,
                Post = true,
                Type = "Receipt",
            };

            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Insert(subTransaction);

                query = $"Select * From [Finance].[CustomersAccounts(View)] " +
                        $"Where CustomerId = {Customer.Id};";
                var customer = connection.QueryFirstOrDefault<CustomerAccount>(query);

                Customer.Update(customer);
                InvoiceData.Update(NewData);
            }

            Navigation.ClosePopup();
        }
        private bool CanCustomerSave()
        {
            if (Amount > Customer.Account)
                return false;

            return true;
        }

        private void Cancel()
        {
            Navigation.ClosePopup();
        }
    }
}