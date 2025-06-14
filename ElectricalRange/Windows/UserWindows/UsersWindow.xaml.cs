
using System.Windows;

namespace ProjectsNow.Windows.UserWindows
{
    public partial class UsersWindow : Window
    {
        public UsersWindow()
        {
            InitializeComponent();
            DataContext = new UsersViewModel();
        }
    }
}
