
using Dapper;
using Microsoft.Data.SqlClient;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;
using ProjectsNow.Data.Users;
using System.Collections.ObjectModel;

namespace ProjectsNow.Views.Production
{
    public class KanbanViewModel : ViewModelBase
    {
        private ObservableCollection<ProductionPanel> _ToDoItems;
        private ObservableCollection<ProductionPanel> _InProgressItems;
        private ObservableCollection<ProductionPanel> _QualityCheckItems;
        private ObservableCollection<ProductionPanel> _ReadyForDeliveryItems;
        private ObservableCollection<ProductionPanel> _CompletedItems;

        public KanbanViewModel(Order order, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            OrderData = order;

            LoadKanbanData();

            MoveToInProgressCommand = new RelayCommand<ProductionPanel>(MoveToInProgress);
            MoveToQualityCheckCommand = new RelayCommand<ProductionPanel>(MoveToQualityCheck);
            MoveToReadyForDeliveryCommand = new RelayCommand<ProductionPanel>(MoveToReadyForDelivery);
            MoveToCompletedCommand = new RelayCommand<ProductionPanel>(MoveToCompleted);
            RefreshCommand = new RelayCommand(LoadKanbanData);
        }

        public User UserData { get; }
        public Order OrderData { get; }

        public ObservableCollection<ProductionPanel> ToDoItems
        {
            get => _ToDoItems;
            set => SetValue(ref _ToDoItems, value);
        }

        public ObservableCollection<ProductionPanel> InProgressItems
        {
            get => _InProgressItems;
            set => SetValue(ref _InProgressItems, value);
        }

        public ObservableCollection<ProductionPanel> QualityCheckItems
        {
            get => _QualityCheckItems;
            set => SetValue(ref _QualityCheckItems, value);
        }

        public ObservableCollection<ProductionPanel> ReadyForDeliveryItems
        {
            get => _ReadyForDeliveryItems;
            set => SetValue(ref _ReadyForDeliveryItems, value);
        }

        public ObservableCollection<ProductionPanel> CompletedItems
        {
            get => _CompletedItems;
            set => SetValue(ref _CompletedItems, value);
        }

        public RelayCommand<ProductionPanel> MoveToInProgressCommand { get; }
        public RelayCommand<ProductionPanel> MoveToQualityCheckCommand { get; }
        public RelayCommand<ProductionPanel> MoveToReadyForDeliveryCommand { get; }
        public RelayCommand<ProductionPanel> MoveToCompletedCommand { get; }
        public RelayCommand RefreshCommand { get; }

        private void LoadKanbanData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            
            // Load panels based on their status
            string query = $"SELECT * FROM [Production].[Panels(View)] WHERE JobOrderId = {OrderData.JobOrderId}";
            var allPanels = connection.Query<ProductionPanel>(query).ToList();

            // Categorize panels based on their current status
            ToDoItems = new ObservableCollection<ProductionPanel>(
                allPanels.Where(p => p.Items == 0 || p.ReceivedItems == 0));

            InProgressItems = new ObservableCollection<ProductionPanel>(
                allPanels.Where(p => p.ReceivedItems > 0 && p.ReceivedItems < p.Items && p.ClosedQty == 0));

            QualityCheckItems = new ObservableCollection<ProductionPanel>(
                allPanels.Where(p => p.ReceivedItems == p.Items && p.ClosedQty == 0));

            ReadyForDeliveryItems = new ObservableCollection<ProductionPanel>(
                allPanels.Where(p => p.ClosedQty > 0 && p.ClosedQty < p.Qty));

            CompletedItems = new ObservableCollection<ProductionPanel>(
                allPanels.Where(p => p.ClosedQty == p.Qty));
        }

        private void MoveToInProgress(ProductionPanel panel)
        {
            if (panel == null) return;
            
            // Update panel status in database
            UpdatePanelStatus(panel, "InProgress");
            LoadKanbanData();
        }

        private void MoveToQualityCheck(ProductionPanel panel)
        {
            if (panel == null) return;
            
            // Update panel status in database
            UpdatePanelStatus(panel, "QualityCheck");
            LoadKanbanData();
        }

        private void MoveToReadyForDelivery(ProductionPanel panel)
        {
            if (panel == null) return;
            
            // Update panel status in database
            UpdatePanelStatus(panel, "ReadyForDelivery");
            LoadKanbanData();
        }

        private void MoveToCompleted(ProductionPanel panel)
        {
            if (panel == null) return;
            
            // Update panel status in database
            UpdatePanelStatus(panel, "Completed");
            LoadKanbanData();
        }

        private void UpdatePanelStatus(ProductionPanel panel, string status)
        {
            using SqlConnection connection = new(Database.ConnectionString);
            
            // This would need to be implemented based on your database schema
            // For now, this is a placeholder for the status update logic
            string updateQuery = $@"
                UPDATE [Production].[Panels] 
                SET Status = @Status, 
                    LastUpdated = @LastUpdated 
                WHERE PanelId = @PanelId";

            connection.Execute(updateQuery, new 
            { 
                Status = status, 
                LastUpdated = DateTime.Now, 
                PanelId = panel.PanelId 
            });
        }
    }
}
