using ProjectsNow.Data.Production;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class ItemsView : UserControl, IView
    {
        public ItemsView(ProductionPanel panel)
        {
            InitializeComponent();
            DataContext = new ItemsViewModel(panel, this);
        }
    }
}
