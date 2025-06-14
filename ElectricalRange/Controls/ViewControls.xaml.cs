using ProjectsNow.Views;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Controls
{
    public partial class ViewControls : UserControl
    {
        public bool Maximize
        {
            get { return (bool)GetValue(MaximizeProperty); }
            set { SetValue(MaximizeProperty, value); }
        }
        public static readonly DependencyProperty MaximizeProperty =
            DependencyProperty.Register("Maximize", typeof(bool), typeof(ViewControls), new PropertyMetadata(false, SetMaximizeValue));
        private static void SetMaximizeValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViewControls windowControls = d as ViewControls;
            if (windowControls.Maximize)
            {
                windowControls.MaximizeButton.Visibility = Visibility.Visible;
            }
            else
            {
                windowControls.MaximizeButton.Visibility = Visibility.Collapsed;
            }
        }

        public ViewControls()
        {
            InitializeComponent();
        }

        private void Drag_MouseMove(object sender, MouseEventArgs e)
        {
            Window.GetWindow((Grid)sender).DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow((Button)sender).WindowState = WindowState.Minimized; ;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow((Button)sender);

            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Normal;
            }
            else
            {
                window.WindowState = WindowState.Maximized;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Navigation.ResetAccessKeys();
            Window.GetWindow((Button)sender).Close();
        }
    }
}
