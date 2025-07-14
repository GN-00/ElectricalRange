using ProjectsNow.Data.Production;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class FactoryMaterialsRequestView : UserControl, IView
    {
        public FactoryMaterialsRequestView(Order order)
        {
            InitializeComponent();
            DataContext = new FactoryMaterialsRequestViewModel(order, this);
        }

        private void NewItem_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void DeleteRequest_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Print_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
