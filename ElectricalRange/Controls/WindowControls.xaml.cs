using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace ProjectsNow.Controls
{
    public partial class WindowControls : UserControl
    {
        public bool Maximize
        {
            get { return (bool)GetValue(MaximizeProperty); }
            set { SetValue(MaximizeProperty, value); }
        }
        public static readonly DependencyProperty MaximizeProperty =
            DependencyProperty.Register("Maximize", typeof(bool), typeof(WindowControls), new PropertyMetadata(false, SetMaximizeValue));
        private static void SetMaximizeValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WindowControls windowControls = d as WindowControls;
            if (windowControls.Maximize)
            {
                windowControls.MaximizeButton.Visibility = Visibility.Visible;
            }
            else
            {
                windowControls.MaximizeButton.Visibility = Visibility.Collapsed;
            }
        }

        public WindowControls()
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
            Window.GetWindow((Button)sender).Close();
        }
    }
}
