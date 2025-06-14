using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class CategoriesDiscountsWindow : Window
    {
        private CollectionViewSource viewData;
        private ObservableCollection<Category> categories;
        public CategoriesDiscountsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string query = $"Select * From [Reference].[CategoriesDiscounts] Order By Name";
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                categories = new ObservableCollection<Category>(connection.Query<Category>(query));
            }

            viewData = new CollectionViewSource() { Source = categories };
            CategoriesList.ItemsSource = viewData.View;

            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (categories.Count == 0)
            {
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = CategoriesList.SelectedIndex;
            if (selectedIndex == -1)
            {
                Navigation.Text = $"Categories: {viewData.View.Cast<object>().Count()}";
            }
            else
            {
                Navigation.Text = $"Category: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
            }
        }
        private void CategoriesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int selectedIndex = CategoriesList.SelectedIndex;
            if (selectedIndex == -1)
            {
                Navigation.Text = $"Categories: {viewData.View.Cast<object>().Count()}";
            }
            else
            {
                Navigation.Text = $"Category: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ChangeDiscount_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesList.SelectedItem is Category categoryData)
            {
                CategoryDiscountWindow categoryDiscountWindow = new()
                {
                    CategoryData = categoryData,
                };
                _ = categoryDiscountWindow.ShowDialog();
            }
        }
    }
}
