using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Delivery_Windows
{
    public partial class PanelsWindow : Window
    {
        public Delivery DeliveryData { get; set; }
        public ObservableCollection<JPanel> PanelsData { get; set; }
        public ObservableCollection<TransactionPanel> PanelsTransaction { get; set; }

        private CollectionViewSource viewData;
        public PanelsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewData = new CollectionViewSource() { Source = PanelsData };
            viewData.Filter += DataFilter;

            PanelsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (viewData.View.Cast<object>().Count() == 0)
            {
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                Navigation.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            }
            else
            {
                Navigation.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
            }
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
            {
                Navigation.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            }
            else
            {
                Navigation.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
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
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is JPanel panelData)
            {
                QtyWindow qtyWindow = new()
                {
                    DeliveryData = DeliveryData,
                    PanelData = panelData,
                    PanelsTransaction = PanelsTransaction,
                };
                _ = qtyWindow.ShowDialog();
                viewData.View.Refresh();
            }
        }

        #region Filters
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JPanel panel)
                {
                    if (panel.NotDeliveredQty == 0)
                    {
                        e.Accepted = false;
                        return;
                    }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }
        #endregion
    }
}
