using ProjectsNow.Data.JobOrders;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.JobOrderPages
{
    public partial class WarrantyCertificatePanelsForm : PageBase
    {
        public WarrantyCertificatePanelsForm(List<WPanel> panels)
        {
            InitializeComponent();
            int rows = 0;
            Border border;
            TextBlock textBlock;
            foreach (WPanel panel in panels)
            {
                rows++;
                Table.RowDefinitions.Add(new RowDefinition() { MinHeight = 30 });

                //SN
                textBlock = new TextBlock()
                {
                    Text = panel.SN.ToString(),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5,2,5,2),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    Child = textBlock,
                    BorderThickness = new Thickness(1,0,1,1)
                };
                Grid.SetRow(border, rows);
                Grid.SetColumn(border, 0);
                Table.Children.Add(border);

                //Name
                textBlock = new TextBlock()
                {
                    Text = panel.Name.ToString(),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5, 2, 5, 2),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                border = new Border()
                {
                    Child = textBlock,
                    BorderThickness = new Thickness(0, 0, 1, 1)
                };
                Grid.SetRow(border, rows);
                Grid.SetColumn(border, 1);
                Table.Children.Add(border);

                //Qty
                textBlock = new TextBlock()
                {
                    Text = panel.Qty.ToString(),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5, 2, 5, 2),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border = new Border()
                {
                    Child = textBlock,
                    BorderThickness = new Thickness(0, 0, 1, 1)
                };
                Grid.SetRow(border, rows);
                Grid.SetColumn(border, 2);
                Table.Children.Add(border);
            }

            DataContext = new WarrantyCertificateViewModel(null);
        }
    }
}
