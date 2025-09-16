using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.References;
using ProjectsNow.Data.Users;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Views.ReferencesViews
{
    public class SupplierCodesViewModel : ViewModelBase
    {
        private bool _IsWriting = false;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private SupplierReference _NewData = new();
        private SupplierReference _SelectedItem;
        private ObservableCollection<SupplierReference> _Items;
        private string _SupplierCode;
        private ICollectionView _ItemsCollection;

        public SupplierCodesViewModel(Reference reference, IView view)
        {
            LoadingText = "Updating....";
            UserData = Navigation.UserData;
            ReferenceData = reference;
            GetData();
            if (Items.Count != 0)
            {
                SelectedItem = Items[0];
            }
            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<SupplierReference>(Edit, CanEdit);
            DeleteCommand = new RelayCommand<SupplierReference>(Delete, CanDelete);
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData { get; }
        public Reference ReferenceData { get; private set; }
        public bool IsWriting
        {
            get => _IsWriting;
            set => SetValue(ref _IsWriting, value);
        }
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
        public SupplierReference NewData
        {
            get => _NewData;
            set => SetValue(ref _NewData, value);
        }
        public SupplierReference SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (SetValue(ref _SelectedItem, value))
                {
                    if (SelectedItem != null)
                        NewData.Update(SelectedItem);
                }
            }
        }
        public ObservableCollection<SupplierReference> Items
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
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }
        public RelayCommand AddCommand { get; }
        public RelayCommand<SupplierReference> EditCommand { get; }
        public RelayCommand<SupplierReference> DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string SupplierCode
        {
            get => _SupplierCode;
            set
            {
                if (SetValue(ref _SupplierCode, value))
                {
                    FilterProperty = nameof(SupplierCode);
                    ItemsCollection.Refresh();
                }
            }
        }

        private bool DataFilter(object code)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{code.GetType().GetProperty(columnName).GetValue(code)}".ToUpper();
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
            ItemsCollection.Refresh();
        }

        #endregion

        private void GetData()
        {
            string query = $"Select * From [Reference].[SuppliersCodes] " +
                           $"Where Code = '{ReferenceData.Code}' " +
                           $"Order By SupplierCode ";
            using SqlConnection connection = new(Database.ConnectionString);
            Items = new ObservableCollection<SupplierReference>(connection.Query<SupplierReference>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("SupplierCode", ListSortDirection.Ascending));
            ItemsCollection.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }

        private void Add()
        {
            NewData = new SupplierReference() { Code = ReferenceData.Code };
            IsWriting = true;
        }
        private bool CanAdd()
        {
            if (IsWriting)
                return false;

            return true;
        }

        private void Edit(SupplierReference code)
        {
            IsWriting = true;
        }
        private bool CanEdit(SupplierReference code)
        {
            if (code == null)
                return false;

            if (IsWriting)
                return false;

            return true;
        }

        private void Delete(SupplierReference code)
        {
            MessageBoxResult result =
                MessageView.Show($"Delete",
                                 $"Are you sure want to Delete\n{code.SupplierCode}?",
                                 MessageViewButton.YesNo,
                                 MessageViewImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Data.Database.ConnectionString))
                {
                    _ = connection.Delete(code);
                }
                Items.Remove(code);

                if (Items.Count > 0)
                {
                    SelectedItem = Items[0];
                }
            }
        }
        private bool CanDelete(SupplierReference code)
        {
            return CanEdit(code);
        }

        private void Save()
        {
            bool isReady = true;
            string message = "Please Enter:";
            if (string.IsNullOrWhiteSpace(NewData.SupplierCode)) { message += $"\n  Code."; isReady = false; }
            else if (Items.Any(i => i.SupplierCode.ToLower() == NewData.SupplierCode.ToLower() && i.Id != NewData.Id))
            { message += $"\n  Supplier Code is already exist."; isReady = false; }

            if (isReady)
            {
                if (NewData.Id == 0)
                {
                    SupplierReference code = new();
                    code.Update(NewData);
                    using (SqlConnection connection = new(Data.Database.ConnectionString))
                    {
                        _ = connection.Insert(code);
                    }

                    Items.Add(code);

                    SelectedItem = code;
                }
                else
                {
                    using (SqlConnection connection = new(Data.Database.ConnectionString))
                    {
                        _ = connection.Update(NewData);
                    }

                    SelectedItem.Update(NewData);

                    ItemsCollection.Refresh();
                }

                IsWriting = false;
            }
            else
            {
                _ = MessageView.Show("Saving", message, MessageViewButton.OK, MessageViewImage.Information);
            }
        }
        private bool CanSave()
        {
            return IsWriting;
        }

        private void Cancel()
        {
            NewData.Update(SelectedItem);
            IsWriting = false;
        }
        private bool CanCancel()
        {
            return IsWriting;
        }
    }
}