using ProjectsNow.Data.Users;
using ProjectsNow.ViewModels.UsersViews;

using System.Collections.ObjectModel;
using System.Windows;

namespace ProjectsNow.Windows.UserWindows
{
    public partial class UserInfoWindow : Window
    {
        public User UserData { get; set; }
        public ObservableCollection<User> UsersData { get; set; }
        public UserInfoWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UserInfoView userInfoView = new(UserData, UsersData) { WindowData = this };
            DataContext = userInfoView;
        }
    }
}
