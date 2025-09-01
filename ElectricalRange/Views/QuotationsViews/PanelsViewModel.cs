using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Services;
using ProjectsNow.Views.InquiriesViews;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.QuotationsViews
{
    public class PanelsViewModel : ViewModelBase
    {

        private string _Indicator = "-";
        private int _SelectedIndex;
        private QPanel _SelectedItem;
        private ObservableCollection<QPanel> _Items;

        private string _PanelSN;
        private string _PanelNameInfo;
        private string _PanelQty;
        private string _EnclosureType;
        private string _EnclosureHeight;
        private string _EnclosureWidth;
        private string _EnclosureDepth;
        private string _EnclosureIP;
        private string _PanelProfit;
        private string _PanelCost;
        private string _PanelPrice;
        private string _PanelsCost;
        private string _PanelsPrice;

        private ICollectionView _ItemsCollection;

        public PanelsViewModel(Quotation quotation, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            QuotationData = quotation;

            GetData();

            InfoCommand = new RelayCommand(Info, CanAccessInfo);
            TermsCommand = new RelayCommand(Terms, CanAccessTerms);
            QuotationItemsCommand = new RelayCommand(QuotationItems, CanAccessQuotationItems);
            CostSheetCommand = new RelayCommand(CostSheet, CanAccessCostSheet);
            PricesCommand = new RelayCommand(Prices, CanAccessPrices);
            PriceNoteCommand = new RelayCommand(PriceNote, CanAccessPriceNote);
            TargetPriceCommand = new RelayCommand(TargetPrice, CanAccessTargetPrice);
            RecalculateCommand = new RelayCommand(Recalculate, CanAccessRecalculate);
            PrintCommand = new RelayCommand(Print, CanAccessPrint);
            OptionsCommand = new RelayCommand(Options, CanAccessOptions);
            SubmitCommand = new RelayCommand(Submit, CanAccessSubmit);

            ItemsCommand = new RelayCommand<QPanel>(GetItems, CanAccessItems);
            PanelItemsCommand = new RelayCommand<QPanel>(PanelItems, CanAccessPanelItems);
            PanelCostSheetCommand = new RelayCommand<QPanel>(PanelCostSheet, CanAccessPanelCostSheet);
            LibraryCommand = new RelayCommand(Library, CanAccessLibrary);
            ImportCommand = new RelayCommand(Import, CanAccessImport);
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            SpecialPanelCommand = new RelayCommand(SpecialPanel, CanAccessSpecialPanel);
            EditCommand = new RelayCommand<QPanel>(Edit, CanAccessEdit);
            InsertUpCommand = new RelayCommand<QPanel>(InsertUp, CanAccessInsertUp);
            InsertDownCommand = new RelayCommand<QPanel>(InsertDown, CanAccessInsertDown);
            MoveUpCommand = new RelayCommand<QPanel>(MoveUp, CanAccessMoveUp);
            MoveDownCommand = new RelayCommand<QPanel>(MoveDown, CanAccessMoveDown);
            CopyCommand = new RelayCommand<QPanel>(Copy, CanAccessCopy);

            UpdateCommand = new RelayCommand(UpdateValues);
            DeleteCommand = new RelayCommand<QPanel>(Delete, CanAccessDelete);

            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData { get; }
        public Quotation QuotationData { get; }

        public Visibility HasItemsButtons
        {
            get
            {
                if (CanAccessItems(SelectedItem))
                    return Visibility.Visible;

                if (CanAccessPanelItems(SelectedItem))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }
        public Visibility HasDataButtons
        {
            get
            {
                if (CanAccessImport())
                    return Visibility.Visible;

                if (CanAccessLibrary())
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }
        public Visibility HasToolsButtons
        {
            get
            {
                if (SelectedItem == null)
                    return Visibility.Collapsed;

                if (CanAccessAdd())
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility HasInfoButtons
        {
            get
            {
                if (SelectedItem == null)
                    return Visibility.Collapsed;

                if (CanAccessEdit(SelectedItem))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility HasMovingButtons
        {
            get
            {
                if (SelectedItem == null)
                    return Visibility.Collapsed;

                if (CanAccessMoveUp(SelectedItem))
                    return Visibility.Visible;

                if (CanAccessMoveDown(SelectedItem))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
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
        public QPanel SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value)
                  .UpdateProperties(this,
                                    nameof(HasInfoButtons),
                                    nameof(HasItemsButtons),
                                    nameof(HasMovingButtons),
                                    nameof(HasToolsButtons));
        }
        public ObservableCollection<QPanel> Items
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

        public RelayCommand InfoCommand { get; }
        public RelayCommand TermsCommand { get; }
        public RelayCommand QuotationItemsCommand { get; }
        public RelayCommand CostSheetCommand { get; }
        public RelayCommand PricesCommand { get; }
        public RelayCommand PriceNoteCommand { get; }
        public RelayCommand TargetPriceCommand { get; }
        public RelayCommand RecalculateCommand { get; }
        public RelayCommand PrintCommand { get; }
        public RelayCommand OptionsCommand { get; }
        public RelayCommand SubmitCommand { get; }

        public RelayCommand<QPanel> ItemsCommand { get; }
        public RelayCommand<QPanel> PanelItemsCommand { get; }
        public RelayCommand<QPanel> PanelCostSheetCommand { get; }
        public RelayCommand LibraryCommand { get; }
        public RelayCommand ImportCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand SpecialPanelCommand { get; }
        public RelayCommand<QPanel> EditCommand { get; }
        public RelayCommand<QPanel> InsertUpCommand { get; }
        public RelayCommand<QPanel> InsertDownCommand { get; }
        public RelayCommand<QPanel> MoveUpCommand { get; }
        public RelayCommand<QPanel> MoveDownCommand { get; }
        public RelayCommand<QPanel> CopyCommand { get; }
        public RelayCommand<QPanel> DeleteCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string PanelSN
        {
            get => _PanelSN;
            set
            {
                if (SetValue(ref _PanelSN, value))
                {
                    FilterProperty = nameof(PanelSN);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelNameInfo
        {
            get => _PanelNameInfo;
            set
            {
                if (SetValue(ref _PanelNameInfo, value))
                {
                    FilterProperty = nameof(PanelNameInfo);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelQty
        {
            get => _PanelQty;
            set
            {
                if (SetValue(ref _PanelQty, value))
                {
                    FilterProperty = nameof(PanelQty);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureType
        {
            get => _EnclosureType;
            set
            {
                if (SetValue(ref _EnclosureType, value))
                {
                    FilterProperty = nameof(EnclosureType);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureHeight
        {
            get => _EnclosureHeight;
            set
            {
                if (SetValue(ref _EnclosureHeight, value))
                {
                    FilterProperty = nameof(EnclosureHeight);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureWidth
        {
            get => _EnclosureWidth;
            set
            {
                if (SetValue(ref _EnclosureWidth, value))
                {
                    FilterProperty = nameof(EnclosureWidth);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureDepth
        {
            get => _EnclosureDepth;
            set
            {
                if (SetValue(ref _EnclosureDepth, value))
                {
                    FilterProperty = nameof(EnclosureDepth);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureIP
        {
            get => _EnclosureIP;
            set
            {
                if (SetValue(ref _EnclosureIP, value))
                {
                    FilterProperty = nameof(EnclosureIP);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelProfit
        {
            get => _PanelProfit;
            set
            {
                if (SetValue(ref _PanelProfit, value))
                {
                    FilterProperty = nameof(PanelProfit);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelCost
        {
            get => _PanelCost;
            set
            {
                if (SetValue(ref _PanelCost, value))
                {
                    FilterProperty = nameof(PanelCost);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelsCost
        {
            get => _PanelsCost;
            set
            {
                if (SetValue(ref _PanelsCost, value))
                {
                    FilterProperty = nameof(PanelsCost);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelPrice
        {
            get => _PanelPrice;
            set
            {
                if (SetValue(ref _PanelPrice, value))
                {
                    FilterProperty = nameof(PanelPrice);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelsPrice
        {
            get => _PanelsPrice;
            set
            {
                if (SetValue(ref _PanelsPrice, value))
                {
                    FilterProperty = nameof(PanelsPrice);
                    ItemsCollection.Refresh();
                }
            }
        }

        private bool DataFilter(object item)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = "" + GetType().GetProperty(columnName).GetValue(this);

            if (!value.Contains(checkValue.ToUpper()))
            {
                result = false;
            }

            return result;
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

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            Items = QPanelController.QuotationPanels(connection, QuotationData.Id);

            ResetSN(connection);
        }

        private void ResetSN(SqlConnection connection)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].PanelSN = i + 1;
            }

            _ = connection.Update(Items);
        }

        private void UpdateValues()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [Quotation].[Quotations(View)] " +
                           $"Where QuotationID = {QuotationData.QuotationID}";
            Quotation newData = connection.QueryFirstOrDefault<Quotation>(query);

            QuotationData.QuotationCost = newData.QuotationCost;
            QuotationData.QuotationPrice = newData.QuotationPrice;
            QuotationData.QuotationEstimatedPrice = newData.QuotationEstimatedPrice;
            QuotationData.QuotationFinalPrice = newData.QuotationFinalPrice;
            QuotationData.QuotationDiscountValue = newData.QuotationDiscountValue;
            QuotationData.QuotationVATValue = newData.QuotationVATValue;
        }

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
            //using SqlConnection connection = new(Database.ConnectionString);
            //ResetSN(connection);
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }

        private void Info()
        {
            Inquiry inquiry;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Inquiry].[Inquiries(View)] Where InquiryID = {QuotationData.InquiryID}";
                inquiry = connection.QueryFirstOrDefault<Inquiry>(query);
            }

            if (UserData.Access(inquiry))
            {
                Navigation.To(new InquiryView(inquiry, QuotationData), ViewData);
            }
        }
        private bool CanAccessInfo()
        {
            return QuotationServices.CanAccessInfo(QuotationData);
        }

        private void Terms()
        {
            QuotationServices.Terms(QuotationData, ViewData);
        }
        private bool CanAccessTerms()
        {
            return QuotationServices.CanAccessTerms(QuotationData);
        }

        private void QuotationItems()
        {
            QuotationServices.GetItems(QuotationData, ViewData);
        }
        private bool CanAccessQuotationItems()
        {
            return QuotationServices.CanAccessGetItems(QuotationData);
        }

        private void CostSheet()
        {
            QuotationServices.CostSheet(QuotationData, ViewData);

        }
        private bool CanAccessCostSheet()
        {
            return QuotationServices.CanAccessCostSheet(QuotationData);
        }

        private void Prices()
        {
            QuotationServices.Prices(QuotationData);
        }
        private bool CanAccessPrices()
        {
            return QuotationServices.CanAccessPrices(QuotationData);
        }

        private void PriceNote()
        {
            QuotationServices.PriceNote(QuotationData);
        }
        private bool CanAccessPriceNote()
        {
            return QuotationServices.CanAccessPriceNote(QuotationData);
        }

        private void TargetPrice()
        {
            Navigation.OpenPopup(new TargetPriceView(QuotationData, Items), PlacementMode.MousePoint, false);
        }
        private bool CanAccessTargetPrice()
        {
            if (Items.Count == 0)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void Recalculate()
        {
            Navigation.OpenLoading();

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                List<QItem> items = QItemController.QuotationRecalculateItems(connection, QuotationData.QuotationID);

                if (items.Count == 0)
                {
                    Navigation.CloseLoading();
                    return;
                }

                string query = "";
                foreach (QItem item in items)
                {
                    query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemCost = {item.ReferenceCost}, ItemDiscount = {item.ReferenceDiscount} where ItemID = {item.ItemID} ;";
                    Navigation.LoadingText($"Update {items.IndexOf(item) + 1}/{items.Count}");
                    Events.ShowEvent.Do();
                }

                _ = connection.Execute(query);
            }

            UpdateValues();
            Navigation.CloseLoading();
        }
        private bool CanAccessRecalculate()
        {
            if (Items.Count == 0)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void Print()
        {
            QuotationServices.Print(QuotationData, ViewData);
        }
        private bool CanAccessPrint()
        {
            if (QuotationData == null)
                return false;

            return true;
        }

        private void Options()
        {
            QuotationServices.Options(QuotationData, ViewData);
        }
        private bool CanAccessOptions()
        {
            if (QuotationData == null)
                return false;

            return true;
        }

        private void Submit()
        {
            QuotationServices.Submit(QuotationData, null, Statuses.All);
        }
        private bool CanAccessSubmit()
        {
            return QuotationServices.CanAccessSubmit(QuotationData);
        }


        private void GetItems(QPanel panel)
        {
            Navigation.To(new ItemsView(QuotationData, panel), ViewData);
        }
        private bool CanAccessItems(QPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void PanelItems(QPanel panel)
        {
            decimal maxRow = 44;
            List<QItem> items;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Quotation].[PanelsItemsList(View)] " +
                               $"Where PanelID = {panel.PanelID} " +
                               $"Order By Code";

                items = connection.Query<QItem>(query).ToList();
            }

            for (int i = 1; i <= items.Count; i++)
            {
                items[i - 1].ItemSort = i;
            }

            decimal pagesNumber = items.Count / maxRow;
            if (pagesNumber - Math.Truncate(pagesNumber) != 0)
            {
                pagesNumber = Math.Truncate(pagesNumber) + 1;
            }

            List<FrameworkElement> elements = new();
            if (pagesNumber != 0)
            {
                for (int i = 1; i <= pagesNumber; i++)
                {
                    Printing.PanelsItems panelsItems = new() { QuotationData = QuotationData, PanelData = panel, Items = items.Where(p => p.ItemSort > ((i - 1) * maxRow) && p.ItemSort <= (i * maxRow)).ToList(), Page = i, Pages = Convert.ToInt32(pagesNumber) };
                    elements.Add(panelsItems);
                }

                Printing.Print.PrintPreview(elements, $"Panel {panel.PanelSN}-{panel.PanelName} Items", ViewData);
            }
            else
            {
                _ = MessageView.Show("Items", "There is no items!!", MessageViewButton.OK, MessageViewImage.Warning);
            }
        }
        private bool CanAccessPanelItems(QPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void PanelCostSheet(QPanel panel)
        {
            Navigation.OpenPopup(new SelectCostSheetView(QuotationData, panel, ViewData), PlacementMode.MousePoint, false);
        }
        private bool CanAccessPanelCostSheet(QPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void Library()
        {

        }
        private bool CanAccessLibrary()
        {
            return false;
        }

        private void Import()
        {
            Navigation.To(new ImportPanelsView(QuotationData, Items), ViewData);
        }
        private bool CanAccessImport()
        {
            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void Add()
        {
            QPanel panel = new()
            {
                QuotationID = QuotationData.Id,
                PanelSN = Items.Count + 1,
                Source = QuotationData.PowerVoltage,
                Frequency = QuotationData.Frequency,
                Busbar = QuotationData.TinPlating,
                NeutralSize = QuotationData.NeutralSize,
                EarthSize = QuotationData.EarthSize,
                EarthingSystem = QuotationData.EarthingSystem,
            };

            Navigation.To(new PanelView(QuotationData, panel, Items), ViewData);
        }
        private bool CanAccessAdd()
        {
            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void SpecialPanel()
        {
            QPanel panel = new()
            {
                IsSpecial = true,
                QuotationID = QuotationData.Id,
                PanelSN = Items.Count + 1,
                Source = QuotationData.PowerVoltage,
                Frequency = QuotationData.Frequency,
                Busbar = QuotationData.TinPlating,
                NeutralSize = QuotationData.NeutralSize,
                EarthSize = QuotationData.EarthSize,
                EarthingSystem = QuotationData.EarthingSystem,
            };

            Navigation.OpenPopup(new SpecialPanelView(QuotationData, panel, Items), PlacementMode.Center, true);
            Navigation.ClosePopupEvent += UpdateValues;
        }
        private bool CanAccessSpecialPanel()
        {
            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void Edit(QPanel panel)
        {
            if (panel.IsSpecial)
            {
                Navigation.OpenPopup(new SpecialPanelView(QuotationData, panel, Items), PlacementMode.Center, true);

                Navigation.ClosePopupEvent += UpdateValues;
            }
            else
            {
                Navigation.To(new PanelView(QuotationData, panel, Items), ViewData);
            }
        }
        private bool CanAccessEdit(QPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void InsertUp(QPanel panel)
        {
            QPanel newPanel = new()
            {
                QuotationID = QuotationData.Id,
                PanelSN = panel.PanelSN,
                Source = QuotationData.PowerVoltage,
                Frequency = QuotationData.Frequency,
                Busbar = QuotationData.TinPlating,
                NeutralSize = QuotationData.NeutralSize,
                EarthSize = QuotationData.EarthSize,
                EarthingSystem = QuotationData.EarthingSystem,
            };

            Navigation.To(new PanelView(QuotationData, newPanel, Items), ViewData);
        }
        private bool CanAccessInsertUp(QPanel panel)
        {
            if (panel == null)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void InsertDown(QPanel panel)
        {
            QPanel newPanel = new()
            {
                QuotationID = QuotationData.Id,
                PanelSN = panel.PanelSN + 1,
                Source = QuotationData.PowerVoltage,
                Frequency = QuotationData.Frequency,
                Busbar = QuotationData.TinPlating,
                NeutralSize = QuotationData.NeutralSize,
                EarthSize = QuotationData.EarthSize,
                EarthingSystem = QuotationData.EarthingSystem,
            };

            Navigation.To(new PanelView(QuotationData, newPanel, Items), ViewData);
        }
        private bool CanAccessInsertDown(QPanel panel)
        {
            if (panel == null)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void MoveUp(QPanel panel)
        {
            panel.PanelSN -= 1;

            QPanel affectedPanel = Items.FirstOrDefault(p => p.PanelSN == panel.PanelSN && p.PanelID != panel.PanelID);
            affectedPanel.PanelSN++;

            ItemsCollection.Refresh();

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(panel);
            _ = connection.Update(affectedPanel);
        }
        private bool CanAccessMoveUp(QPanel panel)
        {
            if (panel == null)
                return false;

            if (ItemsCollection.Cast<QPanel>().ToList().IndexOf(panel) == 0)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void MoveDown(QPanel panel)
        {
            panel.PanelSN += 1;

            QPanel affectedPanel = Items.FirstOrDefault(p => p.PanelSN == panel.PanelSN && p.PanelID != panel.PanelID);
            affectedPanel.PanelSN--;

            ItemsCollection.Refresh();

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(panel);
            _ = connection.Update(affectedPanel);
        }
        private bool CanAccessMoveDown(QPanel panel)
        {
            if (panel == null)
                return false;

            if (ItemsCollection.Cast<QPanel>().ToList().IndexOf(panel) == ItemsCollection.Cast<QPanel>().Count() - 1)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void Copy(QPanel panel)
        {
            Navigation.OpenPopup(new CopyPanelView(panel, Items), PlacementMode.Center, true);
            Navigation.ClosePopupEvent += UpdateValues;
        }
        private bool CanAccessCopy(QPanel panel)
        {
            if (panel == null)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void Delete(QPanel panel)
        {
            MessageBoxResult result = MessageView.Show("Deleting", $"Are you sure you want to \ndelete {panel.PanelName} ?", MessageViewButton.YesNo, MessageViewImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Delete From [Quotation].[QuotationsPanels] Where PanelID = {panel.PanelID}; " +
                                   $"Delete From [Quotation].[QuotationsPanelsItems] Where PanelID = {panel.PanelID}; " +
                                   $"Delete From [Quotation].[QuotationsOptionsPanels] Where PanelID = {panel.PanelID}; ";

                    _ = connection.Execute(query);

                    _ = Items.Remove(panel);

                    ResetSN(connection);
                }

                UpdateValues();
            }
        }
        private bool CanAccessDelete(QPanel panel)
        {
            if (panel == null)
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }
    }
}