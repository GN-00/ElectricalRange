using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.References;
using ProjectsNow.Enums;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class AddStandardItemViewModel : ViewModelBase
    {
        private SearchKey _SelectedKey;
        private Reference _SelectedReference;
        private string _SelectedArticle1;
        private string _SelectedArticle2;
        private string _SelectedRemark;
        private decimal? _ItemQty;
        private decimal? _ItemCost;
        private string _Stock = "Stock: -";
        private bool _EditableCost;

        private ICollectionView _ReferencesCollection;
        private ObservableCollection<Reference> _ReferencesData;
        private ObservableCollection<SearchKey> _SearchKeysData;
        private ObservableCollection<string> _Articles1Data;
        private ObservableCollection<string> _Articles2Data;
        private ObservableCollection<string> _RemarksData;

        public AddStandardItemViewModel(QItem item, ObservableCollection<QItem> items)
        {
            ItemData = item;
            ItemsData = items;
            NewData.Update(ItemData);
            ItemQty = NewData.ItemQty;
            ItemCost = NewData.ItemCost;

            GetData();

            UpdateCommand = new RelayCommand(Update, CanUpdate);
            CustomCommand = new RelayCommand(Custom, CanCustom);
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }


        public QItem ItemData { get; }
        public QItem NewData { get; } = new QItem();
        public ObservableCollection<QItem> ItemsData { get; }

        public SearchKey SelectedKey
        {
            get => _SelectedKey;
            set
            {
                if (SetValue(ref _SelectedKey, value))
                {
                    ReferencesCollection.Refresh();
                }
            }
        }
        public Reference SelectedReference
        {
            get => _SelectedReference;
            set
            {
                if (SetValue(ref _SelectedReference, value))
                {
                    if (SelectedReference == null)
                    {
                        NewData.Description = null;

                        NewData.Brand = null;
                        NewData.Remarks = null;
                        NewData.ItemDiscount = 0;
                        ItemCost = 0;
                        NewData.Unit = null;
                        NewData.Unit = "Lot";
                        Stock = $"Stock: -";
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(NewData.Article1))
                        {
                            NewData.Article1 = SelectedReference.Article1;
                        }

                        if (string.IsNullOrWhiteSpace(NewData.Article2))
                        {
                            NewData.Article2 = SelectedReference.Article2;
                        }

                        NewData.Description = SelectedReference.Description;
                        NewData.Brand = SelectedReference.Brand;

                        if (string.IsNullOrWhiteSpace(NewData.Remarks))
                        {
                            NewData.Remarks = SelectedReference.Remarks;
                        }

                        NewData.ItemDiscount = SelectedReference.Discount;
                        ItemCost = SelectedReference.Cost;
                        NewData.Unit = SelectedReference.Unit;
                        EditableCost = !SelectedReference.EditableCost;
                        Stock = $"Stock: {SelectedReference.Stock}";
                    }
                }
            }
        }
        public string SelectedArticle1
        {
            get => _SelectedArticle1;
            set => SetValue(ref _SelectedArticle1, value);
        }
        public string SelectedArticle2
        {
            get => _SelectedArticle2;
            set => SetValue(ref _SelectedArticle2, value);
        }
        public string SelectedRemark
        {
            get => _SelectedRemark;
            set => SetValue(ref _SelectedRemark, value);
        }
        public decimal? ItemQty
        {
            get => _ItemQty;
            set
            {


                if (SetValue(ref _ItemQty, value))
                {
                    if (NewData.Unit == "No" || NewData.Unit == "Set")
                    {
                        _ItemQty = decimal.ToInt32(_ItemQty.GetValueOrDefault());
                        OnPropertyChanged(nameof(ItemQty));
                    }

                    NewData.ItemQty = _ItemQty.GetValueOrDefault();
                }
            }
        }
        public decimal? ItemCost
        {
            get => _ItemCost;
            set
            {
                if (SetValue(ref _ItemCost, value))
                {
                    NewData.ItemCost = ItemCost.GetValueOrDefault();
                }
            }
        }
        public string Stock
        {
            get => _Stock;
            set => SetValue(ref _Stock, value);
        }
        public bool EditableCost
        {
            get => _EditableCost;
            set => SetValue(ref _EditableCost, value);
        }

        public ObservableCollection<Reference> ReferencesData
        {
            get => _ReferencesData;
            set
            {
                if (SetValue(ref _ReferencesData, value))
                {
                    CreateCollectionView();
                }
            }
        }

        public ObservableCollection<SearchKey> SearchKeysData
        {
            get => _SearchKeysData;
            set => SetValue(ref _SearchKeysData, value);
        }
        public ObservableCollection<string> Articles1Data
        {
            get => _Articles1Data;
            set => SetValue(ref _Articles1Data, value);
        }
        public ObservableCollection<string> Articles2Data
        {
            get => _Articles2Data;
            set => SetValue(ref _Articles2Data, value);
        }
        public ObservableCollection<string> RemarksData
        {
            get => _RemarksData;
            set => SetValue(ref _RemarksData, value);
        }

        public ICollectionView ReferencesCollection
        {
            get => _ReferencesCollection;
            set => SetValue(ref _ReferencesCollection, value);
        }

        public RelayCommand UpdateCommand { get; }
        public RelayCommand CustomCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }


        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            if (AppData.ReferencesListData == null)
            {
                ReferencesData =
                    AppData.ReferencesListData =
                        ReferenceController.GetReferences(connection);
            }
            else
            {
                ReferencesData =
                   AppData.ReferencesListData;
            }

            SearchKeysData = SearchKey.GetKeys(connection);
            Articles1Data = ReferenceController.GetArticle1(connection);
            Articles2Data = ReferenceController.GetArticle2(connection);
            RemarksData = ReferenceController.GetRemarks(connection);
        }
        private void CreateCollectionView()
        {
            ReferencesCollection = CollectionViewSource.GetDefaultView(ReferencesData);
            ReferencesCollection.Filter = new Predicate<object>(DataFilter);
            ReferencesCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
        }

        #region Data Filter
        private bool DataFilter(object item)
        {
            if (SelectedKey == null)
                return true;

            bool result = false;
            string columnName = "SearchKeys";

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = SelectedKey.Key.ToUpper();

            if (value.Contains(checkValue))
            {
                result = true;
            }

            return result;
        }

        #endregion

        private void Custom()
        {
            ItemData.ItemType = Enums.ItemTypes.NewItem.ToString();
            Navigation.OpenPopup(new AddCustomItemView(ItemData, ItemsData), PlacementMode.Center, true);
        }
        private bool CanCustom()
        {
            if (ItemData.ItemID != 0)
                return false;

            return true;
        }

        private void Save()
        {
            NewData.Category = SelectedReference.Category;
            NewData.ItemType = ItemTypes.Standard.ToString();

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (NewData.ItemID != 0) //Edit
                {
                    _ = connection.Update(NewData);
                    ItemData.Update(NewData);
                }
                else
                {
                    if (NewData.ItemSort == ItemsData.Count(x => x.ItemTable == NewData.ItemTable)) //Add
                    {
                        ItemsData.Add(NewData);
                    }
                    else //Insert
                    {
                        _ = connection.Execute($"Update [Quotation].[QuotationsPanelsItems] " +
                                               $"Set ItemSort = ItemSort + 1 " +
                                               $"Where ItemSort >= {NewData.ItemSort} " +
                                               $"And PanelID = {NewData.PanelID} " +
                                               $"And ItemTable = '{NewData.ItemTable}'");

                        foreach (QItem item in ItemsData.Where(x => x.ItemSort >= NewData.ItemSort && x.ItemTable == NewData.ItemTable))
                        {
                            item.ItemSort++;
                        }

                        ItemsData.Add(NewData);
                    }

                    _ = connection.Insert(NewData);
                }
            }

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (SelectedReference == null)
                return false;

            if (string.IsNullOrWhiteSpace(NewData.Article1))
                return false;

            if (ItemQty.GetValueOrDefault() == 0)
                return false;

            if (ItemCost.GetValueOrDefault() == 0)
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

        private void Update()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            ReferencesData =
                AppData.ReferencesListData =
                    ReferenceController.GetReferences(connection);

            SearchKeysData = SearchKey.GetKeys(connection);
            Articles1Data = ReferenceController.GetArticle1(connection);
            Articles2Data = ReferenceController.GetArticle2(connection);
            RemarksData = ReferenceController.GetRemarks(connection);
        }
        private bool CanUpdate()
        {
            return true;
        }
    }
}