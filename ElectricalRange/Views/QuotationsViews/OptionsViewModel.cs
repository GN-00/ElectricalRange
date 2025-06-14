using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

using Option = ProjectsNow.Data.Quotations.QuotationOption;
using Panel = ProjectsNow.Data.Quotations.QuotationOptionPanel;

namespace ProjectsNow.Views.QuotationsViews
{
    public class OptionsViewModel : ViewModelBase
    {
        private string _OptionsIndicator = "-";
        private string _PanelsIndicator = "-";

        private int _SelectedOptionIndex;
        private int _SelectedPanelIndex;

        private Option _SelectedOption;
        private Panel _SelectedPanel;

        private ObservableCollection<Option> _Options;
        private ObservableCollection<Panel> _Panels;

        private ICollectionView _OptionsCollection;
        private ICollectionView _PanelsCollection;

        public OptionsViewModel(Quotation quotation, IView view)
        {
            ViewData = view;
            QuotationData = quotation;
            UserData = Navigation.UserData;

            GetData();
            AddCommand = new RelayCommand(Add, CanAccessAdd);
            EditCommand = new RelayCommand<Option>(Edit, CanAccessEdit);
            DeleteCommand = new RelayCommand<Option>(Delete, CanAccessDelete);
            AddPanelCommand = new RelayCommand(AddPanel, CanAccessAddPanel);
            DeletePanelCommand = new RelayCommand<Panel>(DeletePanel, CanAccessDeletePanel);
            PrintCommand = new RelayCommand<Option>(Print, CanAccessPrint);
        }

        public User UserData { get; }
        public Quotation QuotationData { get; }

        public string OptionsIndicator
        {
            get => _OptionsIndicator;
            set => SetValue(ref _OptionsIndicator, value);
        }
        public string PanelsIndicator
        {
            get => _PanelsIndicator;
            set => SetValue(ref _PanelsIndicator, value);
        }
        public int SelectedOptionIndex
        {
            get => _SelectedOptionIndex;
            set
            {
                if (SetValue(ref _SelectedOptionIndex, value))
                {
                    UpdateOptionsIndicator();
                }
            }
        }
        public int SelectedPanelIndex
        {
            get => _SelectedPanelIndex;
            set
            {
                if (SetValue(ref _SelectedPanelIndex, value))
                {
                    UpdatePanelsIndicator();
                }
            }
        }
        public Option SelectedOption
        {
            get => _SelectedOption;
            set
            {
                if (SetValue(ref _SelectedOption, value))
                {
                    PanelsCollection.Refresh();
                }
            }
        }
        public Panel SelectedPanel
        {
            get => _SelectedPanel;
            set => SetValue(ref _SelectedPanel, value);
        }
        public ObservableCollection<Option> Options
        {
            get => _Options;
            private set => SetValue(ref _Options, value);
        }
        public ObservableCollection<Panel> Panels
        {
            get => _Panels;
            private set => SetValue(ref _Panels, value);
        }
        public ICollectionView OptionsCollection
        {
            get => _OptionsCollection;
            set => SetValue(ref _OptionsCollection, value);
        }
        public ICollectionView PanelsCollection
        {
            get => _PanelsCollection;
            set => SetValue(ref _PanelsCollection, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand<Option> EditCommand { get; }
        public RelayCommand<Option> DeleteCommand { get; }
        public RelayCommand AddPanelCommand { get; }
        public RelayCommand<Panel> DeletePanelCommand { get; }
        public RelayCommand<Option> PrintCommand { get; }


        #region Data Filter
        private bool DataFilter(object item)
        {
            if (SelectedOption == null)
                return false;

            bool result = false;
            string columnName = "OptionID";

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = SelectedOption.ID.ToString();

            if (value == checkValue)
            {
                result = true;
            }

            return result;
        }

        #endregion

        private void GetData()
        {
            string query;
            using (SqlConnection connection = new(Data.Database.ConnectionString))
            {
                query = $"Select * From [Quotation].[QuotationsOptions] " +
                        $"Where QuotationID = {QuotationData.Id} " +
                        $"Order By Number";
                Options = new ObservableCollection<Option>(connection.Query<Option>(query));

                query = $"Select * From [Quotation].[OptionsPanels] " +
                        $"Where OptionID In " +
                        $"(Select ID From [Quotation].[QuotationsOptions] Where QuotationID = {QuotationData.Id}) " +
                        $"Order By PanelSN";
                Panels = new ObservableCollection<Panel>(connection.Query<Panel>(query));
            }

            foreach (Option option in Options)
            {
                option.QuotationEstimatedPrice = Panels.Where(x => x.OptionID == option.ID).Sum(x => x.EstimatedPrice);
            }

            CreateCollectionView();
        }
        private void CreateCollectionView()
        {
            OptionsCollection = CollectionViewSource.GetDefaultView(Options);
            OptionsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
            OptionsCollection.CollectionChanged += OptionsCollectionChanged;

            PanelsCollection = CollectionViewSource.GetDefaultView(Panels);
            PanelsCollection.Filter = new Predicate<object>(DataFilter);
            PanelsCollection.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
            PanelsCollection.CollectionChanged += PanelsCollectionChanged;
        }
        private void OptionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateOptionsIndicator();
        }
        private void PanelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePanelsIndicator();

            if (SelectedOption != null)
            {
                SelectedOption.QuotationEstimatedPrice = Panels.Where(x => x.OptionID == SelectedOption.ID).Sum(x => x.EstimatedPrice);
            }
        }
        private void UpdateOptionsIndicator()
        {
            OptionsIndicator = DataGridIndicator.Get(SelectedOptionIndex, OptionsCollection);
        }
        private void UpdatePanelsIndicator()
        {
            PanelsIndicator = DataGridIndicator.Get(SelectedPanelIndex, PanelsCollection);
        }

