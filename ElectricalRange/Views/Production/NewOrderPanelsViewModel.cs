namespace ProjectsNow.Views.Production
{
    internal class NewOrderPanelsViewModel
    {
        private NewOrderPanelsView newOrderPanelsView;

        public NewOrderPanelsViewModel(Data.Production.Order order, NewOrderPanelsView newOrderPanelsView)
        {
            this.newOrderPanelsView = newOrderPanelsView;
        }
    }
}