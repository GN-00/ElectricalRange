using Dapper.Contrib.Extensions;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.References;
using ProjectsNow.Enums;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Windows.JobOrderWindows.LookingForQuotations
{
    public partial class ItemWindow : Window
    {
        public Actions ActionData { get; set; }
        public QuotationRequest RequestData { get; set; }
        public QuotationRequestItem ItemData { get; set; }
        public ObservableCollection<QuotationRequestItem> ItemsData { get; set; }

        private QuotationRequestItem newItemData;
        private ObservableCollection<Reference> referencesData;
        public ItemWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newItemData = new QuotationRequestItem()
            {
                QuotationRequestId = RequestData.Id,
            };

            referencesData = AppData.ReferencesListData;
            if (referencesData == null)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                referencesData =
                    AppData.ReferencesListData =
                        new ObservableCollection<Reference>(ReferenceController.GetReferences(connection));
            }

            PartNumbersList.ItemsSource = referencesData;

            if (ActionData == Actions.Edit)
            {
                newItemData.Update(ItemData);
                PartNumbersList.Text = newItemData.Code;
            }

            DataContext = newItemData;
        }

        private void PartNumbersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PartNumbersList.SelectedItem is Reference reference)
            {
                newItemData.Code = reference.Code;
                newItemData.Description = reference.Description;
                newItemData.Unit = reference.Unit;
            }
            else
            {
                newItemData.Code =
                    newItemData.Description =
                         newItemData.Unit = null;
            }
        }

        private void Qty_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!(PartNumbersList.SelectedItem is Reference))
                return;

            if (ActionData == Actions.Edit)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Update(newItemData);
                }

                ItemData.Update(newItemData);

                Close();
            }
            else
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(newItemData);
                }

                if (ItemsData != null)
                {
                    ItemsData.Add(newItemData);
                }

                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
