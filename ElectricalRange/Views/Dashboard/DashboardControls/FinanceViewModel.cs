using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.FinanceView;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public class FinanceViewModel : ViewModelBase
    {
        private double _MaxWidth = 1300;
        private User _UserData;

        public FinanceViewModel()
        {
            UserData = Navigation.UserData;
            AccountsCommand = new RelayCommand(Accounts, CanAccessAccounts);
            ReceiptsCommand = new RelayCommand(Receipts, CanAccessReceipts);
            PaymentsCommand = new RelayCommand(Payments, CanAccessPayments);
            JobOrdersCommand = new RelayCommand(JobOrders, CanAccessJobOrders);
            CustomersCommand = new RelayCommand(Customers, CanAccessCustomers);
            SuppliersCommand = new RelayCommand(Suppliers, CanAccessSuppliers);
            PurchaseOrdersCommand = new RelayCommand(PurchaseOrders, CanAccessPurchaseOrders);
            DeliveryNotesCommand = new RelayCommand(DeliveryNotes, CanAccessDeliveryNotes);


            int buttons = 0;
            if (CanAccessAccounts()) buttons += 1;
            if (CanAccessReceipts()) buttons += 1;
            if (CanAccessPayments()) buttons += 1;
            if (CanAccessJobOrders()) buttons += 1;
            if (CanAccessCustomers()) buttons += 1;
            if (CanAccessSuppliers()) buttons += 1;
            if (CanAccessPurchaseOrders()) buttons += 1;
            if (CanAccessDeliveryNotes()) buttons += 1;

            if (buttons == 4) MaxWidth = 900;
        }


        public double MaxWidth
        {
            get => _MaxWidth;
            set => SetValue(ref _MaxWidth, value);
        }
        public User UserData
        {
            get => _UserData;
            set => SetValue(ref _UserData, value);
        }

        public RelayCommand AccountsCommand { get; }
        public RelayCommand ReceiptsCommand { get; }
        public RelayCommand PaymentsCommand { get; }
        public RelayCommand JobOrdersCommand { get; }
        public RelayCommand CustomersCommand { get; }
        public RelayCommand SuppliersCommand { get; }
        public RelayCommand PurchaseOrdersCommand { get; }
        public RelayCommand DeliveryNotesCommand { get; }

        public void Accounts()
        {
            Navigation.To(new CompanyAccountsView());
        }
        public bool CanAccessAccounts()
        {
            return UserData.AccessCompanyAccount;
        }

        public void JobOrders()
        {
            Navigation.To(new JobOrdersView());
        }
        public bool CanAccessJobOrders()
        {
            return UserData.AccessJobordersFinance;
        }

        public void Customers()
        {
            Navigation.To(new CustomersAccountsView());
        }
        public bool CanAccessCustomers()
        {
            return UserData.AccessCustomersAccounts;
        }


        public void Suppliers()
        {
            Navigation.To(new SuppliersAccountsView());
        }
        public bool CanAccessSuppliers()
        {
            return UserData.AccessSuppliersAccounts;
        }


        public void Receipts()
        {
            Navigation.To(new ReceiptsView());
        }
        public bool CanAccessReceipts()
        {
            return UserData.AccessReceipts;
        }

        public void Payments()
        {
            Navigation.To(new PaymentsView());
        }
        public bool CanAccessPayments()
        {
            return UserData.AccessReceipts;
        }


        private void PurchaseOrders()
        {
            Navigation.To(new PurchaseOrdersView());
        }
        private bool CanAccessPurchaseOrders()
        {
            return UserData.AccessSuppliersInvoices;
        }
        private void DeliveryNotes()
        {
            Navigation.To(new DeliveryNotesView());
        }
        private bool CanAccessDeliveryNotes()
        {
            return UserData.AccessAccountantJobOrders;
        }
    }
}