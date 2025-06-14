using ProjectsNow.Data.Quotations;

using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class ItemsView : UserControl, IView
    {
        public ItemsView(Quotation quotation, QPanel panel)
        {
            InitializeComponent();
            DataContext = new ItemsViewModel(quotation, panel, this);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.grid.Focus();
        }
    }
}
