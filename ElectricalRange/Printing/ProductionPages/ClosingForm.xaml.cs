using ProjectsNow.Data.Production;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.ProductionPages
{
    public partial class CloseForm : UserControl
    {
        public CloseForm(CloseRequestInfo requestData)
        {
            InitializeComponent();
            DataContext = requestData;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;
        }
    }
}
