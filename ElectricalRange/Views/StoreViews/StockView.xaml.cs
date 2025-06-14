using System.Windows.Controls;

namespace ProjectsNow.Views.StoreViews
{
    public partial class StockView : UserControl, IView
    {
        public StockView()
        {
            InitializeComponent();
            DataContext = new StockViewModel(this);
        }
    }
}
