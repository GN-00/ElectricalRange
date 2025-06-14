using ProjectsNow.Views;

using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Controls
{
    public partial class PopupControls : UserControl
    {
        public PopupControls()
        {
            InitializeComponent();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow((Button)sender).WindowState = WindowState.Minimized; 
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ClosePopup();
        }
    }
}
