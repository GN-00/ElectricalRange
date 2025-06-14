using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Windows.Data;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class RevisionsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Revision _SelectedItem;
        private ObservableCollection<Revision> _Items;
        private ICollectionView _ItemsCollection;

        public RevisionsViewModel(Quotation quotation, IView checkPoint)
        {
            QuotationData = quotation;
            ViewData = checkPoint;
            GetData();

            InfoCommand = new RelayCommand<Revision>(Info, CanInfo);
        }

        public Quotation QuotationData { get; }
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
        public Revision SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Revision> Items
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

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select QuotationID As Id, " +
                           $"QuotationRevise as Number, " +
                           $"QuotationCode as Code, " +
                           $"QuotationReviseDate as Date, " +
                           $"QuotationEstimatedPrice " +
                           $"From [Quotation].[Quotations(View)] " +
                           $"Where InquiryID = {QuotationData.InquiryID} " +
                           $"And QuotationStatus = 'Revision' " +
                           $"Order By QuotationRevise ";

            Items = new ObservableCollection<Revision>(connection.Query<Revision>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Ascending));
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

        public RelayCommand<Revision> InfoCommand { get; }

        private void Info(Revision revision)
        {
            Quotation quotation;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Quotation].[Quotations(View)] Where QuotationID = {SelectedItem.Id}";
                quotation = connection.QueryFirstOrDefault<Quotation>(query);
            }

            Navigation.To(new PanelsView(quotation), ViewData);
            Navigation.ClosePopup();
        }
        private bool CanInfo(Revision revision)
        {
            if (revision == null)
                return false;

            return true;
        }
    }
}