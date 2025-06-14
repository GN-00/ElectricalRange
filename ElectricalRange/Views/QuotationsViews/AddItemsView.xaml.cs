using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.References;
using ProjectsNow.Enums;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class AddItemsView : UserControl, IView
    {
        public QPanel PanelData { get; set; }
        public ObservableCollection<QItem> Items { get; set; }

        private AddItemsViewModel viewModel;
        public AddItemsView(QPanel panel, ObservableCollection<QItem> items)
        {
            InitializeComponent();
            viewModel = new AddItemsViewModel(panel, items);
            DataContext = viewModel;
        }

        private void DataGridCell_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }
        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //var dataGrid = ((DataGrid)sender);
            //var row = e.Row.Item as QItem;
            //if (viewModel.Items.IndexOf(row) == viewModel.Items.Count - 1)
            //{
            //    if (string.IsNullOrWhiteSpace(row.Code))
            //        dataGrid.CancelEdit();
            //}
        }
        private void Reference_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is Reference selectedReference)
            {
                if (viewModel.SelectedItem.Article1 == null)
                {
                    viewModel.SelectedItem.Article1 = selectedReference.Article1;
                }

                viewModel.SelectedItem.Code = selectedReference.Code;
                viewModel.SelectedItem.Article2 = selectedReference.Article2;
                viewModel.SelectedItem.Description = selectedReference.Description;
                viewModel.SelectedItem.Brand = selectedReference.Brand;
                viewModel.SelectedItem.Remarks = selectedReference.Remarks;
                viewModel.SelectedItem.ItemDiscount = selectedReference.Discount;
                viewModel.SelectedItem.ItemCost = selectedReference.Cost;
                viewModel.SelectedItem.Unit = selectedReference.Unit;

                if (viewModel.SelectedItem.ItemTable == null)
                {
                    viewModel.SelectedItem.ItemTable = "Details";
                }
            }
            else
            {
                if (viewModel.SelectedItem.Article1 == null)
                {
                    viewModel.SelectedItem.Article1 = null;
                }

                viewModel.SelectedItem.Code = null;
                viewModel.SelectedItem.Article2 = null;
                viewModel.SelectedItem.Description = null;
                viewModel.SelectedItem.Brand = null;
                viewModel.SelectedItem.Remarks = null;
                viewModel.SelectedItem.ItemDiscount = 0;
                viewModel.SelectedItem.ItemCost = 0;
                viewModel.SelectedItem.ItemTable = null;
                viewModel.SelectedItem.Unit = null;
            }
        }
        private void Article1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is string article)
            {
                if (article == null)
                {
                    viewModel.SelectedItem.Article1 = null;
                }
                else
                {
                    viewModel.SelectedItem.Article1 = article;
                }
            }
        }
        private void ItemTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is string value)
            {
                if (value == "Details" || value == "Enclosure" || value == $"{Tables.Accessories}")
                {
                    viewModel.SelectedItem.ItemTable = value;
                }
                else
                {
                    value = "Details";
                    viewModel.SelectedItem.ItemTable = value;
                }
            }
        }
    }
}
