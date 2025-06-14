using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.JobOrders;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    public class SelectProformaInvoicePanelsViewModel : ViewModelBase
    {
        private int? _Invoicing;
        private string _Indicator = "-";
        private int _SelectedIndex;
        private JPanel _SelectedItem;
        private ObservableCollection<JPanel> _Items;
        private ICollectionView _ItemsCollection;

        public SelectProformaInvoicePanelsViewModel(ObservableCollection<ProformaInvoicePanel> panels, ObservableCollection<Data.JobOrders.JPanel> orderPanels)
        {
            Panels = panels;
            Items = orderPanels;

            if (Items.Count != 0)
            {
                SelectedItem = Items[0];
            }

            AddCommand = new RelayCommand<JPanel>(Add, CanAdd);
            SubtractCommand = new RelayCommand<JPanel>(Subtract, CanSubtract);
            PostCommand = new RelayCommand<JPanel>(Post, CanPost);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public int? Invoicing
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
        public JPanel SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (SetValue(ref _SelectedItem, value))
                {
                    if (SelectedItem != null)
                        Invoicing = SelectedItem.ReadyToInvoicedQty;
                    else
                        Invoicing = null;
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
        public RelayCommand DeleteFilterCommand { get; }

        public ObservableCollection<ProformaInvoicePanel> Panels { get; }

        #region Data Filter

        private bool DataFilter(object panel)
        {
            if (panel == null)
                return false;

            if (((JPanel)panel).NotInvoicedQty < 1)
                return false;

            if (Items.Count != 0)
            {
                if (Panels.Any(x => x.PanelId == ((JPanel)panel).PanelID))
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
            if (Invoicing == null)
                return;

            if (Invoicing.Value < panel.PanelQty)
            {
                Invoicing++;
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
            if (Invoicing == null)
                return;

            if (Invoicing.Value > 1)
            {
                Invoicing--;
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
            ProformaInvoicePanel newPanel = new()
            {
                PanelId = panel.PanelID,
                SN = panel.PanelSN.Value,
                Description = panel.PanelName,
                ArabicDescription = panel.PanelTypeArabic,
                Qty = Invoicing.Value,
                NetPrice = (double)panel.PanelsEstimatedPrice,
                UnitNetPrice = (double)panel.PanelEstimatedPrice,
                VATValue = (double)panel.PanelsVATValue,
                UnitVATValue = (double)panel.PanelVATValue,
                GrossPrice = (double)panel.PanelsFinalPrice,
                UnitGrossPrice = (double)panel.PanelFinalPrice,
                UnitOriginalPrice = (double)panel.PanelEstimatedPrice,
            };

            Panels.Add(newPanel);

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

            if (Invoicing == null)
                return false;

            return Invoicing.Value > 0;
        }
    }
}