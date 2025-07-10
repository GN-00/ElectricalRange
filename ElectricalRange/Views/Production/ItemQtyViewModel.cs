using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using System.Collections.ObjectModel;

namespace ProjectsNow.Views.Production
{
    internal class ItemQtyViewModel : ViewModelBase
    {
        public ItemQtyViewModel(OrderItem item)
        {
            Item = item;
            NewData.Update(Item);

            Posting = NewData.Missing;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        private double _Posting;
        public double Posting
        {
            get => _Posting;
            set
            {
                if (NewData.Missing >= value)
                    SetValue(ref _Posting, value);
                else
                    SetValue(ref _Posting, NewData.Missing);
            }
        }

        public OrderItem Item { get; }
        public OrderItem NewData { get; } = new OrderItem();
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            NewData.Stock += Posting;

            AddStock newItem = new()
            {
                JobOrderId = Item.JobOrderId,
                Code = Item.Code,
                Description = Item.Description,
                Qty = Posting,
                Date = DateTime.Now
            };

            Item.Update(NewData);

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Insert(newItem);

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