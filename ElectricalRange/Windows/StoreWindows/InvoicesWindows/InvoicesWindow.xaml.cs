using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;

using System.Windows;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows
{
    public partial class InvoicesWindow : Window
    {
        public InvoicesWindow(JobOrder jobOrder, User user)
        {
            InitializeComponent();
            DataContext = new InvoicesViewModel(jobOrder, user);
        }
    }
}
