using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.JobOrdersViews
{
    public class WarrantyViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private WPanel _SelectedItem;
        private ObservableCollection<WPanel> _Items;
        private ICollectionView _ItemsCollection;

        public WarrantyViewModel(Warranty warranty, ObservableCollection<Warranty> warranties, IView view)
        {
            ViewData = view;
            WarrantyData = warranty;
            WarrantiesData = warranties;
            NewData.Update(WarrantyData);

            GetData();

            AddPanelsCommand = new RelayCommand(AddPanels, CanAddPanels);
            DeletePanelCommand = new RelayCommand<WPanel>(DeleteItem, CanDeleteItem);

            CreateCommand = new RelayCommand(Create, CanAccessCreate);
            PrintCommand = new RelayCommand(Print, CanAccessPrint);
        }

        public User UserData => Navigation.UserData;
        public Warranty WarrantyData { get; }
        public Warranty NewData { get; } = new Warranty();
        private ObservableCollection<Warranty> WarrantiesData { get; }
        public ObservableCollection<WPanel> PanelsData { get; set; }
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
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public bool IsNew => NewData.Code == "-New Warranty-";
        public bool IsEditing => IsNew;

        public RelayCommand AddPanelsCommand { get; }
        public RelayCommand<WPanel> EditItemCommand { get; }
        public RelayCommand<WPanel> DeletePanelCommand { get; }

        public RelayCommand CreateCommand { get; }
        public RelayCommand PrintCommand { get; }

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select PanelID As Id," +
                    $"       PanelSN As SN, " +
                    $"       PanelName As Name, " +
                    $"       PanelQty As Qty, " +
                    $"       EnclosureType As Enclosure " +
                    $"From [JobOrder].[Panels(View)] " +
                    $"Where JobOrderID = {NewData.JobOrderId} And " +
                    $"PanelID Not In " +
                    $"(Select PanelId From [JobOrder].[WarrantiesPanels] " +
                    $"Where JobOrderID = {NewData.JobOrderId}) " +
                    $"Order By PanelSN ";
            PanelsData = new ObservableCollection<WPanel>(connection.Query<WPanel>(query));

            if (NewData.Code == "-New Warranty-")
            {
                Items = new ObservableCollection<WPanel>();
            }
            else
            {
                query = $"Select * From [JobOrder].[WarrantiesPanels(View)] " +
                        $"Where WarrantyId  = {NewData.Id} ";

                Items = new ObservableCollection<WPanel>(connection.Query<WPanel>(query));
            }
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

        private void AddPanels()
        {
            Navigation.OpenPopup(new WarrantyPanelsView(Items, PanelsData), PlacementMode.Center, true);
        }
        private bool CanAddPanels()
        {
            if (!UserData.ModifyJobOrders)
                return false;

            return IsEditing;
        }

        private void DeleteItem(WPanel panel)
        {
            MessageBoxResult result =
                MessageView.Show($"Delete",
                                 $"Are you sure want to Delete\n{panel.Name}?",
                                 MessageViewButton.YesNo,
                                 MessageViewImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Items.Remove(panel);

                WPanel panelData = new()
                {
                    Id = panel.PanelId,
                    Name = panel.Name,
                    Qty = panel.Qty,
                    Enclosure = panel.Enclosure,
                };

                PanelsData.Add(panelData);
            }
        }
        private bool CanDeleteItem(WPanel panel)
        {
            if (panel == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return IsEditing;
        }

        private void Create()
        {
            bool isReady = true;
            string message = "Please Add:";
            if (!NewData.Installation && !NewData.Delivery && !NewData.Service && !NewData.Other)
            { message += $"\n  * Reason."; isReady = false; }
            if (Items.Count == 0) { message += $"\n  * Panels."; isReady = false; }

            if (isReady)
            {
                int warrantyNumber;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Select IsNUll(MAX(Number),0) As Number " +
                                   $"From [JobOrder].[Warranties] " +
                                   $"Where Year = {DateTime.Now.Year}";
                    warrantyNumber = connection.QueryFirstOrDefault<int>(query) + 1;

                    //ER/WCERT/002/06/2020
                    NewData.Number = warrantyNumber;
                    NewData.Month = DateTime.Now.Month;
                    NewData.Year = DateTime.Now.Year;
                    NewData.Code = $"ER/WCERT/{NewData.Number:000}/{NewData.Month:00}/{NewData.Year}";
                    NewData.Date = DateTime.Now;

                    if (!NewData.Other)
                        NewData.OtherInfo = null;

                    _ = connection.Insert(NewData);

                    foreach (WPanel panel in Items)
                    {
                        panel.WarrantyId = NewData.Id;
                    }

                    _ = connection.Insert(Items);
                }

                WarrantyData.Update(NewData);

                if (WarrantiesData != null)
                {
                    WarrantiesData.Add(WarrantyData);
                }

                OnPropertyChanged(nameof(IsNew));
                OnPropertyChanged(nameof(IsEditing));
            }
            else
            {
                _ = MessageView.Show("Saving", message, MessageViewButton.OK, MessageViewImage.Information);
            }
        }
        private bool CanAccessCreate()
        {
            if (!UserData.ModifyJobOrders)
                return false;

            return IsNew;
        }

        private void Print()
        {
            Services.WarrantyCertificateServices.PrintWarranty(NewData.Id, ViewData);
        }
        private bool CanAccessPrint()
        {
            return !IsNew;
        }
    }
}