using System.Windows;
using System.Windows.Input;
using ProjectsNow.Data;
using Microsoft.Data.SqlClient;
using ProjectsNow.Controllers;
using Dapper.Contrib.Extensions;
using ProjectsNow.Data.Users;

namespace ProjectsNow.Windows.UserWindows
{
    public partial class UserWindow : Window
    {
        public User UserData { get; set; }

        private User newUserData;
        public UserWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newUserData = new User();
            newUserData.Update(UserData);
            DataContext = newUserData;
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            UserData.Update(newUserData);
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Update(UserData);
            }
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow_Click(sender, e);
        }
    }
}
