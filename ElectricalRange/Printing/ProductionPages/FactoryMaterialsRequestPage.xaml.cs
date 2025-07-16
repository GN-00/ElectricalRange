using ProjectsNow.Data.Production;

using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.ProductionPages
{
    public partial class FactoryMaterialsRequestPage : UserControl
    {
        public FactoryMaterialsRequestPage(MaterialsRequest request)
        {
            InitializeComponent();
            DataContext = request;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;
        }
    }
}
