using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.References;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using ProjectsNow.Data.Production;
using ProjectsNow.Data;

namespace ProjectsNow.Views.Production
{
    internal class ItemViewModel : ViewModelBase
    {
        private Reference _SelectedReference;
        public ItemViewModel(Item item, ObservableCollection<Item> items)
        {
            ItemData = item;
            ItemsData = items;
            NewData.Update(item);

            GetData();

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public Item NewData { get; set; } = new Item();
        public Item ItemData { get; private set; }
        public ObservableCollection<Item> ItemsData { get; private set; }
        public ObservableCollection<Reference> ReferencesData { get; private set; }
        public Reference SelectedReference
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
                    }
                    else
                    {
                        NewData.Code =
                            NewData.Description =
                                 NewData.Unit = null;
                    }
                }
            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void GetData()
        {
            ReferencesData = AppData.ReferencesListData;
            if (ReferencesData == null)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                ReferencesData =
                    AppData.ReferencesListData =
                        new ObservableCollection<Reference>(ReferenceController.GetReferences(connection));
            }
        }

        private void Save()
        {
            if (NewData.Id != 0)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Update(NewData);
                }

                ItemData.Update(NewData);
            }
            else
            {
                if (ItemsData != null)
                {
                    ItemsData.Add(NewData);
                }
            }

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (NewData == null)
                return false;

            if (NewData.Code == null)
                return false;

            if (NewData.Qty <= 0)
                return false;

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