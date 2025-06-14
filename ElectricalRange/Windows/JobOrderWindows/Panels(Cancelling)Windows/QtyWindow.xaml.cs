using Dapper;

using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Cancelling_Windows
{
    public partial class QtyWindow : Window
    {
        public JPanel PanelData { get; set; }
        public ObservableCollection<TransactionPanel> PanelsTransaction { get; set; }

        public QtyWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PanelToCancelInput.Text = PanelData.HoldQty.ToString();
            DataContext = PanelData;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Cancelling_Click(object sender, RoutedEventArgs e)
        {
            string query;
            int qty = int.Parse(PanelToCancelInput.Text);
            if (qty > 0)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    PanelData.CanceledQty += qty;
                    PanelData.HoldQty -= qty;
                    query = $"Insert Into [JobOrder].[PanelsTransactions] " +
                            $"(PanelID, Reference, Qty, Date, Action) " +
                            $"Values " +
                            $"(@PanelID, @Reference, @Qty, @Date, 'Canceled') Select @@IDENTITY";

                    TransactionPanel newPanel = new()
                    {
                        JobOrderID = PanelData.JobOrderID,
                        PanelID = PanelData.PanelID,
                        PanelSN = PanelData.PanelSN,
                        PanelName = PanelData.PanelName,
                        EnclosureType = PanelData.EnclosureType,
                        Qty = qty,
                        Date = DateTime.Now,
                    };

                    newPanel.TransactionID = (int)(decimal)connection.ExecuteScalar(query, newPanel);
                    PanelsTransaction.Add(newPanel);
                }

                Close();
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void PanelToCancelInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PanelToCancelInput.Text))
            {
                PanelToCancelInput.Text = PanelData.HoldQty.ToString();
            }
            else
            {
                int qty = int.Parse(PanelToCancelInput.Text);
                if (qty > PanelData.HoldQty)
                {
                    PanelToCancelInput.Text = PanelData.HoldQty.ToString();
                }
            }
        }
        private void PanelToCancelInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 4);
        }
    }
}
