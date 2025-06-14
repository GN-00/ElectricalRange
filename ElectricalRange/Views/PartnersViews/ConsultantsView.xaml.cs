using System.Windows.Controls;

namespace ProjectsNow.Views.PartnersViews
{
    public partial class ConsultantsView : UserControl, IView
    {
        public ConsultantsView()
        {
            InitializeComponent();
            DataContext = new ConsultantsViewModel();
        }
    }
}
