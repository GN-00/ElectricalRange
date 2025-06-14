using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    public class JobOrderPanelsByPriceViewModel : ViewModelBase
    {
        private string _Description;
        private double? _Invoicing;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private PanelPrices _SelectedItem;
        private ObservableCollection<PanelPrices> _Items;
        private ICollectionView _ItemsCollection;

        public JobOrderPanelsByPriceViewModel(ObservableCollection<InvoiceItem> transactions, ObservableCollection<PanelPrices> panels)
        {
            TransactionsData = transactions;
            Items = panels;

            foreach (PanelPrices panel in Items)
                panel.InvoicingQty = panel.Qty;

            var panelsCollection = ItemsCollection.Cast<PanelPrices>();

            if (panelsCollection.Count() != 0)
            {
                SelectedItem = panelsCollection.First();
            }

            PostCommand = new RelayCommand<PanelPrices>(Post, CanPost);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }
        public double? Invoicing
        {
            get => _Invoicing;
            set => SetValue(ref _Invoicing, value);
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
        public PanelPrices SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (SetValue(ref _SelectedItem, value))
                {
                    if (SelectedItem != null)
                        Invoicing = SelectedItem.Balance;
                    else
                        Invoicing = null;
                }
            }
        }
        public ObservableCollection<PanelPrices> Items
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

        public RelayCommand<PanelPrices> AddCommand { get; }
        public RelayCommand<PanelPrices> SubtractCommand { get; }
        public RelayCommand<PanelPrices> PostCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        public ObservableCollection<InvoiceItem> TransactionsData { get; }

        #region Data Filter

        private bool DataFilter(object panel)
        {
            if (panel == null)
                return false;

            if (((PanelPrices)panel).Balance < 1)
                return false;

            if (Items.Count != 0)
            {
                if (TransactionsData.Any(x => x.PanelId == ((PanelPrices)panel).PanelId))
                    return false;
            }

            return true;
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

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("SN", ListSortDirection.Ascending));
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

        private void Post(PanelPrices panel)
        {
            int index = SelectedIndex;
            double qty = panel.InvoicingQty;
            double invoiced = Invoicing.Value;
            double netPrice = invoiced / (1 + panel.VAT);
            double vatValue = invoiced / (1 + panel.VAT)  * panel.VAT;

            panel.Invoiced += invoiced;
            InvoiceItem newPanel = new()
            {
                PanelId = panel.PanelId,
                SN = panel.SN,
                Description = panel.Name,
                ArabicDescription = panel.ArabicName,
                Qty = qty,
                NetPrice = netPrice,
                UnitNetPrice = netPrice / qty,
                VATValue = vatValue,
                UnitVATValue = vatValue / qty,
                GrossPrice = invoiced,
                UnitGrossPrice = invoiced / qty,
                UnitOriginalPrice = (double)panel.UnitOriginalPrice,
            };

            if (!string.IsNullOrWhiteSpace(Description))
            {
                newPanel.Description += $" {Description}";
            }

            TransactionsData.Add(newPanel);

            ItemsCollection.Refresh();
            var collection = ItemsCollection.Cast<PanelPrices>().ToList();

            if (collection.Count != 0)
            {
            CheckPanel:
                PanelPrices indexPanel = collection.ElementAtOrDefault(index);
                if (indexPanel == null)
                {
                    index--;
                    goto CheckPanel;
                }
                else
                {
                    SelectedItem = indexPanel;
                }
            }
        }
        private bool CanPost(PanelPrices panel)
        {
            if (panel == null)
                return false;

            if (Invoicing == null)
                return false;

            return Invoicing.Value > 0;
        }
    }
}