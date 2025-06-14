using System.Windows.Controls;

namespace ProjectsNow.Views.InquiriesViews
{
    public partial class InquiriesView : UserControl, IView
    {
        public InquiriesView()
        {
            InitializeComponent();
            DataContext = new InquiriesViewModel(this);
        }
    }
}
