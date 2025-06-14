using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.QuotationsStatus;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    public class CommunicationsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Communication _SelectedItem;
        private ObservableCollection<Communication> _Items;
        private ICollectionView _ItemsView;

        public CommunicationsViewModel(Quotation quotation, ICollectionView quotationsCollection, IView view)
        {
            ViewData = view;
            QuotationData = quotation;
            UserData = Navigation.UserData;
            QuotationsCollection = quotationsCollection;
            GetData();
            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Communication>(Edit, CanEdit);
            DeleteCommand = new RelayCommand<Communication>(Delete, CanDelete);
            ClosingCommand = new RelayCommand(Closing, CanClosing);
        }

        public CommunicationsViewModel(int inquiryId, IView view)
        {
            string query;
            Quotation quotation;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [QuotationsStatus].[Quotations(View)] " +
                        $"Where InquiryID = {inquiryId} ";
                quotation = connection.QueryFirstOrDefault<Quotation>(query);
            }

            ViewData = view;
            QuotationData = quotation;
            UserData = Navigation.UserData;
            QuotationsCollection = null;
            GetData();
            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Communication>(Edit, CanEdit);
            DeleteCommand = new RelayCommand<Communication>(Delete, CanDelete);
            ClosingCommand = new RelayCommand(Closing, CanClosing);
        }

        public ICollectionView QuotationsCollection { get; }

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
        public Quotation QuotationData { get; }
        public User UserData { get; }
        public Communication SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Communication> Items
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
        public ICollectionView ItemsView
        {
            get => _ItemsView;
            set => SetValue(ref _ItemsView, value);
        }
        public RelayCommand AddCommand { get; }
        public RelayCommand<Communication> EditCommand { get; }
        public RelayCommand<Communication> DeleteCommand { get; }
        public RelayCommand ClosingCommand { get; }

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Inquiry].[Communications(View)] " +
                    $"Where InquiryID = {QuotationData.InquiryID} " +
                    $"Order By Date Desc";

            Items = new ObservableCollection<Communication>(connection.Query<Communication>(query));
        }
        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            ItemsView.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsView);
        }

        private void Add()
        {
            Communication communication = new()
            {
                InquiryID = QuotationData.InquiryID.Value,
                Salesman = UserData.Name,
                SalesmanID = UserData.EmployeeId,
            };

            Navigation.OpenPopup(new CommunicationView(communication, Items), PlacementMode.Center, true);
        }
        private bool CanAdd()
        {
            if (UserData.ManageQuotationsUpdates)
                return true;

            if (UserData.EmployeeId == QuotationData.SalesmanID)
                return true;

            return false;
        }

        private void Edit(Communication item)
        {
            Navigation.OpenPopup(new CommunicationView(item, Items), PlacementMode.Center, true);
        }
        private bool CanEdit(Communication item)
        {
            if (item == null)
                return false;

            if (UserData.ManageQuotationsUpdates)
                return true;

            if (UserData.EmployeeId == QuotationData.SalesmanID)
                return true;

            return false;
        }

        private void Delete(Communication item)
        {
            MessageBoxResult result =
                MessageWindow.Show($"Delete",
                                   $"Are you sure want to Delete\n{item.Description}?",
                                   MessageWindowButton.YesNo,
                                   MessageWindowImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Delete(item);
                }

                Items.Remove(item);
            }
        }
        private bool CanDelete(Communication item)
        {
            return CanEdit(item);
        }


        private void Closing()
        {
            if (Items.Count > 0)
            {
                DateTime date = Items.Max(x => x.Date);
                var quotation = Items.FirstOrDefault(i => i.Date == date);
                QuotationData.CurrentStatus = quotation.Status;
                QuotationData.Note = quotation.Description;
            }
            else
            {
                if (QuotationData.CurrentStatus != "Win")
                {
                    QuotationData.CurrentStatus = "On Going";
                    QuotationData.Note = null;
                }
            }

            if (QuotationsCollection != null)
                QuotationsCollection.Refresh();
        }

        private bool CanClosing()
        {
            return true;
        }
    }
}