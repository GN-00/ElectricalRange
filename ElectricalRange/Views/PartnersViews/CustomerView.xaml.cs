using ProjectsNow.Data.Customers;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.PartnersViews
{
    public partial class CustomerView : UserControl, IView
    {
        public CustomerView(Customer customer, ObservableCollection<Customer> customers)
        {
            InitializeComponent();
            DataContext = new CustomerViewModel(customer, customers, this);
        }
        private void POBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 4);
        }
        private void VAT_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 15);
        }
        private void CR_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 10);
        }
        private void CreditLimit_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
