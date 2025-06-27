using ProjectsNow.Data.Production;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class PanelsView : UserControl, IView
    {
        public PanelsView(Order order)
        {
            InitializeComponent();
            DataContext = new PanelsViewModel(order, this);
        }
    }
}
