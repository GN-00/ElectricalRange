using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.References;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;
using ProjectsNow.Windows.ReferencesWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Microsoft.Win32;

namespace ProjectsNow.Views.ReferencesViews
{
    public class ReferencesViewModel : ViewModelBase
    {
        private string _Code;
        private string _Description;
        private string _Unit;
        private string _Cost;
        private string _Discount;
        private string _Brand;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Reference _SelectedItem;
        private ObservableCollection<Reference> _Items;

        private ICollectionView _ItemsView;
        public ReferencesViewModel()
        {
            UserData = Navigation.UserData;
            GetData();

            NewCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Reference>(Edit, CanEdit);
            DeleteCommand = new RelayCommand<Reference>(Delete, CanDelete);
            DiscountCommand = new RelayCommand(UpdateDiscount, CanAccessUpdateDiscount);
            PricesCommand = new RelayCommand(Prices, CanAccessPrices);
            CopperCommand = new RelayCommand(Copper, CanAccessCopper);

            ClosingCommand = new RelayCommand(Closing, CanAccessClosing);
        }

        public User UserData { get; }
        public string Indicator
        {
            get => _Indicator;
            set => SetValue(ref _Indicator, value);
        }
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                if (SetValue(ref _SelectedIndex, value))
                {
                    UpdateIndicator();
                }
            }
        }
        public Reference SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Reference> Items
        {
            get => _Items;
            private set
            {
                if (SetValue(ref _Items, value))
                {
                    CreateCollectionView();
                }
            }
        }
        public ICollectionView ItemsView
        {
            get => _ItemsView;
            set => SetValue(ref _ItemsView, value);
        }

        public RelayCommand NewCommand { get; }
        public RelayCommand<Reference> EditCommand { get; }
        public RelayCommand<Reference> DeleteCommand { get; }
        public RelayCommand DiscountCommand { get; }
        public RelayCommand PricesCommand { get; }
        public RelayCommand CopperCommand { get; }
        public RelayCommand ClosingCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string Code
        {
            get => _Code;
            set
            {
                if (SetValue(ref _Code, value))
                {
                    FilterProperty = nameof(Code);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Description
        {
            get => _Description;
            set
            {
                if (SetValue(ref _Description, value))
                {
                    FilterProperty = nameof(Description);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Unit
        {
            get => _Unit;
            set
            {
                if (SetValue(ref _Unit, value))
                {
                    FilterProperty = nameof(Unit);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Cost
        {
            get => _Cost;
            set
            {
                if (SetValue(ref _Cost, value))
                {
                    FilterProperty = nameof(Cost);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Discount
        {
            get => _Discount;
            set
            {
                if (SetValue(ref _Discount, value))
                {
                    FilterProperty = nameof(UpdateDiscount);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Brand
        {
            get => _Brand;
            set
            {
                if (SetValue(ref _Brand, value))
                {
                    FilterProperty = nameof(Brand);
                    ItemsView.Refresh();
                }
            }
        }

        private bool DataFilter(object item)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = "" + GetType().GetProperty(columnName).GetValue(this);

            if (!value.Contains(checkValue.ToUpper()))
            {
                result = false;
            }

            return result;
        }
        private void DeleteFilter()
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                FilterProperty attribute = (FilterProperty)property.GetCustomAttribute(typeof(FilterProperty));
                if (attribute != null)
                {
                    property.SetValue(this, null);
                }
            }
            ItemsView.Refresh();
        }

        #endregion

        private void GetData()
        {
            Items = AppData.ReferencesListData;
            if (Items == null)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                Items =
                    AppData.ReferencesListData =
                        new ObservableCollection<Reference>(ReferenceController.GetReferences(connection));
            }
        }
        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.Filter = new Predicate<object>(DataFilter);
            ItemsView.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            ItemsView.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsView);
        }


        private void Add()
        {
            Reference reference = new();
            Navigation.OpenPopup(new ReferenceView(reference, Items), PlacementMode.Center, true);
        }
        private bool CanAdd()
        {
            return true;
        }

        private void Edit(Reference reference)
        {
            Navigation.OpenPopup(new ReferenceView(reference, Items), PlacementMode.Center, true);
        }
        private bool CanEdit(Reference reference)
        {
            if (reference == null)
                return false;

            return true;
        }

        private void Delete(Reference reference)
        {
            if (reference.Type == "Smart")
            {
                _ = MessageWindow.Show("Delete", "Can't delete this reference!!");
                return;
            }

            MessageBoxResult result = MessageWindow.Show("Deleting", $"Are you sure to delete:\n{reference.Code}\n{reference.Description} ?!", MessageWindowButton.YesNo, MessageWindowImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Delete(reference);
                }

                _ = Items.Remove(reference);
            }
        }
        private bool CanDelete(Reference reference)
        {
            return CanEdit(reference);
        }

        private void UpdateDiscount()
        {
            CategoriesDiscountsWindow categoriesDiscountsWindow = new();
            _ = categoriesDiscountsWindow.ShowDialog();
        }
        private bool CanAccessUpdateDiscount()
        {
            if (!UserData.ReferencesDiscount)
                return false;

            return true;
        }

        private class ExcelRow
        {
            public string Code { get; set; }
            public decimal Cost { get; set; }
        }
        private void Prices()
        {
            UpdatePricesGuideWindow updatePricesGuideWindow = new();
            updatePricesGuideWindow.ShowDialog();

            Navigation.OpenLoading(Visibility.Visible, "Working....");

            OpenFileDialog path = new() { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm" };
            _ = path.ShowDialog();

            string filePath = $@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={path.FileName};" +
                              $@"Extended Properties='Excel 8.0;HDR=Yes;'";

            try
            {
                DataTable excelData = new();
                using (OleDbConnection connection = new(filePath))
                {
                    connection.Open();
                    OleDbDataAdapter oleAdpt = new("select Code, Cost from [Sheet1$]", connection); //here we read data from sheet1  
                    _ = oleAdpt.Fill(excelData);
                }

                if (excelData.Rows.Count == 0)
                {
                    _ = MessageWindow.Show("Data Error", "No data!", MessageWindowButton.OK, MessageWindowImage.Warning);
                    return;
                }

                List<ExcelRow> excelList = new();
                for (int i = 0; i < excelData.Rows.Count; i++)
                {
                    ExcelRow excelRow = new();
                    excelRow.Code = excelData.Rows[i]["Code"].ToString();

                    excelRow.Cost = Convert.ToDecimal(excelData.Rows[i]["Cost"]);
                    excelList.Add(excelRow);
                }

                int itemsCount = 0;
                string updateTable = "";
                foreach (ExcelRow row in excelList)
                {
                    List<Reference> itemsToUpdate = Items.Where(r => r.Code.Trim() == row.Code.Trim()).ToList();
                    foreach (Reference reference in itemsToUpdate)
                    {
                        itemsCount++;
                        reference.Cost = row.Cost;
                        updateTable += $"Update [Reference].[References] Set Cost = {reference.Cost} Where ReferenceID = {reference.ReferenceID}; ";
                    }
                }

                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Execute(updateTable);
                }

                _ = MessageWindow.Show("Data", $"({itemsCount}) references updated!", MessageWindowButton.OK, MessageWindowImage.Information);
            }
            catch (Exception exception)
            {
                _ = MessageWindow.Show("Error", exception.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
            }

            Navigation.CloseLoading();
        }
        private bool CanAccessPrices()
        {
            return true;
        }

        private void Copper()
        {
            UpdateCopperWindow updateCopperWindow = new(Items);
            updateCopperWindow.ShowDialog();
        }
        private bool CanAccessCopper()
        {
            return true;
        }

        private void Closing()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            AppData.ReferencesListData =
                ReferenceController.GetReferences(connection);
        }
        private bool CanAccessClosing()
        {
            return AppData.ReferencesListData != null;
        }
    }
}