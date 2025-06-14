using Dapper;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Data;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class CategoryDiscountWindow : Window
    {
        public Category CategoryData { get; set; }
        public CategoryDiscountWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NewDiscountInput.Text = CategoryData.Discount.ToString();
            DataContext = CategoryData;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string query;
            int discount = int.Parse(NewDiscountInput.Text);
            if (discount > -1)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    CategoryData.Discount = discount;
                    query = $"Update [Reference].[References] Set " +
                            $"Discount = {CategoryData.Discount} " +
                            $"Where Category = '{CategoryData.Name}'";

                    _ = connection.Execute(query);
                }

                Close();
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void NewDiscountInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewDiscountInput.Text))
            {
                NewDiscountInput.Text = CategoryData.Discount.ToString();
            }
            else
            {
                int discount = int.Parse(NewDiscountInput.Text);
                if (discount > 100)
                {
                    NewDiscountInput.Text = CategoryData.Discount.ToString();
                }
            }
        }
        private void NewDiscountInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
