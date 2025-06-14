using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace ProjectsNow.Views.JobOrdersViews
{
    internal class WarrantyPanelsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private WPanel _SelectedItem;
        private ObservableCollection<WPanel> _Items;
        private ICollectionView _ItemsCollection;

        public WarrantyPanelsViewModel(ObservableCollection<WPanel> warrantyPanels, ObservableCollection<WPanel> orderPanels)
        {
            Items = orderPanels;
            PanelsData = warrantyPanels;

            AddCommand = new RelayCommand<WPanel>(Add, CanAdd);
            AddAllCommand = new RelayCommand(AddAll, CanAddAll);
            CloseCommand = new RelayCommand(Close, CanClose);
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
        public WPanel SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<WPanel> Items
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
        public ObservableCollection<WPanel> PanelsData { get; }

        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

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

        public RelayCommand<WPanel> AddCommand { get; }
        public RelayCommand AddAllCommand { get; }
        public RelayCommand CloseCommand { get; }

        private void Add(WPanel panel)
        {
            panel.PanelId = panel.Id;
            panel.Id = 0;

            _ = Items.Remove(panel);
            PanelsData.Add(panel);
        }
        private bool CanAdd(WPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void AddAll()
        {
            foreach (WPanel panel in Items)
            {
                panel.PanelId = panel.Id;
                panel.Id = 0;

                PanelsData.Add(panel);
            }

            Items.Clear();
            Navigation.ClosePopup();
        }
        private bool CanAddAll()
        {
            if (Items == null)
                return false;

            if (Items.Count == 0)
                return false;

            return true;
        }

        private void Close()
        {
            Navigation.ClosePopup();
        }
        private bool CanClose()
        {
            return true;
        }
    }
}