using Dapper;

using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Hold_Windows
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
            PanelToHoldInput.Text = PanelData.ReadyToHoldQty.ToString();
            DataContext = PanelData;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Hold_Click(object sender, RoutedEventArgs e)
        {
            string query;
            int qty = int.Parse(PanelToHoldInput.Text);
            if (qty > 0)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    PanelData.HoldQty += qty;
                    query = $"Insert Into [JobOrder].[PanelsTransactions] " +
                             $"(PanelID, Reference, Qty, Date, Action) " +
                             $"Values " +
                             $"(@PanelID, @Reference, @Qty, @Date, 'Hold') Select @@IDENTITY";

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
        private void PanelToHoldInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PanelToHoldInput.Text))
            {
                PanelToHoldInput.Text = PanelData.ReadyToHoldQty.ToString();
            }
            else
            {
                int qty = int.Parse(PanelToHoldInput.Text);
                if (qty > PanelData.ReadyToHoldQty)
                {
                    PanelToHoldInput.Text = PanelData.ReadyToHoldQty.ToString();
                }
            }
        }
        private void PanelToHoldInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 4);
        }
    }
}
