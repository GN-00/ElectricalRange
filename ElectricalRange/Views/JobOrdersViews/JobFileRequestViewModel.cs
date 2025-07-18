﻿using ProjectsNow.Commands;
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
    public class JobFileRequestViewModel : ViewModelBase
    {
        private int? _Sending;
        private DateTime _DeliveryDate = DateTime.Now.AddDays(7);
        private string _Indicator = "-";
        private int _SelectedIndex;
        private JPanel _SelectedItem;
        private ObservableCollection<JPanel> _Items;
        private ICollectionView _ItemsCollection;

        public JobFileRequestViewModel(JobFileRequest request, ObservableCollection<Panel> panels, ObservableCollection<JPanel> orderPanels)
        {
            RequestData = request;
            TransactionsData = panels;
            Items = orderPanels;

            var items = ItemsCollection.Cast<JPanel>();
            if (items.Count() != 0)
            {
                SelectedItem = items.First();
            }

            AddCommand = new RelayCommand<JPanel>(Add, CanAdd);
            SubtractCommand = new RelayCommand<JPanel>(Subtract, CanSubtract);
            PostCommand = new RelayCommand<JPanel>(Post, CanPost);
        }

        public JobFileRequest RequestData { get; }

        public int? Sending
        {
            get => _Sending;
            set => SetValue(ref _Sending, value);
        }

        public DateTime DeliveryDate
        {
            get => _DeliveryDate;
            set => SetValue(ref _DeliveryDate, value);
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
                    {
                        Sending = SelectedItem.ReadyToProductionQty;
                    }
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

            if (((JPanel)panel).ReadyToProductionQty == 0)
                return false;

            var status = ((JPanel)panel).Status;
            if (status == Enums.Statuses.Production.ToString())
                return false;

            if (status == Enums.Statuses.QC.ToString())
                return false;

            if (status == Enums.Statuses.Closed.ToString())
                return false;

            if (status == Enums.Statuses.Delivered.ToString())
                return false;

            if (status == Enums.Statuses.Canceled.ToString())
                return false;

            if (status == Enums.Statuses.Hold.ToString())
                return false;

            if (Items.Count != 0)
            {
                if (TransactionsData.Any(x => x.Reference == RequestData.Number && x.PanelID == ((JPanel)panel).PanelID))
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
            if (Sending == null)
                return;

            if (Sending.Value < panel.ReadyToProductionQty)
            {
                Sending++;
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
            if (Sending == null)
                return;

            if (Sending.Value > 1)
            {
                Sending--;
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
                Qty = Sending.Value,
                EnclosureType = panel.EnclosureType,
                Reference = RequestData.Number,
                Date = RequestData.Date,
                DeliveryDate = DeliveryDate,
                Action = "Production",
                NetPrice = panel.PanelsEstimatedPrice / panel.PanelQty * Sending.Value,
                UnitNetPrice = panel.PanelEstimatedPrice,
                VATValue = panel.PanelsVATValue / panel.PanelQty * Sending.Value,
                UnitVATValue = panel.PanelVATValue,
                GrossPrice = panel.PanelsFinalPrice / panel.PanelQty * Sending.Value,
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

            if (Sending == null)
                return false;

            return Sending.Value > 0;
        }
    }
}
