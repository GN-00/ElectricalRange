using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace ProjectsNow.Views.Production
{

    public class NewOrderPanelsPostingViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private ProductionPanel _SelectedItem;
        private ObservableCollection<ProductionPanel> _Items;
        private ICollectionView _ItemsCollection;

        public NewOrderPanelsPostingViewModel(JobFile request, ObservableCollection<ProductionPanel> panels, ObservableCollection<ProductionPanel> orderPanels)
        {
            RequestData = request;
            TransactionsData = panels;
            Items = orderPanels;

            if (Items.Count != 0)
            {
                SelectedItem = Items[0];
            }

            AddCommand = new RelayCommand<ProductionPanel>(Add, CanAdd);
            AddAllCommand = new RelayCommand(AddAll, CanAddAll);
        }

        public JobFile RequestData { get; }

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
            set => SetValue(ref _SelectedItem, value);
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
        public RelayCommand AddAllCommand { get; }
        //public RelayCommand<ProductionPanel> SubtractCommand { get; }
        //public RelayCommand<ProductionPanel> PostCommand { get; }
        //public RelayCommand DeleteFilterCommand { get; }

        public ObservableCollection<ProductionPanel> TransactionsData { get; }

        #region Data Filter

        private bool DataFilter(object panel)
        {
            if (panel == null)
                return false;

            var status = ((ProductionPanel)panel).Reference;
            if (status != 0)
                return false;

            //if (status == Enums.Statuses.Production.ToString())
            //    return false;

            //if (status == Enums.Statuses.Closed.ToString())
            //    return false;

            //if (status == Enums.Statuses.Delivered.ToString())
            //    return false;

            //if (status == Enums.Statuses.Canceled.ToString())
            //    return false;

            //if (status == Enums.Statuses.Hold.ToString())
            //    return false;

            if (Items.Count != 0)
            {
                if (TransactionsData.Any(x => x.PanelId == ((ProductionPanel)panel).PanelId))
                    return false;
            }

            return true;
        }
        //private void DeleteFilter()
        //{
        //    foreach (PropertyInfo property in GetType().GetProperties())
        //    {
        //        FilterProperty attribute = (FilterProperty)property.GetCustomAttribute(typeof(FilterProperty));
        //        if (attribute != null)
        //        {
        //            property.SetValue(this, null);
        //        }
        //    }

        //    ItemsCollection.Refresh();
        //}

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
            int index = SelectedIndex;
            ProductionPanel newPanel = new();
            newPanel.Update(panel);

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
        private bool CanAdd(ProductionPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void AddAll()
        {
            var items = ItemsCollection.Cast<ProductionPanel>();
            foreach (ProductionPanel panel in items)
            {
                ProductionPanel newPanel = new();
                newPanel.Update(panel);

                TransactionsData.Add(newPanel);
            }

            ItemsCollection.Refresh();

        }
        private bool CanAddAll()
        {
            if (ItemsCollection.Cast<object>().Count() == 0)
                return false;

            return true;
        }
    }
}