using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows.Data;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class DiscountsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Category _SelectedItem;
        private ObservableCollection<Category> _Items;
        private ICollectionView _ItemsCollection;

        public DiscountsViewModel(Quotation quotation)
        {
            QuotationData = quotation;
            GetData();
            SaveCommand = new RelayCommand<Category>(Save, CanSave);
            EditCommand = new RelayCommand<Category>(Edit, CanEdit);
        }

        public User UserData => Navigation.UserData;
        public Quotation QuotationData { get; }

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
        public Category SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Category> Items
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
        public ObservableCollection<Category> CategorysData { get; }

        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [Quotation].[CategoriesDiscounts] " +
                           $"Where QuotationID = {QuotationData.QuotationID}";

            Items = new ObservableCollection<Category>(connection.Query<Category>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
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

        public RelayCommand<Category> SaveCommand { get; }
        public RelayCommand<Category> EditCommand { get; }

        private void Save(Category category)
        {
            category.IsEnabled = false;

            string query;
            decimal discount = category.Discount;
            if (discount > -1)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                query = $"Update [Quotation].[QuotationsPanelsItems] Set " +
                        $"ItemDiscount = {discount} " +
                        $"Where PanelID in " +
                        $"(Select PanelID From [Quotation].[QuotationsPanels] Where QuotationID = {QuotationData.Id}) " +
                        $"And ItemDiscount = {category.OldDiscount} " +
                        $"And Category = '{category.Name}'";

                _ = connection.Execute(query);

                query = $"Select * From [Quotation].[Quotations(View)] " +
                        $"Where QuotationID = {QuotationData.QuotationID}";

                Quotation newData = connection.QueryFirstOrDefault<Quotation>(query);

                QuotationData.QuotationCost = newData.QuotationCost;
                QuotationData.QuotationPrice = newData.QuotationPrice;
                QuotationData.QuotationEstimatedPrice = newData.QuotationEstimatedPrice;
                QuotationData.QuotationFinalPrice = newData.QuotationFinalPrice;
                QuotationData.QuotationDiscountValue = newData.QuotationDiscountValue;
                QuotationData.QuotationVATValue = newData.QuotationVATValue;
            }
        }
        private bool CanSave(Category category)
        {
            if (category == null)
                return false;

            if (!category.IsEnabled)
                return false;

            return true;
        }

        private void Edit(Category category)
        {
            category.IsEnabled = true;
        }
        private bool CanEdit(Category category)
        {
            if (category == null)
                return false;

            if (category.IsEnabled)
                return false;

            return true;
        }

    }
}