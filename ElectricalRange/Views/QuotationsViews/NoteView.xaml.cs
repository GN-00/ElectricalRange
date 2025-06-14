using ProjectsNow.Data.Quotations;

using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class NoteView : UserControl, IPopup
    {
        public NoteView(Quotation quotation)
        {
            InitializeComponent();
            DataContext = new NoteViewModel(quotation);
        }
    }
}
