using ProjectsNow.Data.Production;

namespace ProjectsNow.Views.Production
{
    internal class OrderPanelsViewModel
    {
        private Order order;
        private IView view;

        public OrderPanelsViewModel(Order order, IView view)
        {
            this.order = order;
            this.view = view;
        }
    }
}