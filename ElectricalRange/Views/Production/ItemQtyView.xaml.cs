using ProjectsNow.Data.Production;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.Production
{
    public partial class ItemQtyView : UserControl, IPopup
    {
        public ItemQtyView(OrderItem item, ObservableCollection<OrderItem> orderItems, ObservableCollection<OrderItem> itemsToAdd)
        {
            InitializeComponent();
            DataContext = new ItemQtyViewModel(item, orderItems, itemsToAdd);
        }
    }
}
