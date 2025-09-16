using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.References;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Views.ReferencesViews
{
    internal class ReferenceViewModel : ViewModelBase
    {
        public ReferenceViewModel(Reference reference, ObservableCollection<Reference> references, IView view)
        {
            ViewData = view;
            NewData = new Reference();
            NewData.Update(reference);
            ReferenceData = reference;
            ReferencesData = references;

            GetData();

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            SupplierCodesCommand = new RelayCommand(SupplierCodes, CanSupplierCodes);
        }

        public Reference NewData { get; private set; }
        public Reference ReferenceData { get; private set; }
        public ObservableCollection<Reference> ReferencesData { get; private set; }
        public ObservableCollection<string> CategoriesData { get; private set; }
        public ObservableCollection<string> ArticlesData1 { get; private set; }
        public ObservableCollection<string> ArticlesData2 { get; private set; }
        public ObservableCollection<string> BrandsData { get; private set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand SupplierCodesCommand { get; }

        public void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = "Select Brand From [Reference].[References] " +
                    "where Brand Is Not Null Group By Brand Order By Brand";
            BrandsData = new ObservableCollection<string>(connection.Query<string>(query));

            query = "Select Category From [Reference].[References] " +
                    "where Category Is Not Null Group By Category Order By Category";
            CategoriesData = new ObservableCollection<string>(connection.Query<string>(query));

            ArticlesData1 = ReferenceController.GetArticle1(connection);
            ArticlesData2 = ReferenceController.GetArticle2(connection);
        }

        private void Save()
        {
            bool isReady = true;
            string message = "Please Enter:";
            if (string.IsNullOrWhiteSpace(NewData.Code)) { message += $"\n  Code."; isReady = false; }
            if (string.IsNullOrWhiteSpace(NewData.Description)) { message += $"\n  Description."; isReady = false; }
            if (string.IsNullOrWhiteSpace(NewData.Category)) { message += $"\n  Category."; isReady = false; }
            if (string.IsNullOrWhiteSpace(NewData.Brand)) { message += $"\n  Brand."; isReady = false; }
            if (string.IsNullOrWhiteSpace(NewData.Unit)) { message += $"\n  Unit."; isReady = false; }

            if (isReady)
            {
                List<string> keys = null;
                Reference checkReference;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    if (!string.IsNullOrWhiteSpace(NewData.SearchKeys))
                        keys = NewData.SearchKeys.Split(',').ToList();

                    string query = $"Select * From [Reference].[References]" +
                                   $"Where ReferenceID <> {NewData.Id} And Code = '{NewData.Code}'";
                    checkReference = connection.QueryFirstOrDefault(query);
                }

                if (checkReference != null)
                {
                    _ = MessageView.Show("Saving", "This code already exsist!", MessageViewButton.OK, MessageViewImage.Information);
                    return;
                }

                if (NewData.Id == 0)
                {
                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        _ = connection.Insert(NewData);
                    }

                    ReferencesData.Add(NewData);
                }
                else
                {
                    using (SqlConnection connection = new(Database.ConnectionString))
                    {
                        _ = connection.Update(NewData);
                    }

                    ReferenceData.Update(NewData);
                }

                if (keys != null)
                    UpdateKeys(keys);

                Navigation.ClosePopup();
            }
            else
            {
                _ = MessageView.Show("Saving", message, MessageViewButton.OK, MessageViewImage.Information);
            }
        }

        private void UpdateKeys(List<string> keys)
        {
            string query;
            string checkValue;
            SearchKey searchKey;
            using SqlConnection connection = new(Database.ConnectionString);
            foreach (string key in keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                string value = key.Trim();

                query = $"Select [Key] From [Reference].[SearchKeys] Where [Key] = '{value}'";
                checkValue = connection.QueryFirstOrDefault<string>(query);

                if (checkValue == null)
                {
                    searchKey = new SearchKey() { Key = value };
                    _ = connection.Insert(searchKey);
                }
            }
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

        private void SupplierCodes()
        {
            Navigation.To(new SupplierCodesView(ReferenceData), ViewData);
        }
        private bool CanSupplierCodes()
        {
            return true;
        }
    }
}