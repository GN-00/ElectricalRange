using ProjectsNow.Data.Quotations;

using System.Collections.Generic;

namespace ProjectsNow.Printing.QuotationPages
{
    internal class QuotationCoverViewModel : PageViewModelBase
    {
        public QuotationCoverViewModel(Quotation quotation, List<QuotationContent> contents)
        {
            QuotationData = quotation;
            ContentsData = contents;
        }

        public Quotation QuotationData { get; }
        public List<QuotationContent> ContentsData { get; }
    }
}