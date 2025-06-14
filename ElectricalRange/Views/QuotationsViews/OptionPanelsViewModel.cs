using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Data;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows.Data;

using Option = ProjectsNow.Data.Quotations.QuotationOption;
using Panel = ProjectsNow.Data.Quotations.QuotationOptionPanel;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class OptionPanelsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Panel _SelectedItem;
        private ObservableCollection<Panel> _Items;
        private ICollectionView _ItemsCollection;

        public OptionPanelsViewModel(Option option, ObservableCollection<Panel> panels)
        {
            OptionData = option;
            PanelsData = panels;
            GetData();
            AddCommand = new RelayCommand<Panel>(Add, CanAdd);
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
        public Panel SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Panel> Items
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
        public Option OptionData { get; }
        public ObservableCollection<Panel> PanelsData { get; }

        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select *  From [Quotation].[OptionsPanels(CanSelect)] " +
                           $"Where OptionID = {OptionData.ID} " +
                           $"Order By PanelSN";

            Items = new ObservableCollection<Panel>(connection.Query<Panel>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

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

        public RelayCommand<Panel> AddCommand { get; }
        public RelayCommand AddAllCommand { get; }
        public RelayCommand CloseCommand { get; }

        private void Add(Panel panel)
        {
            panel.OptionID = OptionData.ID;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Insert(panel);
            }

            _ = Items.Remove(panel);
            PanelsData.Add(panel);
        }
        private bool CanAdd(Panel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void AddAll()
        {
            var items = ItemsCollection.Cast<Panel>();
            foreach (Panel panel in items)
            {
                panel.OptionID = OptionData.ID;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(panel);
                }

                PanelsData.Add(panel);
            }

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