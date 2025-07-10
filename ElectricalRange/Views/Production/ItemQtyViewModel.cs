using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Production;
using System.Collections.ObjectModel;

namespace ProjectsNow.Views.Production
{
    internal class ItemQtyViewModel : ViewModelBase
    {
        public ItemQtyViewModel(OrderItem item, ObservableCollection<OrderItem> orderItems, ObservableCollection<OrderItem> itemsToAdd)
        {
            Item = item;
            NewData.Update(Item);
            Items = orderItems;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public double Posting
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

        public OrderItem Item { get; }
        public OrderItem NewData { get; } = new OrderItem();
        public ObservableCollection<OrderItem> Items { get; }
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