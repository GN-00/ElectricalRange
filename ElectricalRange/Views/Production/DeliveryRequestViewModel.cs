using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{
    public class DeliveryRequestViewModel : ViewModelBase
    {
        private int? _Deliver;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ProductionPanel _SelectedItem;
        private ObservableCollection<ProductionPanel> _Items;
        private ICollectionView _ItemsCollection;

        public DeliveryRequestViewModel(DeliveryRequest request, ObservableCollection<ProductionPanel> panels, ObservableCollection<ProductionPanel> orderPanels)
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

        public DeliveryRequest RequestData { get; }

        public int? Deliver
        {
            get => _Deliver;
            set => SetValue(ref _Deliver, value);
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
                        Deliver = SelectedItem.ReadyToCloseQty;
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

            if (Items.Count != 0)
            {
                if (TransactionsData.Any(x => x.Reference == RequestData.Number && x.PanelId == ((ProductionPanel)panel).PanelId))
                    return false;
            }

            return true;
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

        private void Add(ProductionPanel panel)
        {
            if (Deliver == null)
                return;

            if (Deliver.Value < panel.ReadyToCloseQty)
            {
                Deliver++;
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
            if (Deliver == null)
                return;

            if (Deliver.Value > 1)
            {
                Deliver--;
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
                PanelId = panel.PanelId,
                OrderId = panel.OrderId,
                Reference = RequestData.Number,
                SN = panel.SN,
                Name = panel.Name,
                Qty = panel.Qty,
                DeliveredQty = Deliver.Value,
                Date = DateTime.Now,
                InProduction = false
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

            if (Deliver == null)
                return false;

            return Deliver.Value > 0;
        }
    }
}
