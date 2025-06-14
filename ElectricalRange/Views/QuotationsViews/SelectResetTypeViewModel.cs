using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Enums;

using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class SelectResetTypeViewModel : ViewModelBase
    {
        public SelectResetTypeViewModel(ObservableCollection<QItem> items)
        {
            ItemsData = items;

            AllCommand = new RelayCommand(ResetAll, CanResetAll);
            DetailsCommand = new RelayCommand(ResetDetails, CanResetDetails);
            EnclosureCommand = new RelayCommand(ResetEnclosure, CanResetEnclosure);
            AccessoriesCommand = new RelayCommand(ResetAccessories, CanResetAccessories);
        }

        public ObservableCollection<QItem> ItemsData { get; }
        public RelayCommand AllCommand { get; }
        public RelayCommand DetailsCommand { get; }
        public RelayCommand EnclosureCommand { get; }
        public RelayCommand AccessoriesCommand { get; }

        private void ResetAll()
        {
            MessageBoxResult result = MessageView.Show($"Reset",
                                                       $"Are you sure you want to reset all items?",
                                                       MessageViewButton.YesNo, MessageViewImage.Question);

            if (result == MessageBoxResult.No)
                return;

            var affectedItems = new ObservableCollection<QItem>(ItemsData);
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                connection.Delete(affectedItems);
            }

            foreach (QItem item in affectedItems)
            {
                ItemsData.Remove(item);
            }

            Navigation.ClosePopup();
        }
        private bool CanResetAll()
        {
            return true;
        }

        private void ResetDetails()
        {
            MessageBoxResult result = MessageView.Show($"Reset",
                                                       $"Are you sure you want to reset details items?",
                                                       MessageViewButton.YesNo, MessageViewImage.Question);

            if (result == MessageBoxResult.No)
                return;

            var affectedItems = new ObservableCollection<QItem>(ItemsData.Where(x => x.ItemTable == Tables.Details.ToString()));
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                connection.Delete(affectedItems);
            }

            foreach (QItem item in affectedItems)
            {
                ItemsData.Remove(item);
            }

            Navigation.ClosePopup();
        }
        private bool CanResetDetails()
        {
            return true;
        }

        private void ResetEnclosure()
        {
            MessageBoxResult result = MessageView.Show($"Reset",
                                                       $"Are you sure you want to reset enclosure items?",
                                                       MessageViewButton.YesNo, MessageViewImage.Question);

            if (result == MessageBoxResult.No)
                return;

            var affectedItems = new ObservableCollection<QItem>(ItemsData.Where(x => x.ItemTable == Tables.Enclosure.ToString()));
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                connection.Delete(affectedItems);
            }

            foreach (QItem item in affectedItems)
            {
                ItemsData.Remove(item);
            }

            Navigation.ClosePopup();
        }
        private bool CanResetEnclosure()
        {
            return true;
        }

        private void ResetAccessories()
        {
            MessageBoxResult result = MessageView.Show($"Reset",
                                                       $"Are you sure you want to reset accessories items?",
                                                       MessageViewButton.YesNo, MessageViewImage.Question);

            if (result == MessageBoxResult.No)
                return;

            var affectedItems = new ObservableCollection<QItem>(ItemsData.Where(x => x.ItemTable == Tables.Accessories.ToString()));
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                connection.Delete(affectedItems);
            }

            foreach (QItem item in affectedItems)
            {
                ItemsData.Remove(item);
            }

            Navigation.ClosePopup();
        }
        private bool CanResetAccessories()
        {
            return true;
        }
    }
}