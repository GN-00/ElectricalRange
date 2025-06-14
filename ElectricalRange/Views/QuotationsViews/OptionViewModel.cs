using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

using Option = ProjectsNow.Data.Quotations.QuotationOption;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class OptionViewModel : ViewModelBase
    {
        public OptionViewModel(Option option, ObservableCollection<Option> options)
        {
            OptionData = option;
            OptionsData = options;
            NewData.Update(OptionData);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public Option NewData { get; } = new Option();
        public Option OptionData { get; }
        public ObservableCollection<Option> OptionsData { get; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (NewData.ID == 0) //New
                {
                    connection.Insert(NewData);
                    OptionsData.Add(NewData);
                }
                else //Edit
                {
                    connection.Update(NewData);
                    OptionData.Update(NewData);
                }
            }

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(NewData.Name))
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