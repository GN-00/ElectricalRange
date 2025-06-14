using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

using Panel = ProjectsNow.Data.JobOrders.TransactionPanel;

namespace ProjectsNow.Views.JobOrdersViews
{
    public class DeliveryViewModel : ViewModelBase
    {
        private int? _Delivering;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private JPanel _SelectedItem;
        private ObservableCollection<JPanel> _Items;
        private ICollectionView _ItemsCollection;

        public DeliveryViewModel(Delivery delivery, ObservableCollection<Panel> panels, ObservableCollection<JPanel> orderPanels)
        {
            DeliveryData = delivery;
            TransactionsData = panels;
            Items = orderPanels;

            if (Items.Count != 0)
            {
                SelectedItem = Items[0];
            }

            AddCommand = new RelayCommand<JPanel>(Add, CanAdd);
            SubtractCommand = new RelayCommand<JPanel>(Subtract, CanSubtract);
            PostCommand = new RelayCommand<JPanel>(Post, CanPost);
        }

        public Delivery DeliveryData { get; }

        public int? Delivering
        {
            get => _Delivering;
            set => SetValue(ref _Delivering, value);
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
        public JPanel SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (SetValue(ref _SelectedItem, value))
                {
                    if (SelectedItem != null)
                        Delivering = SelectedItem.ReadyToDeliverQty;
                    else
                        Delivering = null;
                }
            }
        }
        public ObservableCollection<JPanel> Items
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

        public RelayCommand<JPanel> AddCommand { get; }
        public RelayCommand<JPanel> SubtractCommand { get; }
        public RelayCommand<JPanel> PostCommand { get; }

        public ObservableCollection<Panel> TransactionsData { get; }

        #region Data Filter

        private bool DataFilter(object panel)
        {
            if (panel == null)
                return false;

            if (((JPanel)panel).NotDeliveredQty < 1)
                return false;

            if (Items.Count != 0)
            {
                if (TransactionsData.Any(x => x.Reference == DeliveryData.Number && x.PanelID == ((JPanel)panel).PanelID))
                    return false;
            }

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

        private void Add(JPanel panel)
        {
            if (Delivering == null)
                return;

            if (Delivering.Value < panel.ReadyToInvoicedQty)
            {
                Delivering++;
            }
        }
        private bool CanAdd(JPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void Subtract(JPanel panel)
        {
            if (Delivering == null)
                return;

            if (Delivering.Value > 1)
            {
                Delivering--;
            }
        }
        private bool CanSubtract(JPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void Post(JPanel panel)
        {
            int index = SelectedIndex;

            Panel newPanel = new()
            {
                JobOrderID = panel.JobOrderID,
                PanelID = panel.PanelID,
                PanelSN = panel.PanelSN.Value,
                PanelName = panel.PanelName,
                PanelTypeArabic = panel.PanelTypeArabic,
                Qty = Delivering.Value,
                EnclosureType = panel.EnclosureType,
                Reference = DeliveryData.Number,
                Date = DeliveryData.Date,
                Action = "Delivered",
                NetPrice = panel.PanelsEstimatedPrice / panel.PanelQty * Delivering.Value,
                UnitNetPrice = panel.PanelEstimatedPrice,
                VATValue = panel.PanelsVATValue / panel.PanelQty * Delivering.Value,
                UnitVATValue = panel.PanelVATValue,
                GrossPrice = panel.PanelsFinalPrice / panel.PanelQty * Delivering.Value,
                UnitGrossPrice = panel.PanelFinalPrice,
            };

            TransactionsData.Add(newPanel);

            ItemsCollection.Refresh();
            var collection = ItemsCollection.Cast<JPanel>().ToList();

            if (collection.Count != 0)
            {
            CheckPanel:
                JPanel indexPanel = collection.ElementAtOrDefault(index);
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

        private bool CanPost(JPanel panel)
        {
            if (panel == null)
                return false;

            if (Delivering == null)
                return false;

            return Delivering.Value > 0;
        }
    }
}