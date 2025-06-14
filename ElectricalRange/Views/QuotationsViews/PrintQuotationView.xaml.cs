using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;

using System.Collections.Generic;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class PrintQuotationView : UserControl, IView
    {
        public PrintQuotationView(Quotation quotation, List<BillPanel> panels = null)
        {
            InitializeComponent();
            DataContext = new PrintQuotationViewModel(quotation, panels);
        }
    }
}
