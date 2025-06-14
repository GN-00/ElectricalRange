using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class ImportPanelsViewModel : ViewModelBase
    {
        private string _SourceIndicator = "-";
        private string _TargetIndicator = "-";
        private int _SelectedSourceIndex;
        private int _SelectedTargetIndex;
        private QPanel _SelectedSourceItem;
        private QPanel _SelectedTargetItem;

        private int? _SelectedYear;
        private Quotation _SelectedQuotation;

        private ObservableCollection<QPanel> _SourceItems;
        private ObservableCollection<Quotation> _SourceQuotations;
        private ObservableCollection<int> _SourceYears;

        private ICollectionView _SourceItemsCollection;
        private ICollectionView _TargetItemsCollection;

        public ImportPanelsViewModel(Quotation quotation, ObservableCollection<QPanel> panels, IView view)
        {
            ViewData = view;
            QuotationData = quotation;
            PanelsData = panels;

            GetData();

            LoadCommand = new RelayCommand<Quotation>(Load, CanLoad);
            ImportCommand = new RelayCommand<QPanel>(Import, CanImport);
            ImportAllCommand = new RelayCommand(ImportAll, CanImportAll);
            DeleteCommand = new RelayCommand<QPanel>(Delete, CanDelete);
        }

        public Quotation QuotationData { get; }
        public ObservableCollection<QPanel> PanelsData { get; }

        public string SourceIndicator
        {
            get => _SourceIndicator;
            set => SetValue(ref _SourceIndicator, value);
        }
        public string TargetIndicator
        {
            get => _TargetIndicator;
            set => SetValue(ref _TargetIndicator, value);
        }
        public int SelectedSourceIndex
        {
            get => _SelectedSourceIndex;
            set
            {
                if (SetValue(ref _SelectedSourceIndex, value))
                {
                    UpdateSourceIndicator();
                }
            }
        }
        public int SelectedTargetIndex
        {
            get => _SelectedTargetIndex;
            set
            {
                if (SetValue(ref _SelectedTargetIndex, value))
                {
                    UpdateTargetIndicator();
                }
            }
        }
        public QPanel SelectedSourceItem
        {
            get => _SelectedSourceItem;
            set => SetValue(ref _SelectedSourceItem, value);
        }
        public QPanel SelectedTargetItem
        {
            get => _SelectedTargetItem;
            set => SetValue(ref _SelectedTargetItem, value);
        }

        public int? SelectedYear
        {
            get => _SelectedYear;
            set
            {
                if (SetValue(ref _SelectedYear, value))
                {
                    GetQuotations();
                }
            }
        }
        public Quotation SelectedQuotation
        {
            get => _SelectedQuotation;
            set => SetValue(ref _SelectedQuotation, value);
        }
        public ObservableCollection<QPanel> SourceItems
        {
            get => _SourceItems;
            private set
            {
                if (SetValue(ref _SourceItems, value))
                {
                    CreateSourceCollectionView();
                }
            }
        }
        public ObservableCollection<int> SourceYears
        {
            get => _SourceYears;
            set => SetValue(ref _SourceYears, value);
        }
        public ObservableCollection<Quotation> SourceQuotations
        {
            get => _SourceQuotations;
            set => SetValue(ref _SourceQuotations, value);
        }

        public ICollectionView SourceItemsCollection
        {
            get => _SourceItemsCollection;
            set => SetValue(ref _SourceItemsCollection, value);
        }
        public ICollectionView TargetItemsCollection
        {
            get => _TargetItemsCollection;
            set => SetValue(ref _TargetItemsCollection, value);
        }

        public RelayCommand<Quotation> LoadCommand { get; }
        public RelayCommand<QPanel> ImportCommand { get; }
        public RelayCommand ImportAllCommand { get; }
        public RelayCommand<QPanel> DeleteCommand { get; }

        #region Data Filter
        private bool DataFilter(object panel)
        {
            bool result = true;
            string columnName = "PanelName";

            string value = $"{panel.GetType().GetProperty(columnName).GetValue(panel)}".ToUpper();
            if (PanelsData.Any(p => p.PanelName == value))
            {
                result = false;
            }

            return result;
        }
        #endregion

        private void GetData()
        {
            string query;

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (SourceYears == null)
                {
                    query = $"Select * From [Quotation].[Years(View)] ORDER BY QuotationYear DESC";
                    SourceYears = new ObservableCollection<int>(connection.Query<int>(query));
                }
            }

            if (SourceYears != null)
                SelectedYear = SourceYears[0];

            TargetCreateCollectionView();
        }

        private void GetQuotations()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [Quotation].[Quotations(View)] " +
                           $"Where QuotationYear = {SelectedYear} " +
                           $"And QuotationID <> {QuotationData.QuotationID} " +
                           $"And ProjectStatus <> 'Canceled'" +
                           $"And ProjectStatus <> 'Revision'" +
                           $"Order By QuotationNumber";
            SourceQuotations = new ObservableCollection<Quotation>(connection.Query<Quotation>(query));
        }

        private void CreateSourceCollectionView()
        {
            SourceItemsCollection = CollectionViewSource.GetDefaultView(SourceItems);

            SourceItemsCollection.Filter = new Predicate<object>(DataFilter);
            SourceItemsCollection.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
            SourceItemsCollection.CollectionChanged += SourceCollectionChanged;
            UpdateSourceIndicator();
        }
        private void TargetCreateCollectionView()
        {
            TargetItemsCollection = CollectionViewSource.GetDefaultView(PanelsData);

            TargetItemsCollection.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
            TargetItemsCollection.CollectionChanged += TargetCollectionChanged;
        }
        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSourceIndicator();
        }
        private void TargetCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateTargetIndicator();
        }
        private void UpdateSourceIndicator()
        {
            SourceIndicator = DataGridIndicator.Get(SelectedSourceIndex, SourceItemsCollection);
        }
        private void UpdateTargetIndicator()
        {
            TargetIndicator = DataGridIndicator.Get(SelectedTargetIndex, TargetItemsCollection);
        }

        private void Load(Quotation quotation)
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [Quotation].[QuotationsPanels(View)] " +
                           $"Where QuotationID = {quotation.QuotationID} " +
                           $"Order By PanelSN";

            SourceItems = new ObservableCollection<QPanel>(connection.Query<QPanel>(query));
        }
        private bool CanLoad(Quotation quotationg)
        {
            if (quotationg == null)
                return false;

            return true;
        }

        private void Import(QPanel panel)
        {
            Navigation.OpenLoading();

            QPanel newPanel = new();
            newPanel.Update(panel);

            newPanel.PurchaseOrdersID = null;
            newPanel.PanelSN = PanelsData.Count + 1;
            newPanel.QuotationID = QuotationData.QuotationID;

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Insert(newPanel);

                string query = $"Select * From [Quotation].[QuotationsPanelsItems] Where PanelID = {panel.PanelID}";
                List<QItem> items = connection.Query<QItem>(query).ToList();

                if (items.Count != 0)
                {
                    string insert = $"Insert Into [Quotation].[QuotationsPanelsItems] " +
                                    $"(PanelID, Article1, Article2, Category, Code, Description, Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort) " +
                                    $"Values " +
                                    $"({newPanel.PanelID}, @Article1, @Article2, @Category, @Code, @Description, @Unit, @ItemQty, @Brand, @Remarks, @ItemCost, @ItemDiscount, @ItemTable, @ItemType, @ItemSort)";
                    _ = connection.Execute(insert, items);
                }
            }

            PanelsData.Add(newPanel);
            SourceItemsCollection.Refresh();

            Navigation.CloseLoading();
        }

        private bool CanImport(QPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void ImportAll()
        {
            var panels = new ObservableCollection<QPanel>(SourceItemsCollection.Cast<QPanel>());
            Navigation.OpenLoading();

            foreach (QPanel panel in panels)
            {
                Navigation.LoadingText($"Importing {panels.IndexOf(panel) + 1}/{panels.Count}");

                Import(panel);
                SourceItemsCollection.Refresh();
            }

            Navigation.CloseLoading();
        }

        private bool CanImportAll()
        {
            if (SourceItemsCollection == null)
                return false;

            if (SourceItemsCollection.Cast<object>().Count() == 0)
                return false;

            return true;
        }

        private void Delete(QPanel panel)
        {
            MessageBoxResult result = MessageWindow.Show($"Deleting",
                                                         $"Are you sure you want to \ndelete {panel.PanelName} ?",
                                                         MessageWindowButton.YesNo,
                                                         MessageWindowImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Delete From [Quotation].[QuotationsPanels] Where PanelID = {panel.PanelID}; " +
                                   $"Delete From [Quotation].[QuotationsPanelsItems] Where PanelID = {panel.PanelID}; " +
                                   $"Delete From [Quotation].[QuotationsOptionsPanels] Where PanelID = {panel.PanelID}; " +
                                   $"Update [Quotation].[QuotationsPanels] Set PanelSN = PanelSN - 1 Where PanelSN > {panel.PanelSN} And QuotationID = {panel.QuotationID}; ";

                    _ = connection.Execute(query);
                }

                foreach (QPanel panelData in PanelsData.Where(p => p.PanelSN > panel.PanelSN))
                {
                    --panelData.PanelSN;
                }

                _ = PanelsData.Remove(panel);

                UpdateTargetIndicator();
            }
        }

        private bool CanDelete(QPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }
    }
}