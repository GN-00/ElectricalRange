using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Data.Users;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    public class ItemsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Item _SelectedItem;
        private ObservableCollection<Item> _Items;

        private string _DetailsCost;
        private string _EnclosureCost;
        private string _AccessoriesCost;
        private string _CopperCost;

        private string _Article1;
        private string _Article2;
        private string _Code;
        private string _Description;
        private string _ItemQty;
        private string _Brand;
        private string _ItemDiscount;
        private string _ItemTable;

        private ICollectionView _ItemsCollection;

        public ItemsViewModel(ProductionPanel panel, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            PanelData = panel;

            GetData();
            AddItemsCommand = new RelayCommand(AddItems, CanAccessAddItems);
            UpdateCommand = new RelayCommand(GetData);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData { get; }
        public ProductionPanel PanelData { get; }

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
        public Item SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Item> Items
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


        public RelayCommand AddItemsCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string Article1
        {
            get => _Article1;
            set
            {
                if (SetValue(ref _Article1, value))
                {
                    FilterProperty = nameof(Article1);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Article2
        {
            get => _Article2;
            set
            {
                if (SetValue(ref _Article2, value))
                {
                    FilterProperty = nameof(Article2);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Code
        {
            get => _Code;
            set
            {
                if (SetValue(ref _Code, value))
                {
                    FilterProperty = nameof(Code);
                    ItemsCollection.Refresh();
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
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string ItemQty
        {
            get => _ItemQty;
            set
            {
                if (SetValue(ref _ItemQty, value))
                {
                    FilterProperty = nameof(ItemQty);
                    ItemsCollection.Refresh();
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
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string ItemDiscount
        {
            get => _ItemDiscount;
            set
            {
                if (SetValue(ref _ItemDiscount, value))
                {
                    FilterProperty = nameof(ItemDiscount);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string ItemTable
        {
            get => _ItemTable;
            set
            {
                if (SetValue(ref _ItemTable, value))
                {
                    FilterProperty = nameof(ItemTable);
                    ItemsCollection.Refresh();
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
                if (property.Name == nameof(ItemTable))
                    continue;

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
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [Production].[PanelsItems(View)] " +
                           $"Where PanelId = {PanelData.PanelId} " +
                           $"And JobOrderId = {PanelData.JobOrderId}" +
                           $"Order By Code";

            Items = new ObservableCollection<Item>(connection.Query<Item>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
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

        private void AddItems()
        {
            Services.ProductionServices.AddItems(PanelData);
            GetData();
        }
        private bool CanAccessAddItems()
        {
            return true;
        }

    }
}