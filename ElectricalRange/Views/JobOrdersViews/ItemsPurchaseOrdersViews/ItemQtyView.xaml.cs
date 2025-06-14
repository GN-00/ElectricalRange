using ProjectsNow.Data;

using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews
{
    public partial class ItemQtyView : UserControl, IPopup
    {
        ItemPurchased ItemData { get; set; }

        public ItemQtyView(ItemPurchased item, ObservableCollection<ItemPurchased> items, ObservableCollection<CompanyPOTransaction> purchaseItemsData)
        {
            InitializeComponent();
            ItemData = item;
            DataContext = new ItemQtyViewModel(item, items, purchaseItemsData);
        }

        private void NetPrice_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }

        private void PostingInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ItemData.Unit == "No" || ItemData.Unit == "Set")
            {
                DataInput.Input.IntOnly(e, 4);
            }
            else
            {
                DataInput.Input.DoubleOnly(e);
            }
        }
    }
}
