using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;

using System.Collections.ObjectModel;

namespace ProjectsNow.Views.JobOrdersViews.ItemsPurchaseOrdersViews
{
    internal class ItemQtyViewModel : ViewModelBase
    {
        private decimal _Posting;
        private decimal _NetPrice;
        private decimal _VAT = AppData.VAT;

        public ItemQtyViewModel(ItemPurchased item, ObservableCollection<ItemPurchased> items, ObservableCollection<CompanyPOTransaction> purchaseItemsData)
        {
            Item = item;
            NewData.Update(Item);
            Items = purchaseItemsData;

            Posting = NewData.RemainingQty;
            NetPrice = NewData.ListPrice;
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public decimal Posting
        {
            get => _Posting;
            set
            {
                if (NewData.RemainingQty >= value)
                {
                    SetValue(ref _Posting, value)
                   .UpdateProperties(this, nameof(GrossPrice));
                }
                else
                {
                    SetValue(ref _Posting, NewData.RemainingQty)
                   .UpdateProperties(this, nameof(GrossPrice));
                }
            }
        }
        public decimal NetPrice
        {
            get => _NetPrice;
            set => SetValue(ref _NetPrice, value)
                  .UpdateProperties(this, nameof(GrossPrice));
        }
        public decimal VAT
        {
            get => _VAT;
            set => SetValue(ref _VAT, value)
                  .UpdateProperties(this, nameof(GrossPrice));
        }
        public decimal GrossPrice => Posting * NetPrice * (1 + VAT / 100);

        public ItemPurchased Item { get; }
        public ItemPurchased NewData { get; } = new ItemPurchased();
        public ObservableCollection<CompanyPOTransaction> Items { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            NewData.InOrderQty += Posting;

            CompanyPOTransaction newItem = new()
            {
                VAT = VAT,
                Code = NewData.Code,
                Description = NewData.Description,
                Unit = NewData.Unit,
                Qty = Posting,
                Cost = NetPrice,
            };

            Item.Update(NewData);

            if (Items != null)
            {
                Items.Add(newItem);
            }

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            return true;
        }

        private void Cancel()
        {
            Navigation.ClosePopup();
        }
        private bool CanCancel()
        {
            return true;
        }
    }
}