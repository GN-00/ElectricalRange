using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.ReferencesViews;
using ProjectsNow.Views.StoreViews;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public class StoreViewModel : ViewModelBase
    {
        private double _MaxWidth = 1300;
        private User _UserData;

        public StoreViewModel()
        {
            UserData = Navigation.UserData;
            ReferencesCommand = new RelayCommand(References, CanAccessReferences);
            StockCommand = new RelayCommand(Stock, CanAccessStock);
            SalesInvoicesCommand = new RelayCommand(SalesInvoices, CanAccessSalesInvoices);

            int buttons = 0;
            if (CanAccessReferences()) buttons += 1;
            if (CanAccessStock()) buttons += 1;
            if (CanAccessSalesInvoices()) buttons += 1;

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

        public RelayCommand ReferencesCommand { get; }
        public RelayCommand StockCommand { get; }
        public RelayCommand SalesInvoicesCommand { get; }

        public void References()
        {
            Navigation.To(new ReferencesView());
        }
        public bool CanAccessReferences()
        {
            return UserData.AccessReferences;
        }

        public void Stock()
        {
            Navigation.To(new StockView());
        }
        public bool CanAccessStock()
        {
            return UserData.AccessStore;
        }

        private void SalesInvoices()
        {
            Navigation.To(new SalesInvoicesView.InvoicesView());
        }
        private bool CanAccessSalesInvoices()
        {
            return UserData.AccessSalesInvoices;
        }
    }
}