using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class SelectItemTypeViewModel : ViewModelBase
    {
        public SelectItemTypeViewModel(QItem item, ObservableCollection<QItem> items, ItemsViewModel itemsViewModel)
        {
            ItemData = item;
            ItemsData = items;
            ItemsViewModelData = itemsViewModel;

            StandardCommand = new RelayCommand(Standard, CanStandard);
            CustomCommand = new RelayCommand(Custom, CanCustom);
        }

        public QItem ItemData { get; }
        public ObservableCollection<QItem> ItemsData { get; }
        public ItemsViewModel ItemsViewModelData { get; }
        public RelayCommand StandardCommand { get; }
        public RelayCommand CustomCommand { get; }

        private void Standard()
        {
            Navigation.OpenPopup(new AddStandardItemView(ItemData, ItemsData), PlacementMode.Center, true);
        }
        private bool CanStandard()
        {
            return true;
        }

        private void Custom()
        {
            Navigation.OpenPopup(new AddCustomItemView(ItemData, ItemsData), PlacementMode.Center, true);
        }
        private bool CanCustom()
        {
            return true;
        }
    }
}