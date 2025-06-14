using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class TermView : UserControl, IPopup
    {
        public TermView(Term term, ObservableCollection<Term> terms)
        {
            InitializeComponent();
            DataContext = new TermViewModel(term, terms);
        }
    }
}
