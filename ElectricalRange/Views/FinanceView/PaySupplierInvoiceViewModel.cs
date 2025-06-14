using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;

using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;

using ReturnInvoice = ProjectsNow.Data.Store.ReturnInvoice;

namespace ProjectsNow.Views.FinanceView
{
    internal class PaySupplierInvoiceViewModel : ViewModelBase
    {
        private double _Amount;

        public PaySupplierInvoiceViewModel(SupplierAccount supplier, SupplierInvoice invoice)
        {
            Supplier = supplier;
            InvoiceData = invoice;
            NewData.Update(InvoiceData);

            Amount = NewData.GrossPrice;
            SaveCommand = new RelayCommand(SupplierSave, CanSupplierSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        public double Amount
        {
            get => _Amount;
            set => SetValue(ref _Amount, value);
        }
        public SupplierAccount Supplier { get; }
        public SupplierInvoice InvoiceData { get; }
        public SupplierInvoice NewData { get; } = new SupplierInvoice();
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public bool UseReturn { get; set; }
        public bool CanUseReturn => NewData.Return > 0;
        private void SupplierSave()
        {
            NewData.Paid = Amount;
            SubTransaction subTransaction = new()
            {
                Date = DateTime.Now,
                Description = $"Invoice {NewData.Number} Payment",
                JobOrderId = NewData.JobOrderID,
                PurchaseOrderId = NewData.PurchaseOrderID,
                PurchaseOrderInvoiceId = NewData.ID,
                Amount = Amount,
                Post = true,
                Type = "Payment",
            };

            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (UseReturn)
                {
                    subTransaction.Amount -= NewData.ReturnValue;
                    query = $"Select * From [Store].[ReturnInvoices(View)] Where OriginalInvoiceID = {NewData.ID}";
                    List<ReturnInvoice> returnInvoices = connection.Query<ReturnInvoice>(query).ToList();

                    foreach (ReturnInvoice returnInvoice in returnInvoices)
                    {
                        SubTransaction returnTransaction = new()
                        {
                            Date = DateTime.Now,
                            Description = $"Return Invoice {returnInvoice.Code} Extract.",
                            JobOrderId = NewData.JobOrderID,
                            PurchaseOrderId = NewData.PurchaseOrderID,
                            PurchaseOrderInvoiceId = NewData.ID,
                            ReturnInvoiceId = returnInvoice.ID,
                            Amount = returnInvoice.ReturnValue,
                            Post = true,
                            Type = "Extract",
                        };

                        _ = connection.Insert(returnTransaction);
                    }
                }
                _ = connection.Insert(subTransaction);

                query = $"Select * From [Finance].[SuppliersAccounts(View)] " +
                        $"Where ID = {Supplier.Id};";
                var supplier = connection.QueryFirstOrDefault<SupplierAccount>(query);

                Supplier.Update(supplier);
                InvoiceData.Update(NewData);
            }

            Navigation.ClosePopup();
        }
        private bool CanSupplierSave()
        {
            if (UseReturn)
            {
                if (Amount > Supplier.Account + NewData.ReturnValue)
                    return false;
            }
            else
            {
                if (Amount > Supplier.Account)
                    return false;
            }

            return true;
        }
        private void Cancel()
        {
            Navigation.ClosePopup();
        }
    }
}