        private void Add()
        {
            Option option = new()
            {
                QuotationID = QuotationData.Id,
                Number = Options.Count,
            };
            Navigation.OpenPopup(new OptionView(option, Options), PlacementMode.Center, false);
        }
        private bool CanAccessAdd()
        {
            if (QuotationData == null)
                return false;

            if (QuotationData.EstimationID != UserData.EmployeeId)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void Edit(Option option)
        {
            Navigation.OpenPopup(new OptionView(option, Options), PlacementMode.Center, false);
        }
        private bool CanAccessEdit(Option option)
        {
            if (option == null)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.EstimationID != UserData.EmployeeId)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void Delete(Option option)
        {
            string query;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Delete From [Quotation].[QuotationsOptions] Where ID = {option.ID}; ";
                query += $"Delete From [Quotation].[QuotationsOptionsPanels] Where OptionID = {option.ID} ";
                _ = connection.Execute(query);
            }

            _ = Options.Remove(option);
            ObservableCollection<Panel> affectedPanels =
                new(Panels.Where(x => x.OptionID == option.ID));

            foreach (Panel panel in affectedPanels)
            {
                Panels.Remove(panel);
            }
        }
        private bool CanAccessDelete(Option option)
        {
            if (option == null)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.EstimationID != UserData.EmployeeId)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void AddPanel()
        {
            Navigation.OpenPopup(new OptionPanelsView(SelectedOption, Panels), PlacementMode.Center, true);
        }
        private bool CanAccessAddPanel()
        {
            if (SelectedOption == null)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.EstimationID != UserData.EmployeeId)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void DeletePanel(Panel panel)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Delete(panel);
            }

            _ = Panels.Remove(panel);
        }
        private bool CanAccessDeletePanel(Panel panel)
        {
            if (panel == null)
                return false;

            if (QuotationData == null)
                return false;

            if (QuotationData.EstimationID != UserData.EmployeeId)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            return true;
        }

        private void Print(Option option)
        {
            IEnumerable<Panel> panels = Panels.Where(p => p.OptionID == option.ID);
            if (panels.Count() != 0)
            {
                string PanelsIDs = "";
                foreach (Panel panel in panels)
                {
                    PanelsIDs += panel.PanelID + ", ";
                }
                PanelsIDs = PanelsIDs.Substring(0, PanelsIDs.Length - 2);

                Option optionData;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Select * From [Quotation].[OptionsCost] Where OptionID = {option.ID}";
                    optionData = connection.QueryFirstOrDefault<Option>(query);
                }

                Quotation newQuotation = new();
                newQuotation.Update(QuotationData);
                newQuotation.QuotationCode = newQuotation.QuotationCode.Insert(6, SelectedOption.Code);
                newQuotation.QuotationCost = optionData.QuotationCost;
                newQuotation.QuotationPrice = optionData.QuotationPrice;
                newQuotation.QuotationEstimatedPrice = optionData.QuotationEstimatedPrice;
                newQuotation.QuotationFinalPrice = optionData.QuotationFinalPrice;
                newQuotation.QuotationDiscountValue = optionData.QuotationDiscountValue;
                newQuotation.QuotationVATValue = optionData.QuotationVATValue;
                newQuotation.OptionCode = SelectedOption.Code;
                newQuotation.OptionName = SelectedOption.Name;

                List<BillPanel> panelsData;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    panelsData = BillPanelController.GetBillPanels(connection, PanelsIDs);
                }

                Navigation.To(new PrintQuotationView(newQuotation, panelsData), ViewData);
            }
            else
            {
                _ = MessageView.Show("Panels!!", "This option do not have panels!", MessageViewButton.OK, MessageViewImage.Warning);
            }
        }
        private bool CanAccessPrint(Option option)
        {
            if (option == null)
                return false;

            return true;
        }
    }
}