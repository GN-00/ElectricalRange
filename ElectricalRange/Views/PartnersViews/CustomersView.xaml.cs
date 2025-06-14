using ProjectsNow.Data.Customers;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.PartnersViews
{
    public partial class CustomersView : UserControl, IView
    {
        public CustomersView(ObservableCollection<Customer> customers = null)
        {
            InitializeComponent();
            DataContext = new CustomersViewModel(customers, this);
        }
    }
}
