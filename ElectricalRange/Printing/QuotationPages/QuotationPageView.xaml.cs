using ProjectsNow.Data.Quotations;

using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.QuotationPages
{
    public partial class QuotationPageView : PageBase
    {
        public QuotationPageView(Quotation quotation, Panel content, Visibility isContinue = Visibility.Collapsed)
        {
            InitializeComponent();
            DataContext = new QuotationPageViewModel(quotation, content, isContinue);
        }
    }
}
