using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    public class ClosingRequestViewModel : ViewModelBase
    {
        private int? _Closing;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ProductionPanel _SelectedItem;
        private ObservableCollection<ProductionPanel> _Items;
        private ICollectionView _ItemsCollection;

        public ClosingRequestViewModel(ClosingRequest request, ObservableCollection<ProductionPanel> panels, ObservableCollection<ProductionPanel> orderPanels)
        {
            RequestData = request;
            TransactionsData = panels;
            Items = orderPanels;

            var items = ItemsCollection.Cast<ProductionPanel>();
            if (items.Count() != 0)
            {
                SelectedItem = items.First();
            }

            AddCommand = new RelayCommand<ProductionPanel>(Add, CanAdd);
            SubtractCommand = new RelayCommand<ProductionPanel>(Subtract, CanSubtract);
            PostCommand = new RelayCommand<ProductionPanel>(Post, CanPost);
        }

        public ClosingRequest RequestData { get; }

        public int? Closing
        {
            get => _Closing;
            set => SetValue(ref _Closing, value);
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

        public ProductionPanel SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (SetValue(ref _SelectedItem, value))
                {
                    if (SelectedItem != null)
                    {
                        Closing = SelectedItem.ReadyToCloseQty;
                    }
                }
            }
        }

        public ObservableCollection<ProductionPanel> Items
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

        public RelayCommand<ProductionPanel> AddCommand { get; }
        public RelayCommand<ProductionPanel> SubtractCommand { get; }
        public RelayCommand<ProductionPanel> PostCommand { get; }

        public ObservableCollection<ProductionPanel> TransactionsData { get; }

        #region Data Filter

        private bool DataFilter(object panel)
        {
            if (panel == null)
                return false;

            if (((ProductionPanel)panel).ReadyToCloseQty == 0)
                return false;

            //if (Items.Count != 0)
            //{
            //    if (TransactionsData.Any(x => x.Reference == RequestData.Number && x.PanelID == ((ProductionPanel)panel).PanelID))
            //        return false;
            //}

            return true;
        }

        #endregion

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
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

        private void Add(ProductionPanel panel)
        {
            if (Closing == null)
                return;

            if (Closing.Value < panel.ReadyToCloseQty)
            {
                Closing++;
            }
        }
        private bool CanAdd(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void Subtract(ProductionPanel panel)
        {
            if (Closing == null)
                return;

            if (Closing.Value > 1)
            {
                Closing--;
            }
        }
        private bool CanSubtract(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void Post(ProductionPanel panel)
        {
            int index = SelectedIndex;

            ProductionPanel newPanel = new()
            {
                JobOrderId = panel.JobOrderID,
                PanelID = panel.PanelID,
                PanelSN = panel.PanelSN.Value,
                PanelName = panel.PanelName,
                PanelTypeArabic = panel.PanelTypeArabic,
                Qty = Closing.Value,
                EnclosureType = panel.EnclosureType,
                Reference = RequestData.Number,
                Date = RequestData.Date,
                Action = "Closed",
                NetPrice = panel.PanelsEstimatedPrice / panel.PanelQty * Closing.Value,
                UnitNetPrice = panel.PanelEstimatedPrice,
                VATValue = panel.PanelsVATValue / panel.PanelQty * Closing.Value,
                UnitVATValue = panel.PanelVATValue,
                GrossPrice = panel.PanelsFinalPrice / panel.PanelQty * Closing.Value,
                UnitGrossPrice = panel.PanelFinalPrice,
            };

            TransactionsData.Add(newPanel);

            ItemsCollection.Refresh();
            var collection = ItemsCollection.Cast<ProductionPanel>().ToList();

            if (collection.Count != 0)
            {
CheckPanel:
                ProductionPanel indexPanel = collection.ElementAtOrDefault(index);
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

        private bool CanPost(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            if (Closing == null)
                return false;

            return Closing.Value > 0;
        }
    }
}
