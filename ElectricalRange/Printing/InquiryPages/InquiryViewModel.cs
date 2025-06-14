using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;

namespace ProjectsNow.Printing.InquiryPages
{
    internal class InquiryViewModel: ViewModelBase
    {
        public InquiryViewModel(InquiryInfo info)
        {
            Data = info;
        }

        public InquiryInfo Data { get; }
    }
}