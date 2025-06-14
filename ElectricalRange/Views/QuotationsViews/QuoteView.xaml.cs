using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class QuoteView : UserControl, IView
    {
        public QuoteView()
        {
            InitializeComponent();
            DataContext = new QuoteViewModel();
        }
    }
}
