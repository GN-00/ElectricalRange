using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectsNow.Printing
{
    public partial class QuotationsItems : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public List<QItem> Items { get; set; }
        public Quotation QuotationData { get; set; }

        public QuotationsItems()
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
                ItemsList.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(21) });

                textBlock = new TextBlock() { Text = Items[i].ItemSort.ToString(), HorizontalAlignment = HorizontalAlignment.Center };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 0);
                _ = ItemsList.Children.Add(border);

                textBlock = new TextBlock() { Text = Items[i].Code.ToString(), Margin = new Thickness(5, 0, 5, 0) };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 1);
                _ = ItemsList.Children.Add(border);

                textBlock = new TextBlock() { Text = Items[i].Description.ToString(), Margin = new Thickness(5, 0, 5, 0) };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 2);
                _ = ItemsList.Children.Add(border);

                textBlock = new TextBlock() { Text = Items[i].ItemQty.ToString(), HorizontalAlignment = HorizontalAlignment.Center };
                border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 0, 1, 1), Child = textBlock };
                Grid.SetRow(border, i + 1); Grid.SetColumn(border, 3);
                _ = ItemsList.Children.Add(border);
            }

            PageView.Text = Page.ToString();
            PagesView.Text = Pages.ToString();
        }
    }
}
