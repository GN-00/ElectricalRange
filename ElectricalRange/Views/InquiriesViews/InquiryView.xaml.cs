using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.InquiriesViews
{
    public partial class InquiryView : UserControl, IView
    {
        public InquiryView(Inquiry inquiry, ObservableCollection<Inquiry> inquiries)
        {
            InitializeComponent();
            DataContext = new InquiryViewModel(inquiry, inquiries, this);
        }

        public InquiryView(Inquiry inquiry, Quotation quotation)
        {
            InitializeComponent();
            DataContext = new InquiryViewModel(inquiry, quotation, this);
        }
    }
}
