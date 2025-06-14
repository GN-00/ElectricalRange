using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;

namespace ProjectsNow.Views.FinanceView
{
    internal class SelectInvoicingTypeViewModel : ViewModelBase
    {
        public CustomerAccount CustomerData { get; set; }
        public Invoice Invoice { get; set; }
        public ObservableCollection<Invoice> Invoices { get; set; }

        public SelectInvoicingTypeViewModel(Invoice invoice, ObservableCollection<Invoice> invoices, CustomerAccount customer, IView checkPoint)
        {
            Invoice = invoice;
            Invoices = invoices;
            ViewData = checkPoint;
            CustomerData = customer;

            QtyCommand = new RelayCommand(Qty, CanAccessQty);
            PriceCommand = new RelayCommand(Price, CanAccessPrice);
        }

        public RelayCommand QtyCommand { get; }
        public RelayCommand PriceCommand { get; }

        private void Qty()
        {
            Navigation.ClosePopup();
            Navigation.To(new JobOrderInvoiceView(Invoice, Invoices, CustomerData), ViewData);
        }
        private bool CanAccessQty()
        {
            return true;
        }

        private void Price()
        {
            Navigation.ClosePopup();
            Navigation.To(new JobOrderInvoiceByPriceView(Invoice, Invoices), ViewData);
        }
        private bool CanAccessPrice()
        {
            return true;
        }

    }
}