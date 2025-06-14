using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Views.UserViews
{
    public partial class LoginView : UserControl, IView
    {
        public LoginViewModel LoginViewModel { get; set; }
        public LoginView()
        {
            InitializeComponent();
            LoginViewModel = new LoginViewModel();
            DataContext = LoginViewModel;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Password.Password = LoginViewModel.Password;
            Username.Focus();
        }
        private void UserKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _ = Password.Focus();
            }
        }
        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginViewModel.LoginCommand.Execute();
            }
        }
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LoginViewModel.Password = ((PasswordBox)sender).Password;
        }
    }
}
