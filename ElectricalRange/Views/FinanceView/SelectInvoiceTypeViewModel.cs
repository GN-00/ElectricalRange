using Dapper;

using DocumentFormat.OpenXml.Drawing.Charts;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;

using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.FinanceView
{
    internal class SelectInvoiceTypeViewModel: ViewModelBase
    {
        public SelectInvoiceTypeViewModel(JobOrder order, IView checkPoint)
        {
            ViewData = checkPoint;
            OrderData = order;
            UserData = Navigation.UserData;

            InvoicesCommand = new RelayCommand(Invoices, CanAccessInvoices);
            ProformaCommand = new RelayCommand(Proforma, CanAccessProforma);
            SuppliersCommand = new RelayCommand(Suppliers, CanAccessSuppliers);
        }

        private JobOrder OrderData { get; }
        private User UserData { get; }

        public RelayCommand InvoicesCommand { get; }
        public RelayCommand ProformaCommand { get; }
        public RelayCommand SuppliersCommand { get; }


        private void Invoices()
        {
            Navigation.ClosePopup();
            FinanceServices.GetJobOrderInvoices(OrderData, ViewData);
        }
        private bool CanAccessInvoices()
        {
            return FinanceServices.CanGetJobOrderInvoices(OrderData);
        }

        private void Proforma()
        {
            Navigation.ClosePopup();
            if (UserData.Access(OrderData))
            {
                Navigation.To(new ProformaInvoicesView(OrderData), ViewData);
            }
        }
        private bool CanAccessProforma()
        {
            return true;
        }

        private void Suppliers()
        {
            Navigation.ClosePopup();
            if (UserData.Access(OrderData))
            {
                Navigation.To(new SuppliersInvoicesView(OrderData), ViewData);
            }
        }
        private bool CanAccessSuppliers()
        {
            return true;
        }
    }
}