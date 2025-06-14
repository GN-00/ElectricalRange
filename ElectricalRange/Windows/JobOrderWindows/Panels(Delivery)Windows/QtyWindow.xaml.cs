using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Delivery_Windows
{
    public partial class QtyWindow : Window
    {
        public JPanel PanelData { get; set; }
        public Delivery DeliveryData { get; set; }
        public ObservableCollection<TransactionPanel> PanelsTransaction { get; set; }

        public QtyWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PanelToDeliveryInput.Text = PanelData.ReadyToDeliverQty.ToString();
            DataContext = PanelData;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Deliver_Click(object sender, RoutedEventArgs e)
        {
            int qty = int.Parse(PanelToDeliveryInput.Text);
            if (qty > 0)
            {
                PanelData.DeliveredQty += qty;

                TransactionPanel newPanel = new()
                {
                    JobOrderID = PanelData.JobOrderID,
                    PanelID = PanelData.PanelID,
                    PanelSN = PanelData.PanelSN,
                    PanelName = PanelData.PanelName,
                    EnclosureType = PanelData.EnclosureType,
                    Qty = qty,
                    Reference = DeliveryData.Number,
                    Date = DeliveryData.Date,
                };
                PanelsTransaction.Add(newPanel);

                Close();
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void PanelToDeliveryInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PanelToDeliveryInput.Text))
            {
                PanelToDeliveryInput.Text = PanelData.ReadyToInvoicedQty.ToString();
            }
            else
            {
                int qty = int.Parse(PanelToDeliveryInput.Text);
                if (qty > PanelData.ReadyToDeliverQty)
                {
                    PanelToDeliveryInput.Text = PanelData.ReadyToDeliverQty.ToString();
                }
            }
        }
        private void PanelToDeliveryInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 4);
        }
    }
}
