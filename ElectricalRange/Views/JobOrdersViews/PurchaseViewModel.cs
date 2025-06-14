using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

namespace ProjectsNow.Views.JobOrdersViews
{
    internal class PurchaseViewModel : ViewModelBase
    {
        public PurchaseViewModel(JobOrder jobOrder, IView checkPoint)
        {
            ViewData = checkPoint;
            JobOrderData = jobOrder;

            QuotationsCommand = new RelayCommand(Quotations, CanAccessQuotations);
            OrdersCommand = new RelayCommand(Orders, CanAccessOrders);
        }

        public JobOrder JobOrderData { get; }
        public RelayCommand QuotationsCommand { get; }
        public RelayCommand OrdersCommand { get; }

        private void Quotations()
        {
            Navigation.ClosePopup();

            Windows.JobOrderWindows.LookingForQuotations.QuotationsRequestsWindow requestsWindow = 
                new()
                {
                    UserData = Navigation.UserData,
                    JobOrderData = JobOrderData,
                };
            _ = requestsWindow.ShowDialog();

        }
        private bool CanAccessQuotations()
        {
            return true;
        }

        private void Orders()
        {
            Navigation.ClosePopup();

            //Windows.JobOrderWindows.ItemPurchaseOrdersWindows.PurchaseOrdersWindow purchaseOrdersWindow =
            //    new Windows.JobOrderWindows.ItemPurchaseOrdersWindows.PurchaseOrdersWindow()
            //    {
            //        UserData = Navigation.UserData,
            //        JobOrderData = JobOrderData,
            //    };
            //_ = purchaseOrdersWindow.ShowDialog();

            Navigation.To(new ItemsPurchaseOrdersViews.PurchaseOrdersView(JobOrderData), ViewData);

        }
        private bool CanAccessOrders()
        {
            return true;
        }
    }
}