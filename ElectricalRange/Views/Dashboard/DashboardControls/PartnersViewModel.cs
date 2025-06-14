using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.PartnersViews;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public class PartnersViewModel : ViewModelBase
    {
        private double _MaxWidth = 1300;
        private User _UserData;

        public PartnersViewModel()
        {
            UserData = Navigation.UserData;
            CustomersCommand = new RelayCommand(Customers, CanAccessCustomers);
            SuppliersCommand = new RelayCommand(Suppliers, CanAccessSuppliers);
            ConsultantsCommand = new RelayCommand(Consultants, CanAccessConsultants);
            OthersCommand = new RelayCommand(Others, CanAccessOthers);

            int buttons = 0;
            if (CanAccessCustomers()) buttons += 1;
            if (CanAccessSuppliers()) buttons += 1;
            if (CanAccessConsultants()) buttons += 1;
            if (CanAccessOthers()) buttons += 1;

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

        public RelayCommand CustomersCommand { get; }
        public RelayCommand SuppliersCommand { get; }
        public RelayCommand ConsultantsCommand { get; }
        public RelayCommand OthersCommand { get; }

        public void Customers()
        {
            Navigation.To(new CustomersView());
        }
        public bool CanAccessCustomers()
        {
            return UserData.AccessCustomers;
        }

        public void Suppliers()
        {
            Navigation.To(new SuppliersView());
        }
        public bool CanAccessSuppliers()
        {
            return UserData.AccessSuppliers;
        }

        private void Consultants()
        {
            Navigation.To(new ConsultantsView());
        }

        private bool CanAccessConsultants()
        {
            return UserData.AccessConsultants;
        }

        private void Others()
        {
            Navigation.To(new OthersView());
        }

        private bool CanAccessOthers()
        {
            return UserData.AccessOthers;
        }
    }
}