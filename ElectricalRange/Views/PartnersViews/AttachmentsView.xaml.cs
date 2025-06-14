using ProjectsNow.Data.Customers;
using ProjectsNow.Data.Suppliers;

using System.Windows.Controls;

namespace ProjectsNow.Views.PartnersViews
{
    public partial class AttachmentsView : UserControl, IPopup
    {
        public AttachmentsView(Customer customerData, string type)
        {
            InitializeComponent();
            DataContext = new AttachmentsViewModel(customerData, type);
        }

        public AttachmentsView(Supplier supplier, string type)
        {
            InitializeComponent();
            DataContext = new AttachmentsViewModel(supplier, type);
        }
    }
}
