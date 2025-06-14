using ProjectsNow.Data.Quotations;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectsNow.Printing.QuotationPages.CostSheet
{
    public partial class CostSheetPage : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public List<Item> Items { get; set; }
        public Quotation QuotationData { get; set; }

        public CostSheetPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            Border border;
            TextBlock textBlock;

            DataContext = QuotationData;

            for (int i = 0; i < Items.Count; i++)
            {
                ItemsList.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });

                textBlock = new TextBlock() { Text = Items[i].Code.ToString(), Margin = new Thickness(5, 0, 5, 0) };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 1);
                _ = ItemsList.Children.Add(border);

                textBlock = new TextBlock() { Text = Items[i].Description.ToString(), Margin = new Thickness(5, 0, 5, 0) };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 2);
                _ = ItemsList.Children.Add(border);

                textBlock = new TextBlock() { Text = Items[i].PublicPrice.ToString("N2"), HorizontalAlignment = HorizontalAlignment.Center };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 3);
                _ = ItemsList.Children.Add(border);

                textBlock = new TextBlock() { Text = Items[i].Discount.ToString() + " %", HorizontalAlignment = HorizontalAlignment.Center };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 4);
                _ = ItemsList.Children.Add(border);

                textBlock = new TextBlock() { Text = Items[i].Price.ToString("N2"), HorizontalAlignment = HorizontalAlignment.Center };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 5);
                _ = ItemsList.Children.Add(border);

                textBlock = new TextBlock() { Text = Items[i].Qty.ToString("N2"), HorizontalAlignment = HorizontalAlignment.Center };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 6);
                _ = ItemsList.Children.Add(border);

                textBlock = new TextBlock() { Text = Items[i].Total.ToString("N2"), HorizontalAlignment = HorizontalAlignment.Center };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 7);
                _ = ItemsList.Children.Add(border);
            }

            PageView.Text = Page.ToString();
            PagesView.Text = Pages.ToString();

        }
    }
}
