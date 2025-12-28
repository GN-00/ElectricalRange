using System.Windows;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class PurchaseOrderImportItemsWindow : Window
    {
        public PurchaseOrderImportItemsWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
