using Dapper;
using Dapper.Contrib.Extensions;

using DocumentFormat.OpenXml.Packaging;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Printing;
using ProjectsNow.Printing.JobOrderPages;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Views.JobOrdersViews
{
    internal class QualityViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private QCPanel _SelectedItem;
        private ObservableCollection<QCPanel> _Items;
        private ICollectionView _ItemsCollection;

        public QualityViewModel(JobOrder order, IView view)
        {
            ViewData = view;
            OrderData = order;

            GetData();

            PrintCommand = new RelayCommand<QCPanel>(Print, CanPrint);
            PrintAllCommand = new RelayCommand(PrintAll, CanPrintAll);
        }


        public User UserData => Navigation.UserData;
        public JobOrder OrderData { get; set; }
        public bool IsEditable { get => UserData.ModifyQuality; }
        public ObservableCollection<string> Voltages { get; private set; }
        public ObservableCollection<string> Currents { get; private set; }
        public ObservableCollection<string> Enclosures { get; private set; }
        public ObservableCollection<string> Installations { get; private set; }
        public ObservableCollection<string> IPs { get; private set; }


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
                    Save();
                }
            }
        }
        public QCPanel SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<QCPanel> Items
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

        public RelayCommand<QCPanel> PrintCommand { get; }
        public RelayCommand PrintAllCommand { get; }

        private void GetData()
        {
            Navigation.OpenLoading(Visibility.Visible, "Loading...");

            string query;
            IEnumerable<QCPanel> items;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [JobOrder].[QualityPanels(View)] " +
                        $"Where JobOrderId = {OrderData.Id} " +
                        $"Order By SN";

                items = connection.Query<QCPanel>(query);

                if (items.Count() == 0)
                {
                    query = $"Select * From [JobOrder].[CreateNewQualityPanels] " +
                            $"Where JobOrderId = {OrderData.Id} " +
                            $"Order By SN";

                    items = connection.Query<QCPanel>(query);

                    connection.Insert(items);
                }

                query = "Select Voltage From [JobOrder].[QualityPanels] Group By Voltage Order By Voltage";
                Voltages = new ObservableCollection<string>(connection.Query<string>(query));

                query = "Select [Current] From [JobOrder].[QualityPanels] Group By [Current] Order By [Current]";
                Currents = new ObservableCollection<string>(connection.Query<string>(query));

                query = "Select Enclosure From [JobOrder].[QualityPanels] Group By Enclosure Order By Enclosure";
                Enclosures = new ObservableCollection<string>(connection.Query<string>(query));

                query = "Select Installation From [JobOrder].[QualityPanels] Group By Installation Order By Installation";
                Installations = new ObservableCollection<string>(connection.Query<string>(query));

                query = "Select IP From [JobOrder].[QualityPanels] Group By IP Order By IP";
                IPs = new ObservableCollection<string>(connection.Query<string>(query));
            }

            Items = new ObservableCollection<QCPanel>(items);

            if (Items.Count != 0)
                SelectedItem = Items[0];

            Navigation.CloseLoading();
        }
        private void Save()
        {
            if (SelectedItem == null)
                return;

            if (SelectedItem.Id == 0)
                return;

            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(SelectedItem);
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.SortDescriptions.Add(new SortDescription("SN", ListSortDirection.Ascending));
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

        private void Print(QCPanel panel)
        {
            PageBase element = new FinalInspectionReport(panel);
            element.SetPage(1, 1);
            Printing.Print.PrintPreview(element, $"FIR JO#{OrderData.Code} -{panel.NameInfo}-", ViewData);
        }
        private bool CanPrint(QCPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void PrintAll()
        {
            if (Items.Count == 0)
                return;

            PageBase element;
            List<FrameworkElement> pages = new();

            foreach (QCPanel panel in Items)
            {
                element = new FinalInspectionReport(panel);
                element.SetPage(Items.IndexOf(panel) + 1, Items.Count);
                pages.Add(element);
            }

            Printing.Print.PrintPreview(pages, $"FIR JO#{OrderData.Code} All Panels", ViewData);
        }

        private bool CanPrintAll()
        {
            return true;
        }

    }
}