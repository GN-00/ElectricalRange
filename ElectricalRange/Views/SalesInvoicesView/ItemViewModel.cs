using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.SalesInvoicesView
{

    internal class ItemViewModel : ViewModelBase
    {
        private ItemStock _SelectedReference;
        public ItemStock SelectedReference
        {
            get => _SelectedReference;
            set
            {
                if (SetValue(ref _SelectedReference, value))
                {
                    if (SelectedReference != null)
                    {
                        NewData.Code = SelectedReference.Code;
                        NewData.Description = SelectedReference.Description;
                        NewData.Unit = SelectedReference.Unit;
                        NewData.Brand = SelectedReference.Brand;
                    }
                    else
                    {
                        NewData.Code = null;
                        NewData.Description = null;
                        NewData.Unit = null;
                        NewData.Brand = null;
                    }
                }
            }
        }
        public ObservableCollection<ItemStock> ReferencesData { get; private set; }
        public ItemViewModel(InvoiceItem item, ObservableCollection<InvoiceItem> items)
        {
            GetData();

            ItemData = item;
            NewData.Update(item);
            ItemsData = items;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = "Select * from [Store].[JobOrderStock(AvgCost)] " +
                    "Where JobOrderID = 0 " +
                    "Order By Code";

            ReferencesData = new ObservableCollection<ItemStock>(connection.Query<ItemStock>(query));
        }

        public InvoiceItem NewData { get; private set; } = new InvoiceItem();
        public InvoiceItem ItemData { get; private set; }
        public ObservableCollection<InvoiceItem> ItemsData { get; private set; }

        public double NewQty
        {
            get => NewData.Qty;
            set
            {
                if (value > (double)InStockQty)
                {
                    NewData.Qty = (double)InStockQty;
                }
                else
                {
                    NewData.Qty = value;
                }

                OnPropertyChanged();
            }
        }
        public decimal InStockQty
        {
            get
            {
                if (SelectedReference == null)
                {
                    return 0;
                }
                else
                {
                    return SelectedReference.Qty;
                }
            }
        }
        public double UnitNetPrice
        {
            get => NewData.UnitNetPrice;
            set
            {
                if (value != NewData.UnitNetPrice)
                {
                    NewData.UnitNetPrice = value;
                    NewData.NetPrice = NewData.UnitNetPrice * NewData.Qty;

                    NewData.UnitGrossPrice = value * (1 + NewData.VAT);
                    NewData.GrossPrice = NewData.UnitNetPrice * NewData.Qty * (1 + NewData.VAT);

                    NewData.UnitVATValue = value * NewData.VAT;
                    NewData.VATValue = NewData.UnitNetPrice * NewData.Qty * NewData.VAT;

                    OnPropertyChanged();
                }

            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            ItemsData.Add(NewData);
            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            return NewData.Qty > 0;
        }

        private void Cancel()
        {
            WindowData.Close();
        }
        private bool CanCancel()
        {
            return true;
        }
    }
}