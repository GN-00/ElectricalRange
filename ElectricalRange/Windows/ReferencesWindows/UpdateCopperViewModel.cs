using Dapper.Contrib.Extensions;

using ProjectsNow.Data;
using ProjectsNow.Data.References;
using ProjectsNow.Commands;
using ProjectsNow.Windows.MessageWindows;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ProjectsNow.Windows.ReferencesWindows
{
    internal class UpdateCopperViewModel : ViewModelBase
    {
        private string _NewData;

        public UpdateCopperViewModel(ObservableCollection<Reference> items, Window window)
        {
            WindowData = window;
            Items = items;
            CopperItems = items.Where(i => i.Category == "Busbar" && i.Unit == "Kg").ToList();

            if (CopperItems.Count != 0)
            {
                CopperPrice = CopperItems.Max(i => i.Cost);
            }

            NewData = CopperPrice.ToString();
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }
        public string NewData
        {
            get => _NewData;
            set => SetValue(ref _NewData, value);
        }
        public decimal CopperPrice { get; set; } = 0;
        public List<Reference> CopperItems { get; private set; }
        public ObservableCollection<Reference> Items { get; private set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            decimal.TryParse(NewData, out decimal value);
            if (value > 0)
            {
                foreach (var item in Items.Where(i => i.Category == "Busbar" && i.Unit == "Kg"))
                {
                    item.Cost = value;
                }

                using (SqlConnection connection = new(Data.Database.ConnectionString))
                {
                    _ = connection.Update(Items.Where(i => i.Category == "Busbar" && i.Unit == "Kg"));
                }

                WindowData.Close();
            }
            else
            {
                MessageWindow.Show("Error", "Please enter correct value!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }
        private bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(NewData))
                return false;

            return true;
        }

        private void Cancel()
        {
            WindowData.Close();
        }
    }
}