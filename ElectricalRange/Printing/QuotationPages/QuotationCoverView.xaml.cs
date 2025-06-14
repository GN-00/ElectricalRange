using ProjectsNow.Data.Quotations;

using System.Collections.Generic;

namespace ProjectsNow.Printing.QuotationPages
{
    public partial class QuotationCoverView : PageBase
    {
        public QuotationCoverView(Quotation quotation, List<QuotationContent> contents)
        {
            InitializeComponent();
            DataContext = new QuotationCoverViewModel(quotation, contents);
        }
    }
}
