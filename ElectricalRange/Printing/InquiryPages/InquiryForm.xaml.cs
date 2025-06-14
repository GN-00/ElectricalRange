using ProjectsNow.Data.Inquiries;

using System.Windows.Controls;

namespace ProjectsNow.Printing.InquiryPages
{
    public partial class InquiryForm : UserControl
    {
        public InquiryForm(InquiryInfo info)
        {
            InitializeComponent();
            DataContext = new InquiryViewModel(info);
        }
    }
}
