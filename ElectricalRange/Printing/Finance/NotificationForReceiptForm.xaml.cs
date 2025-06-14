using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Application;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectsNow.Printing.Finance
{
    public partial class NotificationForReceiptForm : UserControl
    {
        public NotificationForReceiptForm(Notification info, ObservableCollection<ReceiptPanel> panels)
        {
            InitializeComponent();

            BackgroundImage.Source = AppData.CompanyWatermark;

            int row = 1;
            Border border;
            TextBlock textBlock;
            foreach (ReceiptPanel receiptPanel in panels)
            {
                PanelsList.RowDefinitions[row].Height = new GridLength(25);

                textBlock = new TextBlock()
                {
                    FontSize = 14,
                    Text = receiptPanel.SN.ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 0, 1, 1),
                    Child = textBlock,
                };
                Grid.SetRow(border, row);
                Grid.SetColumn(border, 0);
                PanelsList.Children.Add(border);

                textBlock = new TextBlock()
                {
                    FontSize = 14,
                    Margin = new Thickness(5,0,5,0),
                    Text = receiptPanel.NameInfo,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = textBlock,
                };
                Grid.SetRow(border, row);
                Grid.SetColumn(border, 1);
                PanelsList.Children.Add(border);

                textBlock = new TextBlock()
                {
                    FontSize = 14,
                    Text = receiptPanel.Qty.ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = textBlock,
                };
                Grid.SetRow(border, row);
                Grid.SetColumn(border, 2);
                PanelsList.Children.Add(border);

                row++;
            }

            DataContext = info;
        }
    }
}
