using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.References;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Views.ReferencesViews;
using ProjectsNow.Windows.ReferencesWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.QuotationsViews
{
    public class ItemsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private QItem _SelectedItem;
        private ObservableCollection<QItem> _Items;

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

        public ItemsViewModel(Quotation quotation, QPanel panel, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            PanelData = panel;
            QuotationData = quotation;

            GetData();
            ItemTable = Tables.Details.ToString();

            DetailsCommand = new RelayCommand(Details, CanAccessDetails);
            EnclosureCommand = new RelayCommand(Enclosure, CanAccessEnclosure);
            AccessoriesCommand = new RelayCommand(Accessories, CanAccessAccessories);

            ToDetailsCommand = new RelayCommand<QItem>(ToDetails, CanAccessToDetails);
            ToEnclosureCommand = new RelayCommand<QItem>(ToEnclosure, CanAccessToEnclosure);
            ToAccessoriesCommand = new RelayCommand<QItem>(ToAccessories, CanAccessToAccessories);

            CostSheetCommand = new RelayCommand(CostSheet, CanAccessCostSheet);

            AddCommand = new RelayCommand(Add, CanAccessAdd);
            AddStandardCommand = new RelayCommand(AddStandard, CanAccessAddStandard);
            AddCustomCommand = new RelayCommand(AddCustom, CanAccessAddCustom);
            AddItemsCommand = new RelayCommand(AddItems, CanAccessAddItems);
            EditCommand = new RelayCommand<QItem>(Edit, CanAccessEdit);
            InsertUpCommand = new RelayCommand<QItem>(InsertUp, CanAccessInsertUp);
            InsertDownCommand = new RelayCommand<QItem>(InsertDown, CanAccessInsertDown);
            MoveUpCommand = new RelayCommand<QItem>(MoveUp, CanAccessMoveUp);
            MoveDownCommand = new RelayCommand<QItem>(MoveDown, CanAccessMoveDown);
            CopyCommand = new RelayCommand<QItem>(Copy, CanAccessCopy);
            DeleteCommand = new RelayCommand<QItem>(Delete, CanAccessDelete);
            ResetCommand = new RelayCommand(Reset, CanAccessReset);

            ReferencesCommand = new RelayCommand(References, CanAccessReferences);
            DigitalLibraryCommand = new RelayCommand(DigitalLibrary, CanAccessDigitalLibrary);
            AddEnclosureCommand = new RelayCommand(AddEnclosure, CanAccessAddEnclosure);
            DeleteEnclosureCommand = new RelayCommand(DeleteEnclosure, CanAccessDeleteEnclosure);

            UpdateCommand = new RelayCommand(UpdateValues);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public Actions ActionData { get; private set; }
        public User UserData { get; }
        public QPanel PanelData { get; }
        public Quotation QuotationData { get; }

        public Visibility HasToolsButtons
        {
            get
            {
                if (SelectedItem == null)
                    return Visibility.Collapsed;

                if (CanAccessAdd())
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }
        public Visibility HasInfoButtons
        {
            get
            {
                if (SelectedItem == null)
                    return Visibility.Collapsed;

                if (CanAccessEdit(SelectedItem))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }
        public Visibility HasMovingButtons
        {
            get
            {
                if (SelectedItem == null)
                    return Visibility.Collapsed;

                if (CanAccessMoveUp(SelectedItem))
                    return Visibility.Visible;

                if (CanAccessMoveDown(SelectedItem))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
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
        public QItem SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value)
                  .UpdateProperties(this,
                                    nameof(HasInfoButtons),
                                    nameof(HasMovingButtons),
                                    nameof(HasToolsButtons));
        }
        public ObservableCollection<QItem> Items
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
        public ObservableCollection<Reference> ReferencesData { get; set; }

        public string DetailsCost
        {
            get => _DetailsCost;
            set => SetValue(ref _DetailsCost, value);
        }
        public string EnclosureCost
        {
            get => _EnclosureCost;
            set => SetValue(ref _EnclosureCost, value);
        }
        public string AccessoriesCost
        {
            get => _AccessoriesCost;
            set => SetValue(ref _AccessoriesCost, value);
        }
        public string CopperCost
        {
            get => _CopperCost;
            set => SetValue(ref _CopperCost, value);
        }

        public RelayCommand DetailsCommand { get; }
        public RelayCommand EnclosureCommand { get; }
        public RelayCommand AccessoriesCommand { get; }

        public RelayCommand<QItem> ToDetailsCommand { get; }
        public RelayCommand<QItem> ToEnclosureCommand { get; }
        public RelayCommand<QItem> ToAccessoriesCommand { get; }

        public RelayCommand CostSheetCommand { get; }

        public RelayCommand AddCommand { get; }
        public RelayCommand AddStandardCommand { get; }
        public RelayCommand AddCustomCommand { get; }
        public RelayCommand AddItemsCommand { get; }
        public RelayCommand<QItem> EditCommand { get; }
        public RelayCommand<QItem> InsertUpCommand { get; }
        public RelayCommand<QItem> InsertDownCommand { get; }
        public RelayCommand<QItem> MoveUpCommand { get; }
        public RelayCommand<QItem> MoveDownCommand { get; }
        public RelayCommand<QItem> CopyCommand { get; }
        public RelayCommand<QItem> DeleteCommand { get; }
        public RelayCommand ResetCommand { get; }
        public RelayCommand ReferencesCommand { get; }
        public RelayCommand DigitalLibraryCommand { get; }
        public RelayCommand AddEnclosureCommand { get; }
        public RelayCommand DeleteEnclosureCommand { get; }

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
            string query = $"Select * From [Quotation].[QuotationsPanelsItems] " +
                           $"Where PanelID = {PanelData.Id} " +
                           $"Order By ItemTable, ItemSort";

            Items = new ObservableCollection<QItem>(connection.Query<QItem>(query));

            int i = 0;
            foreach (QItem item in Items.Where(x => x.ItemTable == Tables.Details.ToString()))
            {
                item.ItemSort = i++;
            }

            i = 0;
            foreach (QItem item in Items.Where(x => x.ItemTable == Tables.Enclosure.ToString()))
            {
                item.ItemSort = i++;
            }

            i = 0;
            foreach (QItem item in Items.Where(x => x.ItemTable == Tables.Accessories.ToString()))
            {
                item.ItemSort = i++;
            }

            _ = connection.Update(Items);
        }
        private void UpdateValues()
        {
            PanelData.PanelCost =
                    Items.Sum(item => item.ItemCost * item.ItemQty * (1m - item.ItemDiscount / 100m));

            decimal total = Items.Sum(item => item.ItemCost * item.ItemQty * (1m - item.ItemDiscount / 100m));

            decimal detailsCost = Items.Where(i => i.Article1 != "COPPER" && i.ItemTable == Tables.Details.ToString()).Sum(item => item.ItemCost * item.ItemQty * (1m - item.ItemDiscount / 100m));
            decimal enclosureCost = Items.Where(i => i.ItemTable == Tables.Enclosure.ToString()).Sum(item => item.ItemCost * item.ItemQty * (1m - item.ItemDiscount / 100m));
            decimal accessoriesCost = Items.Where(i => i.ItemTable == Tables.Accessories.ToString()).Sum(item => item.ItemCost * item.ItemQty * (1m - item.ItemDiscount / 100m));
            decimal copperCost = Items.Where(i => i.Article1 == "COPPER" && i.ItemTable == Tables.Details.ToString()).Sum(item => item.ItemCost * item.ItemQty * (1m - item.ItemDiscount / 100m));

            if (total != 0)
            {
                DetailsCost = $"{detailsCost:N2} ({detailsCost / total * 100m:N2} %)";
                EnclosureCost = $"{enclosureCost:N2} ({enclosureCost / total * 100m:N2} %)";
                AccessoriesCost = $"{accessoriesCost:N2} ({accessoriesCost / total * 100m:N2} %)";
                CopperCost = $"{copperCost:N2} ({copperCost / total * 100m:N2} %)";
            }
        }

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("ItemSort", ListSortDirection.Ascending));
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

        private void Details()
        {
            ItemTable = Tables.Details.ToString();
            ItemsCollection.Refresh();
        }
        private bool CanAccessDetails()
        {
            if (ItemTable == Tables.Details.ToString())
                return false;

            return true;
        }

        private void Enclosure()
        {
            ItemTable = Tables.Enclosure.ToString();
            ItemsCollection.Refresh();
        }
        private bool CanAccessEnclosure()
        {
            if (ItemTable == Tables.Enclosure.ToString())
                return false;

            return true;
        }

        private void Accessories()
        {
            ItemTable = Tables.Accessories.ToString();
            ItemsCollection.Refresh();
        }
        private bool CanAccessAccessories()
        {
            if (ItemTable == Tables.Accessories.ToString())
                return false;

            return true;
        }

        private void ToDetails(QItem item)
        {
            IEnumerable<QItem> affectedItems = Items.Where(x => x.ItemSort > item.ItemSort && x.ItemTable == this.ItemTable);
            foreach (QItem itemData in affectedItems)
            {
                --itemData.ItemSort;
            }

            item.ItemTable = Tables.Details.ToString();
            item.ItemSort = Items.Count(x => x.ItemTable == Tables.Details.ToString());
            ItemsCollection.Refresh();

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(item);
            _ = connection.Update(affectedItems);
        }
        private bool CanAccessToDetails(QItem item)
        {
            if (item == null)
                return false;

            if (item.ItemTable == Tables.Details.ToString())
                return false;

            if (Items.Count == 0)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void ToEnclosure(QItem item)
        {
            IEnumerable<QItem> affectedItems = Items.Where(x => x.ItemSort > item.ItemSort && x.ItemTable == this.ItemTable);
            foreach (QItem itemData in affectedItems)
            {
                --itemData.ItemSort;
            }

            item.ItemTable = Tables.Enclosure.ToString();
            item.ItemSort = Items.Count(x => x.ItemTable == Tables.Enclosure.ToString());
            ItemsCollection.Refresh();

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(item);
            _ = connection.Update(affectedItems);
        }
        private bool CanAccessToEnclosure(QItem item)
        {
            if (item == null)
                return false;

            if (item.ItemTable == Tables.Enclosure.ToString())
                return false;

            if (Items.Count == 0)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void ToAccessories(QItem item)
        {
            IEnumerable<QItem> affectedItems = Items.Where(x => x.ItemSort > item.ItemSort && x.ItemTable == this.ItemTable);
            foreach (QItem itemData in affectedItems)
            {
                --itemData.ItemSort;
            }

            item.ItemTable = Tables.Accessories.ToString();
            item.ItemSort = Items.Count(x => x.ItemTable == Tables.Accessories.ToString());
            ItemsCollection.Refresh();

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(item);
            _ = connection.Update(affectedItems);
        }
        private bool CanAccessToAccessories(QItem item)
        {
            if (item == null)
                return false;

            if (item.ItemTable == Tables.Accessories.ToString())
                return false;

            if (Items.Count == 0)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void CostSheet()
        {
            Navigation.OpenPopup(new SelectCostSheetView(QuotationData, PanelData, ViewData), PlacementMode.MousePoint, false);
        }
        private bool CanAccessCostSheet()
        {
            return true;
        }
        
        private void References()
        {
            Navigation.To(new ReferencesView(), ViewData);
        }
        private bool CanAccessReferences()
        {
            if (!UserData.AccessReferences)
                return false;

            return true;
        }

        private void DigitalLibrary()
        {
            Navigation.OpenPopup(new Estimator.Views.SelectGroupView(PanelData.Id, Items), PlacementMode.Center, true);
        }
        private bool CanAccessDigitalLibrary()
        {
            //if (!UserData.AccessReferences)
            //    return false;

            return true;
        }

        private void AddEnclosure()
        {
            EnclosuresWindow enclosuresWindow = new()
            {
                UserData = UserData,
                PanelData = PanelData,
                Items = Items,
            };
            _ = enclosuresWindow.ShowDialog();

            UpdateValues();
            ItemsCollection.Refresh();
        }
        private bool CanAccessAddEnclosure()
        {
            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void DeleteEnclosure()
        {
            MessageBoxResult result = MessageView.Show($"Delete",
                                                       $"Are you sure you want to delete the enclosure!!",
                                                       MessageViewButton.YesNo, MessageViewImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                List<QItem> items = new();
                items.AddRange(Items.Where(i => i.SelectionGroup == SelectionGroups.SmartEnclosure.ToString()));
                int detailsCount = items.Count(i => i.ItemTable == Tables.Details.ToString());
                int enclosureCount = items.Count(i => i.ItemTable == Tables.Enclosure.ToString());

                int itemsCounter = items.Count;
                if (itemsCounter != 0)
                {
                    using SqlConnection connection = new(Database.ConnectionString);
                    string query;
                    query = $"Delete From [Quotation].[QuotationsPanelsItems] Where PanelID = {PanelData.PanelID} And SelectionGroup = '{SelectionGroups.SmartEnclosure}'";
                    query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = (ItemSort - {detailsCount}) Where (PanelID = {PanelData.PanelID}) And (ItemTable = '{Tables.Details}'); ";
                    query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = (ItemSort - {enclosureCount}) Where (PanelID = {PanelData.PanelID}) And (ItemTable = '{Tables.Enclosure}'); ";
                    _ = connection.Execute(query);

                    PanelData.EnclosureType = null;
                    PanelData.EnclosureHeight = null;
                    PanelData.EnclosureWidth = null;
                    PanelData.EnclosureDepth = null;

                    PanelData.EnclosureMetalType = null;
                    PanelData.EnclosureColor = null;
                    PanelData.EnclosureIP = null;
                    PanelData.EnclosureForm = null;
                    PanelData.EnclosureLocation = null;
                    PanelData.EnclosureInstallation = null;
                    PanelData.EnclosureFunctional = null;

                    PanelData.EnclosureName = null;
                    _ = connection.Update(PanelData);


                    foreach (QItem item in items)
                    {
                        _ = Items.Remove(item);
                    }

                    int i = 0;
                    foreach (QItem item in Items.Where(x => x.ItemTable == Tables.Details.ToString()))
                    {
                        item.ItemSort = i++;
                    }

                    i = 0;
                    foreach (QItem item in Items.Where(x => x.ItemTable == Tables.Enclosure.ToString()))
                    {
                        item.ItemSort = i++;
                    }

                    _ = connection.Update(Items);
                }

                UpdateValues();
            }
        }
        private bool CanAccessDeleteEnclosure()
        {
            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void Add()
        {
            ActionData = Actions.New;
            QItem item = new()
            {
                PanelID = PanelData.Id,
                ItemSort = Items.Count(x => x.ItemTable == ItemTable),
                ItemTable = ItemTable,
            };
            Navigation.OpenPopup(new SelectItemTypeView(item, Items, this), PlacementMode.MousePoint, false);

            Navigation.ClosePopupEvent += UpdateValues;
        }
        private bool CanAccessAdd()
        {
            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void AddStandard()
        {
            QItem item = new()
            {
                PanelID = PanelData.Id,
                ItemSort = Items.Count(x => x.ItemTable == ItemTable),
                ItemTable = ItemTable,
            };
            Navigation.OpenPopup(new AddStandardItemView(item, Items), PlacementMode.Center, true);
            Navigation.ClosePopupEvent += UpdateValues;
        }
        private bool CanAccessAddStandard()
        {
            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void AddCustom()
        {
            QItem item = new()
            {
                PanelID = PanelData.Id,
                ItemSort = Items.Count(x => x.ItemTable == ItemTable),
                ItemTable = ItemTable,
            };
            Navigation.OpenPopup(new AddCustomItemView(item, Items), PlacementMode.Center, true);
            Navigation.ClosePopupEvent += UpdateValues;
        }
        private bool CanAccessAddCustom()
        {
            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void AddItems()
        {
            Navigation.To(new AddItemsView(PanelData, Items), ViewData);
            UpdateValues();
        }
        private bool CanAccessAddItems()
        {
            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void Edit(QItem item)
        {
            if (item.ItemType == ItemTypes.NewItem.ToString())
            {
                Navigation.OpenPopup(new AddCustomItemView(item, Items), PlacementMode.Center, true);
                Navigation.ClosePopupEvent += UpdateValues;
            }
            else
            {
                Navigation.OpenPopup(new AddStandardItemView(item, Items), PlacementMode.Center, true);
                Navigation.ClosePopupEvent += UpdateValues;
            }
        }
        private bool CanAccessEdit(QItem item)
        {
            if (item == null)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void InsertUp(QItem item)
        {
            QItem newItem = new()
            {
                PanelID = PanelData.Id,
                ItemSort = item.ItemSort,
                ItemTable = ItemTable,
            };
            Navigation.OpenPopup(new SelectItemTypeView(newItem, Items, this), PlacementMode.MousePoint, false);

            Navigation.ClosePopupEvent += UpdateValues;
        }
        private bool CanAccessInsertUp(QItem item)
        {
            if (item == null)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void InsertDown(QItem item)
        {
            QItem newItem = new()
            {
                PanelID = PanelData.Id,
                ItemSort = item.ItemSort + 1,
                ItemTable = ItemTable,
            };
            Navigation.OpenPopup(new SelectItemTypeView(newItem, Items, this), PlacementMode.MousePoint, false);

            Navigation.ClosePopupEvent += UpdateValues;
        }
        private bool CanAccessInsertDown(QItem item)
        {
            if (item == null)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void MoveUp(QItem item)
        {
            item.ItemSort -= 1;

            QItem affecteditem = Items.FirstOrDefault(x => x.ItemSort == item.ItemSort
                                                        && x.ItemID != item.ItemID
                                                        && x.ItemTable == ItemTable);
            affecteditem.ItemSort++;

            ItemsCollection.Refresh();

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(item);
            _ = connection.Update(affecteditem);
        }
        private bool CanAccessMoveUp(QItem item)
        {
            if (item == null)
                return false;

            if (ItemsCollection.Cast<QItem>().ToList().IndexOf(item) == 0)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void MoveDown(QItem item)
        {
            item.ItemSort += 1;

            QItem affecteditem = Items.FirstOrDefault(x => x.ItemSort == item.ItemSort
                                                        && x.ItemID != item.ItemID
                                                        && x.ItemTable == ItemTable);
            affecteditem.ItemSort--;

            ItemsCollection.Refresh();

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(item);
            _ = connection.Update(affecteditem);
        }
        private bool CanAccessMoveDown(QItem item)
        {
            if (item == null)
                return false;

            if (ItemsCollection.Cast<QItem>().ToList().IndexOf(item) == ItemsCollection.Cast<QItem>().Count() - 1)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void Copy(QItem item)
        {
            QItem newItem = new();
            newItem.Update(item);
            newItem.ItemSort = Items.Count(x => x.ItemTable == ItemTable);

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Insert(newItem);
            }

            Items.Add(newItem);

            UpdateValues();
        }
        private bool CanAccessCopy(QItem item)
        {
            if (item == null)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void Delete(QItem item)
        {
            MessageBoxResult result = MessageView.Show($"Deleting",
                                                       $"Are you sure you want to \ndelete {item.Code} ?",
                                                       MessageViewButton.YesNo, MessageViewImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Delete From [Quotation].[QuotationsPanelsItems] Where ItemID = {item.ItemID}; " +
                                   $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = ItemSort - 1 Where ItemSort > {item.ItemSort} And PanelID = {item.PanelID} And ItemTable = '{ItemTable}'; ";
                    _ = connection.Execute(query);
                }

                foreach (QItem itemData in Items.Where(x => x.ItemSort > item.ItemSort && x.ItemTable == ItemTable))
                {
                    --itemData.ItemSort;
                }

                _ = Items.Remove(item);

                UpdateValues();
            }
        }
        private bool CanAccessDelete(QItem item)
        {
            if (item == null)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void Reset()
        {
            Navigation.OpenPopup(new SelectResetTypeView(Items), PlacementMode.MousePoint, false);

            Navigation.ClosePopupEvent += UpdateValues;
        }
        private bool CanAccessReset()
        {
            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }
    }
}