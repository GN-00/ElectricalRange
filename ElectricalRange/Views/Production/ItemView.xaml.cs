using ProjectsNow.Data.Production;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Views.Production
{
    public partial class ItemView : UserControl, IPopup
    {
        public ItemView(Item item, ObservableCollection<Item> items)
        {
            InitializeComponent();
            DataContext = new ItemViewModel(item, items);
        }

        private void Qty_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }

        private void Cost_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
