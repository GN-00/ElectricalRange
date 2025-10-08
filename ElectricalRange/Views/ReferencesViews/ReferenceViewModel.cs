using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.References;

using System.Collections.ObjectModel;

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
        public ObservableCollection<Data.References.Category> CategoriesData { get; private set; }
        public ObservableCollection<Article1> ArticlesData1 { get; private set; }
        public ObservableCollection<Article2> ArticlesData2 { get; private set; }
        public ObservableCollection<Brand> BrandsData { get; private set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand SupplierCodesCommand { get; }

        public void GetData()
        {
            string query;
            SqlConnection connection = new(Database.ConnectionString);
            if (AppData.BrandsData == null)
            {
                query = "Select Name From [Reference].[Brands] " +
                        "Order By Name";
                AppData.BrandsData = new ObservableCollection<Data.References.Brand>(connection.Query<Data.References.Brand>(query));
            }

            if (AppData.CategoriesData == null)
            {
                query = "Select Name From [Reference].[Categories] " +
                        "Order By Name";
                AppData.CategoriesData = new ObservableCollection<Data.References.Category>(connection.Query<Data.References.Category>(query));
            }

            AppData.Articles1Data ??= ReferenceController.GetArticle1(connection);

            AppData.Articles2Data ??= ReferenceController.GetArticle2(connection);

            BrandsData = AppData.BrandsData;
            CategoriesData = AppData.CategoriesData;
            ArticlesData1 = AppData.Articles1Data;
            ArticlesData2 = AppData.Articles2Data;
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

                Navigation.Back();
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
            Navigation.Back();
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