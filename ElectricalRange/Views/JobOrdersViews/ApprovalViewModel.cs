using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using Panel = ProjectsNow.Data.JobOrders.TransactionPanel;

namespace ProjectsNow.Views.JobOrdersViews
{
    public class ApprovalViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private JPanel _SelectedItem;
        private ObservableCollection<JPanel> _Items;
        private ICollectionView _ItemsCollection;

        public ApprovalViewModel(ApprovalRequest request, ObservableCollection<Panel> panels, ObservableCollection<JPanel> orderPanels)
        {
            RequestData = request;
            TransactionsData = panels;
            Items = orderPanels;

            if (Items.Count != 0)
            {
                SelectedItem = Items[0];
            }

            AddCommand = new RelayCommand<JPanel>(Add, CanAdd);
            AddAllCommand = new RelayCommand(AddAll, CanAddAll);
            //SubtractCommand = new RelayCommand<JPanel>(Subtract, CanSubtract);
            //PostCommand = new RelayCommand<JPanel>(Post, CanPost);
            //DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public ApprovalRequest RequestData { get; }

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
            set => SetValue(ref _SelectedItem, value);
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
        public RelayCommand AddAllCommand { get; }
        //public RelayCommand<JPanel> SubtractCommand { get; }
        //public RelayCommand<JPanel> PostCommand { get; }
        //public RelayCommand DeleteFilterCommand { get; }

        public ObservableCollection<Panel> TransactionsData { get; }

        #region Data Filter

        private bool DataFilter(object panel)
        {
            if (panel == null)
                return false;

            var status = ((JPanel)panel).Status;
            if (status == Enums.Statuses.Waiting_Approval.ToString())
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
                if (TransactionsData.Any(x => x.PanelID == ((JPanel)panel).PanelID))
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
            int index = SelectedIndex;
            panel.ApprovedQty = panel.ApprovedQty;

            Panel newPanel = new()
            {
                JobOrderID = panel.JobOrderID,
                PanelID = panel.PanelID,
                PanelSN = panel.PanelSN.Value,
                PanelName = panel.PanelName,
                PanelTypeArabic = panel.PanelTypeArabic,
                Qty = panel.PanelQty,
                EnclosureType = panel.EnclosureType,
                Reference = RequestData.Number,
                Date = RequestData.Date,
                Action = Enums.Statuses.Waiting_Approval.ToString(),
                NetPrice = panel.PanelsEstimatedPrice,
                UnitNetPrice = panel.PanelEstimatedPrice,
                VATValue = panel.PanelsVATValue,
                UnitVATValue = panel.PanelVATValue,
                GrossPrice = panel.PanelsFinalPrice,
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
        private bool CanAdd(JPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void AddAll()
        {
            var items = ItemsCollection.Cast<JPanel>();
            foreach (JPanel panel in items)
            {
                panel.ApprovedQty = panel.ApprovedQty;
                Panel newPanel = new()
                {
                    JobOrderID = panel.JobOrderID,
                    PanelID = panel.PanelID,
                    PanelSN = panel.PanelSN.Value,
                    PanelName = panel.PanelName,
                    PanelTypeArabic = panel.PanelTypeArabic,
                    Qty = panel.PanelQty,
                    EnclosureType = panel.EnclosureType,
                    Reference = RequestData.Number,
                    Date = RequestData.Date,
                    Action = Enums.Statuses.Waiting_Approval.ToString(),
                    NetPrice = panel.PanelsEstimatedPrice,
                    UnitNetPrice = panel.PanelEstimatedPrice,
                    VATValue = panel.PanelsVATValue,
                    UnitVATValue = panel.PanelVATValue,
                    GrossPrice = panel.PanelsFinalPrice,
                    UnitGrossPrice = panel.PanelFinalPrice,
                };

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

        //private void Post(JPanel panel)
        //{

        //}

        //private bool CanPost(JPanel panel)
        //{
        //    if (panel == null)
        //        return false;

        //    if (Sending == null)
        //        return false;

        //    return Sending.Value > 0;
        //}
    }
}