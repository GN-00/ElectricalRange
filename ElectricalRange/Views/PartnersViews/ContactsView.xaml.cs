using ProjectsNow.Data.Customers;

using System.Windows.Controls;

namespace ProjectsNow.Views.PartnersViews
{
    public partial class ContactsView : UserControl, IView
    {
        public ContactsView(Customer customer)
        {
            InitializeComponent();
            DataContext = new ContactsViewModel(customer);
        }
    }
}
