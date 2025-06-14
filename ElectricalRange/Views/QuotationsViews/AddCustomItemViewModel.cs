using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Enums;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class AddCustomItemViewModel : ViewModelBase
    {
        private string _SelectedArticle1;
        private string _SelectedArticle2;
        private decimal? _ItemDiscount;
        private decimal? _ItemQty;
        private decimal? _ItemCost;

        private ObservableCollection<string> _Articles1Data;
        private ObservableCollection<string> _Articles2Data;

        public AddCustomItemViewModel(QItem item, ObservableCollection<QItem> items)
        {
            ItemData = item;
            ItemsData = items;
            NewData.Update(ItemData);
            ItemDiscount = NewData.ItemDiscount;
            ItemQty = NewData.ItemQty;
            ItemCost = NewData.ItemCost;

            GetData();

            StandardCommand = new RelayCommand(Standard, CanStandard);
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }


        public QItem ItemData { get; }
        public QItem NewData { get; } = new QItem();
        public ObservableCollection<QItem> ItemsData { get; }

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
        public decimal? ItemDiscount
        {
            get => _ItemDiscount;
            set
            {
                if (SetValue(ref _ItemDiscount, value))
                {
                    if (ItemDiscount > 100)
                    {
                        _ItemDiscount = 100;
                        OnPropertyChanged(nameof(ItemDiscount));
                    }

                    NewData.ItemDiscount = _ItemDiscount.GetValueOrDefault();
                }
            }
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


        public RelayCommand StandardCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }


        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            Articles1Data = ReferenceController.GetArticle1(connection);
            Articles2Data = ReferenceController.GetArticle2(connection);
        }

        private void Standard()
        {
            ItemData.ItemType = Enums.ItemTypes.Standard.ToString();
            Navigation.OpenPopup(new AddStandardItemView(ItemData, ItemsData), PlacementMode.Center, true);
        }
        private bool CanStandard()
        {
            if (ItemData.ItemID != 0)
                return false;

            return true;
        }

        private void Save()
        {
            NewData.Category = "Custom Item";
            NewData.ItemType = ItemTypes.NewItem.ToString();

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
                                               $"And PanelID ={NewData.PanelID} " +
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
            if (string.IsNullOrWhiteSpace(NewData.Code))
                return false;

            if (string.IsNullOrWhiteSpace(NewData.Description))
                return false;

            if (string.IsNullOrWhiteSpace(NewData.Unit))
                return false;

            if (string.IsNullOrWhiteSpace(NewData.Article1))
                return false;

            if (ItemQty.GetValueOrDefault() == 0)
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
            GetData();
        }
        private bool CanUpdate()
        {
            return true;
        }
    }
}