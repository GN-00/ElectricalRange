using ProjectsNow.Data.Quotations;

using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.QuotationPages
{
    internal class QuotationPageViewModel : PageViewModelBase
    {
        public QuotationPageViewModel(Quotation quotation, Panel content, Visibility isContinue)
        {
            QuotationData = quotation;
            ContentData = content;
            IsContinue = isContinue;
        }

        public Quotation QuotationData { get; }
        public Panel ContentData { get; }
        public Visibility IsContinue { get; }
    }
}