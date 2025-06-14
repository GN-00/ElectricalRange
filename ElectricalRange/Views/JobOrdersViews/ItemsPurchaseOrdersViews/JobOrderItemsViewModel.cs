using ProjectsNow.Commands;
using ProjectsNow.Data;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews
{
    internal class JobOrderItemsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ItemPurchased _SelectedItem;
        private ObservableCollection<ItemPurchased> _Items;
        private ICollectionView _ItemsCollection;

        public JobOrderItemsViewModel(CompanyPO order, ObservableCollection<CompanyPOTransaction> items, ObservableCollection<ItemPurchased> jobOrderItems)
        {
            OrderData = order;
            PurchaseItemsData = items;
            JobOrderItemsData = jobOrderItems;

            GetData();

            AddCommand = new RelayCommand<ItemPurchased>(Add, CanAdd);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }


        public CompanyPO OrderData { get; private set; }
        public ObservableCollection<CompanyPOTransaction> PurchaseItemsData { get; private set; }
        public ObservableCollection<ItemPurchased> JobOrderItemsData { get; private set; }

        public string Indicator
        {
            get => _Indicator;
            set => SetValue(ref _Indicator, value);
        }
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                if (SetValue(ref _SelectedIndex, value))
                {
                    UpdateIndicator();
                }
            }
        }
        public ItemPurchased SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<ItemPurchased> Items
        {
            get => _Items;
            private set
            {
                if (SetValue(ref _Items, value))
                {
                    CreateCollectionView();
                }
            }
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand<ItemPurchased> AddCommand { get; }
        public RelayCommand CancelCommand { get; }


        private void GetData()
        {
            Items = JobOrderItemsData;
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += CollectionChanged;
        }
        private bool DataFilter(object obj)
        {
            if (!(obj is ItemPurchased item))
                return false;

            decimal value = item.PurchasedQty + item.InOrderQty;
            decimal checkValue = item.Qty;

            if (value >= checkValue)
                return false;

            if (PurchaseItemsData.Any(x => x.PurchaseOrderID == 0 && x.Code == item.Code))
                return false;

            return true;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }

        private void Add(ItemPurchased item)
        {
            Navigation.OpenPopup(new ItemQtyView(item, Items, PurchaseItemsData), PlacementMode.Center, true);
            Navigation.ClosePopupEvent += UpdateList;
        }
        private void UpdateList()
        {
            ItemsCollection.Refresh();
        }
        private bool CanAdd(ItemPurchased item)
        {
            if (item == null)
                return false;

            if (item.RemainingQty <= 0)
                return false;

            return true;
        }

        private void Cancel()
        {
            Navigation.Back();
        }
        private bool CanCancel()
        {
            return true;
        }
    }
}