using ProjectsNow.Data.References;

using System.Collections.ObjectModel;
using System.Windows;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class UpdateCopperWindow : Window
    {
        public UpdateCopperWindow(ObservableCollection<Reference> items)
        {
            InitializeComponent();
            DataContext = new UpdateCopperViewModel(items, this);
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
