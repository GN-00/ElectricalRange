using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.References;
using ProjectsNow.Enums;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class AddItemsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private QItem _SelectedItem;

        public AddItemsViewModel(QPanel panel, ObservableCollection<QItem> items)
        {
            PanelData = panel;
            ItemsData = items;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (ReferencesData == null)
                {
                    ReferencesData =
                        AppData.ReferencesListData =
                            ReferenceController.GetReferences(connection);
                }

                Article1List = ReferenceController.GetArticle1Old(connection);
                Article2List = ReferenceController.GetArticle2Old(connection);
                ItemsTables = new List<string>()
                {
                    Enums.Tables.Details.ToString(),
                    Enums.Tables.Enclosure.ToString(),
                    Enums.Tables.Accessories.ToString(),
                };
            }

            Items = new ObservableCollection<QItem>();
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            DeleteCommand = new RelayCommand<QItem>(Delete, CanDelete);
        }

        public string Indicator
        {
            get => _Indicator;
            set => SetValue(ref _Indicator, value);
        }
        public QItem SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set => SetValue(ref _SelectedIndex, value);
        }

        public QPanel PanelData { get; }
        public Grid ItemsTable { get; }
        public ObservableCollection<QItem> Items { get; }
        public ObservableCollection<QItem> ItemsData { get; }

        public ObservableCollection<string> Article1List { get; set; }
        public ObservableCollection<string> Article2List { get; set; }
        public List<string> ItemsTables { get; set; }
        public ObservableCollection<Reference> ReferencesData { get; }
        public ObservableCollection<Reference> SelectedReferences { get; }
        public RelayCommand<QItem> DeleteCommand { get; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            int indexDetails = 0;
            var ItemsDetailsToUpdate = ItemsData.Where(x => x.ItemTable == Tables.Details.ToString());
            if (ItemsDetailsToUpdate != null)
            {
                indexDetails += ItemsDetailsToUpdate.Count();
            }

            int indexEnclosure = 0;
            var ItemsEnclosureToUpdate = ItemsData.Where(x => x.ItemTable == Tables.Enclosure.ToString());
            if (ItemsEnclosureToUpdate != null)
            {
                indexEnclosure += ItemsEnclosureToUpdate.Count();
            }

            int indexAccessories = 0;
            var ItemsAccessoriesToUpdate = ItemsData.Where(x => x.ItemTable == Tables.Accessories.ToString());
            if (ItemsEnclosureToUpdate != null)
            {
                indexAccessories += ItemsAccessoriesToUpdate.Count();
            }

            foreach (QItem item in Items)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                if (item.Code == null)
                    continue;

                item.PanelID = PanelData.PanelID;
                item.ItemType = ItemTypes.Standard.ToString();

                if (item.ItemTable == Tables.Details.ToString())
                {
                    item.ItemSort = indexDetails++;
                }
                else if (item.ItemTable == Tables.Enclosure.ToString())
                {
                    item.ItemSort = indexEnclosure++;
                }
                else
                {
                    item.ItemSort = indexAccessories++;
                }

                connection.Insert(item);

                ItemsData.Add(item);
            }

            Navigation.Back();
        }
        private bool CanSave()
        {
            if (SelectedReferences == null)
                return false;

            if (SelectedReferences.All(x => x.Code == null))
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

        private void Delete(QItem item)
        {
            Items.Remove(item);
        }
        private bool CanDelete(QItem item)
        {
            if (item == null)
                return false;

            return true;
        }
    }
}