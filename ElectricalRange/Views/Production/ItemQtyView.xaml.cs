using ProjectsNow.Data.Production;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class ItemQtyView : UserControl, IPopup
    {
        public ItemQtyView(OrderItem item)
        {
            InitializeComponent();
            DataContext = new ItemQtyViewModel(item);
        }
    }
}
